using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KiriFig.Model
{
    public class Figure
    {
        public int Width { get; }
        public int Height { get; }
        public int OffsetX { get; }
        public int OffsetY { get; }

        private List<Shape> _shapes = new List<Shape>();
        public ReadOnlyCollection<Shape> Shapes { get; }

        private List<Contour> _contours = new List<Contour>();
        public ReadOnlyCollection<Contour> Contours { get; }

        private List<PointGroup> _pointGroups = new List<PointGroup>();
        public IReadOnlyList<PointGroup> PointGroups { get; }

        private List<Cut> _cuts = new List<Cut>();
        public IReadOnlyList<Cut> Cuts { get; }

        public Figure(IEnumerable<PointGroup> pointGroups, IEnumerable<Cut> cuts, int width, int height, int offsetX, int offsetY)
        {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;

            ValidatePointGroups(pointGroups);
            ValidateCuts(cuts);

            var contours = new HashSet<Contour>();

            foreach (var group in pointGroups)
            {
                foreach (var point in group.Points)
                {
                    contours.Add(point.Contour);
                }
            }

            var shapes = new HashSet<Shape>();

            foreach (var contour in contours)
            {
                shapes.Add(contour.Shape);
            }

            _pointGroups.AddRange(pointGroups);
            _contours.AddRange(contours);
            _shapes.AddRange(shapes);
            _cuts.AddRange(cuts);

            PointGroups = _pointGroups.AsReadOnly();
            Contours = _contours.AsReadOnly();
            Shapes = _shapes.AsReadOnly();
            Cuts = _cuts.AsReadOnly();
        }

        private void ValidatePointGroups(IEnumerable<PointGroup> pointGroups)
        {
            foreach (var group in pointGroups)
            {
                if (group.Points.Count == 0)
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

                if (group.IsFixed)
                {
                    if (group.OriginalIncomingEdge == null
                        || group.OriginalOutgoingEdge == null
                        )
                        throw new ApplicationException();
                }
            }
        }

        private void ValidateCuts( IEnumerable<Cut> cuts )
        {
            foreach (var cut in cuts)
            {
                if ( cut.PairedEdge1 == null
                    || cut.PairedEdge2 == null
                    || cut.PairedEdge1.Type != EdgeType.Line
                    || cut.PairedEdge1.P1 == null
                    || cut.PairedEdge1.P2 == null
                    || cut.PairedEdge1.P1 == cut.PairedEdge1.P2
                    || cut.PairedEdge2.Type != EdgeType.Line
                    || cut.PairedEdge2.P1 == null
                    || cut.PairedEdge2.P2 == null
                    || cut.PairedEdge2.P1 == cut.PairedEdge2.P2
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

            splitResult.nearestPoint.IncomingEdge = splitResult.edgeBefore;
            splitResult.nearestPoint.OutgoingEdge = splitResult.edgeAfter;

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
            if (CutExists(point1.Group, point2.Group))
                throw new ApplicationException("Only one cut is allowed between these point groups.");

            if (point1.OutgoingEdge.P2 == point2 || point1.IncomingEdge.P1 == point2)
                throw new ApplicationException("Cuts are not allowed between directly connected points.");

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

            // Split or combine contours and shapes.

            Contour oldContour1 = point1.Contour;
            Contour oldContour2 = point2.Contour;

            ModifyContours(point1, newPoint1);
            ModifyShapes(oldContour1, oldContour2, point1.Contour, newPoint1.Contour);

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

            Point keepPoint1 = null;
            Point keepPoint2 = null;
            Point deletePoint1 = null;
            Point deletePoint2 = null;
            var edge1 = cut.PairedEdge1;
            var edge2 = cut.PairedEdge2;

            // Link points from one edge to the other.

            edge1.P1.OutgoingEdge = edge2.P2.OutgoingEdge;
            edge1.P1.OutgoingEdge.P1 = edge1.P1;
            keepPoint1 = edge1.P1;
            deletePoint1 = edge2.P1;

            edge1.P2.IncomingEdge = edge2.P1.IncomingEdge;
            edge1.P2.IncomingEdge.P2 = edge1.P2;
            keepPoint2 = edge1.P2;
            deletePoint2 = edge2.P2;

            // Remove extra points from their groups.

            deletePoint1.Group.Points.Remove(deletePoint1);
            deletePoint2.Group.Points.Remove(deletePoint2);

            // Split or combine contours and shapes.

            Contour oldContour1 = edge1.P1.Contour;
            Contour oldContour2 = edge2.P1.Contour;

            ModifyContours(keepPoint1, keepPoint2);
            ModifyShapes(oldContour1, oldContour2, keepPoint1.Contour, keepPoint2.Contour);

            // Delete the cut.

            _cuts.Remove(cut);
        }

        private void ModifyShapes(Contour oldContour1, Contour oldContour2, Contour newContour1, Contour newContour2)
        {
            // Get the old shapes.

            Shape oldShape1 = oldContour1.Shape;
            Shape oldShape2 = oldContour2.Shape;

            // Collect contours except old ones.

            var contours = new List<Contour>();

            foreach (var contour in oldShape1.Contours)
            {
                if (contour != oldContour1 && contour != oldContour2)
                    contours.Add(contour);
            }

            if (oldShape2 != oldShape1)
            {
                foreach (var contour in oldShape2.Contours)
                {
                    if (contour != oldContour1 && contour != oldContour2)
                        contours.Add(contour);
                }
            }

            // Add new contours.

            contours.Add(newContour1);

            if (newContour2 != newContour1)
                contours.Add(newContour2);

            // Recalculate shapes.

            var tool = new OutlineTool(contours);
            var newShapes = tool.CalculateShapes();

            // Assign new shapes to all contours.

            foreach (var shape in newShapes)
            {
                foreach (var contour in shape.Contours)
                {
                    contour.Shape = shape;
                }
            }

            // Replace old shapes with new ones.

            _shapes.Remove(oldShape1);
            _shapes.Remove(oldShape2);

            _shapes.AddRange(newShapes);
        }


        #region FindPointsForCut

        private struct FindPointsResult
        {
            public float DotProduct;
            public Point Point1;
            public Point Point2;

            public FindPointsResult(float dotProduct, Point point1, Point point2)
            {
                DotProduct = dotProduct;
                Point1 = point1;
                Point2 = point2;
            }
        }

        public static (Point, Point) FindPointsForCut(PointGroup pointGroup1, PointGroup pointGroup2)
        {
            // Keep in mind, only points in the same shape are good.

            var results = new List<FindPointsResult>();

            // First, look for two points on the outside.

            foreach (var point1 in pointGroup1.Points)
            {
                foreach (var point2 in pointGroup2.Points)
                {
                    if (point1.Contour.Shape == point2.Contour.Shape
                        && ArePointsApart(point1, point2))
                    {
                        float dotProduct = GetMutualDotProduct(point1, point2);

                        results.Add(new FindPointsResult(dotProduct, point1, point2));
                    }
                }
            }

            if (results.Count > 0)
                return FindBestPoints(results);

            // Next, look for an outside-inside or inside-inside pair of points.
            // They must be in different contours.

            foreach (var point1 in pointGroup1.Points)
            {
                foreach (var point2 in pointGroup2.Points)
                {
                    if (point1.Contour.Shape == point2.Contour.Shape
                        && point1.Contour != point2.Contour
                        && ArePointsApart(point1, point2))
                    {
                        float dotProduct = GetMutualDotProduct(point1, point2);

                        results.Add(new FindPointsResult(dotProduct, point1, point2));
                    }
                }
            }

            if (results.Count > 0)
                return FindBestPoints(results);

            return (null, null);
        }

        private static (Point, Point) FindBestPoints(List<FindPointsResult> results)
        {
            Debug.Assert(results.Count > 0);

            float maxDotProduct = float.NegativeInfinity;
            int index = -1;

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].DotProduct > maxDotProduct)
                {
                    maxDotProduct = results[i].DotProduct;
                    index = i;
                }
            }

            return (results[index].Point1, results[index].Point2);
        }

        private static bool ArePointsApart(Point point1, Point point2)
        {
            // Points have to be more than one edge apart.

            if (   point1.OutgoingEdge.P2 == point2
                || point2.OutgoingEdge.P2 == point1)
                return false;

            return true;
        }

        // Cutting outside points must yield two shapes.

        private static float GeDotProductToTarget(Point jointPoint, Point targetPoint)
        {
            Point p1, p2, p3;
            float angle;

            p1 = jointPoint.IncomingEdge.P1;
            p2 = jointPoint;
            p3 = jointPoint.OutgoingEdge.P2;

            angle = Utils.GetBisectorAngle(p1, p2, p3);

            return Utils.GetAngleDotProduct(angle, p2, targetPoint);
        }

        // The joint at each point should point at the other point.

        private static float GetMutualDotProduct(Point point1, Point point2)
        {
            float dotProduct1 = GeDotProductToTarget(point1, point2);
            float dotProduct2 = GeDotProductToTarget(point2, point1);

            return dotProduct1 + dotProduct2;
        }

        #endregion
    }
}
