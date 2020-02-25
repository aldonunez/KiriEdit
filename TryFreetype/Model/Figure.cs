using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TryFreetype.Model
{
    public class Figure
    {
        public int Width { get; }
        public int Height { get; }
        public double OffsetX { get; }
        public double OffsetY { get; }

        private List<Contour> _contours = new List<Contour>();
        public ReadOnlyCollection<Contour> Contours { get; }

        private List<PointGroup> _pointGroups = new List<PointGroup>();
        public IReadOnlyList<PointGroup> PointGroups { get; }

        private List<Cut> _cuts = new List<Cut>();
        public IReadOnlyList<Cut> Cuts { get; }

        public Figure(IEnumerable<PointGroup> pointGroups, int width, int height, double offsetX, double offsetY)
        {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;

            ValidatePointGroups(pointGroups);

            var contours = new HashSet<Contour>();

            foreach (var group in pointGroups)
            {
                contours.Add(group.Points[0].Contour);
            }

            _pointGroups.AddRange(pointGroups);
            _contours.AddRange(contours);

            PointGroups = _pointGroups.AsReadOnly();
            Contours = _contours.AsReadOnly();
            Cuts = _cuts.AsReadOnly();
        }

        private void ValidatePointGroups(IEnumerable<PointGroup> pointGroups)
        {
            foreach (var group in pointGroups)
            {
                if (!group.IsFixed)
                    throw new ApplicationException("");
                if (group.Points.Count != 1)
                    throw new ApplicationException("");

                var point = group.Points[0];

                if (point.Contour == null
                    || point.Group != group
                    || point.IncomingEdge == null
                    || point.OutgoingEdge == null
                    || point.IncomingEdge.P2 != point
                    || point.OutgoingEdge.P1 != point
                    || point.Contour.FirstPoint == null
                    )
                    throw new ApplicationException("");

                if (group.OriginalIncomingEdge == null
                    || group.OriginalOutgoingEdge == null
                    )
                    throw new ApplicationException();
            }
        }

        public Point AddDiscardablePoint(Point point, Edge edge)
        {
            if (edge.Unbreakable)
                throw new ApplicationException("");

            var splitResult = SplitEdge(point, edge);

            var newGroup = new PointGroup();
            newGroup.Points.Add(splitResult.nearestPoint);
            splitResult.nearestPoint.Group = newGroup;
            splitResult.nearestPoint.Contour = edge.P1.Contour;
            _pointGroups.Add(newGroup);

            return splitResult.nearestPoint;
        }

        private SplitResult SplitEdge(Point point, Edge edge)
        {
            var splitResult = edge.Split(point);

            edge.P1.OutgoingEdge = splitResult.edgeBefore;
            edge.P2.IncomingEdge = splitResult.edgeAfter;

            return splitResult;
        }

        public void DeleteDiscardablePoint(PointGroup pointGroup)
        {
            if (pointGroup.IsFixed)
                throw new ApplicationException("Can't delete a fixed point group.");

            if (pointGroup.Points.Count > 1)
                throw new ApplicationException("Can't delete a point group that has more than one point.");

            Debug.Assert(pointGroup.Points.Count == 1);

            Point pointToDelete = pointGroup.Points[0];

            // Find the original edge and endpoints.

            Point fixedPointBefore = pointToDelete;

            while (!fixedPointBefore.Group.IsFixed)
            {
                fixedPointBefore = fixedPointBefore.IncomingEdge.P1;
            }

            Point fixedPointAfter = pointToDelete;

            while (!fixedPointAfter.Group.IsFixed)
            {
                fixedPointAfter = fixedPointAfter.OutgoingEdge.P2;
            }

            // Collect the points between the endpoints.

            Edge firstEdgeToReplace = fixedPointBefore.OutgoingEdge;

            // Replace the path between the original endpoints with the original edge.

            Edge directEdge = (Edge) fixedPointBefore.Group.OriginalOutgoingEdge.Clone();

            directEdge.P1 = fixedPointBefore;
            directEdge.P2 = fixedPointAfter;

            fixedPointBefore.OutgoingEdge = directEdge;
            fixedPointAfter.IncomingEdge = directEdge;

            // Add the points we collected except for the one to remove.

            var edge = directEdge;

            for ( Point p = firstEdgeToReplace.P2; p != fixedPointAfter; p = p.OutgoingEdge.P2 )
            {
                if (p == pointToDelete)
                    continue;

                var splitResult = SplitEdge(p, edge);

                edge = splitResult.edgeAfter;
            }

            _pointGroups.Remove(pointGroup);

            // We might have deleted the contour's anchor point.

            if (pointToDelete.Contour.FirstPoint == pointToDelete)
                pointToDelete.Contour.FirstPoint = fixedPointBefore;
        }

        // TODO: Do this in the caller.
        // Find a point in each group that share the same contour: old points

        public Cut AddCut(Point point1, Point point2)
        {
            // Only allow one cut between the same point groups.

            if (CutExists(point1.Group, point2.Group))
                throw new ApplicationException("Only one cut is allowed between these point groups.");

            // Add a point to each point group: new points

            Point newPoint1 = point1.Group.MakePoint();
            point1.Group.Points.Add(newPoint1);

            Point newPoint2 = point2.Group.MakePoint();
            point2.Group.Points.Add(newPoint2);

            // Move half the edges from the old points to the new points.

            newPoint1.IncomingEdge = point1.IncomingEdge;
            newPoint1.IncomingEdge.P2 = newPoint1;
            point1.IncomingEdge = null;

            newPoint2.OutgoingEdge = point2.OutgoingEdge;
            newPoint2.OutgoingEdge.P1 = newPoint2;
            point2.OutgoingEdge = null;

            // Add a line edge between old points and between new points.

            LineEdge lineForNew = new LineEdge(newPoint1, newPoint2, unbreakable: true);
            LineEdge lineForOld = new LineEdge(point2, point1, unbreakable: true);

            newPoint1.OutgoingEdge = lineForNew;
            newPoint2.IncomingEdge = lineForNew;

            point1.IncomingEdge = lineForOld;
            point2.OutgoingEdge = lineForOld;

            // Split or combine contours.

            ModifyContours(point1, newPoint1);

            // Save cut object.

            Cut cut = new Cut(lineForNew, lineForOld);

            _cuts.Add(cut);

            return cut;
        }

        private bool CutExists(PointGroup group1, PointGroup group2)
        {
            foreach (var cut in _cuts)
            {
                if ((cut.PairedEdge1.P1.Group == group1 && cut.PairedEdge1.P2.Group == group2)
                    || (cut.PairedEdge1.P1.Group == group2 && cut.PairedEdge1.P2.Group == group1))
                    return true;
            }

            return false;
        }

        private void ModifyContours(Point point1, Point point2)
        {
            Point p = point1.OutgoingEdge.P2;
            bool separateContours = false;

            while (true)
            {
                if (p == point1)
                {
                    // The points are in different contours.
                    separateContours = true;
                    break;
                }
                else if (p == point2)
                {
                    // The points are in the same contour.
                    separateContours = false;
                    break;
                }

                p = p.OutgoingEdge.P2;
            }

            ReplaceContours(point1);

            if (separateContours)
                ReplaceContours(point2);
        }

        private void ReplaceContours(Point point)
        {
            Contour contour = new Contour();
            Point p = point;

            do
            {
                _contours.Remove(p.Contour);
                p.Contour = contour;

                p = p.OutgoingEdge.P2;
            } while (p != point);

            contour.FirstPoint = point;
            _contours.Add(contour);
        }

        public void DeleteCut(Cut cut)
        {
            if (!_cuts.Contains(cut))
                throw new ApplicationException("This cut doesn't exist.");

            Point point1 = null;
            Point point2 = null;
            var edge1 = cut.PairedEdge1;
            var edge2 = cut.PairedEdge2;

            // Link points from one edge to the other.

            edge1.P1.OutgoingEdge = edge2.P2.OutgoingEdge;
            edge1.P1.OutgoingEdge.P1 = edge1.P1;
            point1 = edge1.P1;

            edge1.P2.IncomingEdge = edge2.P1.IncomingEdge;
            edge1.P2.IncomingEdge.P2 = edge1.P2;
            point2 = edge1.P2;

            // Remove extra points from their groups.

            edge2.P1.Group.Points.Remove(edge2.P1);
            edge2.P2.Group.Points.Remove(edge2.P2);

            // Split or combine contours.

            ModifyContours(point1, point2);

            // Delete the cut.

            _cuts.Remove(cut);
        }
    }
}
