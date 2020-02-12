using System;
using System.Collections.Generic;

namespace TryFreetype.Sample2
{
    public class Figure
    {
        public List<Contour> Contours = new List<Contour>();
        public List<PointGroup> PointGroups = new List<PointGroup>();
        public List<Cut> Cuts = new List<Cut>();

        public Point AddDiscardablePoint(Point point, Edge edge)
        {
            var splitResult = edge.Split(point);

            edge.P1.OutgoingEdge = splitResult.edgeBefore;
            edge.P2.IncomingEdge = splitResult.edgeAfter;

            var newGroup = new PointGroup();
            newGroup.IsFixed = false;
            newGroup.Points.Add(splitResult.nearestPoint);
            splitResult.nearestPoint.Group = newGroup;
            splitResult.nearestPoint.Contour = edge.P1.Contour;
            PointGroups.Add(newGroup);

            return splitResult.nearestPoint;
        }

        public void DeleteDiscardablePoint(PointGroup pointGroup)
        {
            if (pointGroup.IsFixed)
                throw new ApplicationException("Can't delete a fixed point group.");

            if (pointGroup.Points.Count > 1)
                throw new ApplicationException("Can't delete a point group that has more than one point.");

            Point pointToDelete = pointGroup.Points[0];

            // Find the original edge and endpoints.

            Point fixedPointBefore = pointToDelete;

            while (!fixedPointBefore.Group.IsFixed)
            {
                fixedPointBefore = fixedPointBefore.IncomingEdge.P1;
            }

            Point fixedPointAfter = fixedPointBefore.OriginalOutgoingEdge.P2;

            // Collect the points between the endpoints.

            Edge directEdge = (Edge) fixedPointBefore.OriginalOutgoingEdge.Clone();
            Edge firstEdgeToReplace = fixedPointBefore.OutgoingEdge;

            // Replace the path between the original endpoints with the original edge.

            fixedPointBefore.OutgoingEdge = directEdge;
            fixedPointAfter.IncomingEdge = directEdge;

            // Add the points we collected except for the one to remove.

            var edge = directEdge;

            for ( Point p = firstEdgeToReplace.P2; p != fixedPointAfter; p = p.OutgoingEdge.P2 )
            {
                if (p == pointToDelete)
                    continue;

                var splitResult = edge.Split(p);

                edge.P1.OutgoingEdge = splitResult.edgeBefore;
                edge.P2.IncomingEdge = splitResult.edgeAfter;

                edge = splitResult.edgeAfter;
            }

            PointGroups.Remove(pointGroup);
        }

        public Cut AddCut(Point point1, Point point2)
        {
            // Only allow one cut between the same point groups.

            // TODO: Do this in the caller.
            // Find a point in each group that share the same contour: old points

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

            LineEdge lineForNew = new LineEdge();
            lineForNew.P1 = newPoint1;
            lineForNew.P2 = newPoint2;

            LineEdge lineForOld = new LineEdge();
            lineForOld.P1 = point2;
            lineForOld.P2 = point1;

            newPoint1.OutgoingEdge = lineForNew;
            newPoint2.IncomingEdge = lineForNew;

            point1.IncomingEdge = lineForOld;
            point2.OutgoingEdge = lineForOld;

            // Split or combine contours.

            ModifyContours(point1, newPoint1);

            // Save cut object.

            Cut cut = new Cut(lineForNew, lineForOld);

            Cuts.Add(cut);

            return cut;
        }

        private void ModifyContours(Point point1, Point point2)
        {
            Point p = point1.OutgoingEdge.P2;
            bool separateContours = false;

            Console.WriteLine("Starting at {0}, searching for {1}", point1, point2);

            while (true)
            {
                Console.WriteLine(p);

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
                Contours.Remove(p.Contour);
                p.Contour = contour;

                p = p.OutgoingEdge.P2;
            } while (p != point);

            contour.FirstPoint = point;
            Contours.Add(contour);
        }

        public void DeleteCut(Cut cut)
        {
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

            // Split or combine contours.

            ModifyContours(point1, point2);

            // Delete the cut.

            Cuts.Remove(cut);
        }
    }
}
