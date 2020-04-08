/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using KiriFig.Model;

namespace KiriFig
{
    internal class OutlineTool
    {
        private enum Orientation
        {
            Unknown,
            Inside,
            Outside
        }

        private IReadOnlyList<Contour> _contours;

        public OutlineTool(IReadOnlyList<Contour> contours)
        {
            _contours = contours;
        }

        private static Orientation GetOrientation(Contour contour)
        {
            int prevX = contour.FirstPoint.X;
            int prevY = contour.FirstPoint.Y;
            Point p = contour.FirstPoint;
            long area = 0;

            while (true)
            {
                var edge = p.OutgoingEdge;

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        area += (edge.P2.Y - prevY) * (edge.P2.X + prevX);
                        break;

                    case EdgeType.Conic:
                        {
                            var conicEdge = (ConicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.P2.Y - conicEdge.Control1.Y) * (conicEdge.P2.X + conicEdge.Control1.X);
                        }
                        break;

                    case EdgeType.Cubic:
                        {
                            var conicEdge = (CubicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.Control2.Y - conicEdge.Control1.Y) * (conicEdge.Control2.X + conicEdge.Control1.X);
                            area += (conicEdge.P2.Y - conicEdge.Control2.Y) * (conicEdge.P2.X + conicEdge.Control2.X);
                        }
                        break;
                }

                prevX = edge.P2.X;
                prevY = edge.P2.Y;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            if (area > 0)
                return Orientation.Inside;
            else if (area < 0)
                return Orientation.Outside;
            else
                return Orientation.Unknown;
        }

        private static Point FindRightmostPoint(Contour contour)
        {
            Point p = contour.FirstPoint;
            Point rightP = new Point(0, 0);

            while (true)
            {
                if (p.X > rightP.X)
                    rightP = p;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            return rightP;
        }

        private (List<Contour> outsideContours, List<Contour> insideContours) PartitionContours()
        {
            var outsideContours = new List<Contour>();
            var insideContours = new List<Contour>();

            foreach (var contour in _contours)
            {
                Orientation orientation = GetOrientation(contour);

                switch (orientation)
                {
                    case Orientation.Inside:
                        insideContours.Add(contour);
                        break;

                    case Orientation.Outside:
                        outsideContours.Add(contour);
                        break;

                    default:
                        Debug.Fail("");
                        break;
                }
            }

            return (outsideContours, insideContours);
        }

        private struct CrossingInfo
        {
            public int Number;
            public int Distance;
        }

        private void TestLineCrossing(Point p, Point p1, Point p2, ref CrossingInfo crossing)
        {
            if (   (p1.Y <= p.Y && p2.Y  > p.Y)
                || (p1.Y  > p.Y && p2.Y <= p.Y))
            {
                float proportion = (float) (p.Y - p1.Y) / (p2.Y - p1.Y);
                float intersectX = p1.X + proportion * (p2.X - p1.X);

                if (p.X < intersectX)
                {
                    crossing.Number++;

                    if (intersectX < crossing.Distance)
                        crossing.Distance = (int) intersectX;
                }
            }
        }

        private void TestCrossing(Point p, Edge edge, ref CrossingInfo crossing)
        {
            if (edge is LineEdge)
            {
                TestLineCrossing(p, edge.P1, edge.P2, ref crossing);
            }
            else if (edge is ConicEdge conicEdge)
            {
                TestLineCrossing(p, conicEdge.P1, conicEdge.Control1, ref crossing);
                TestLineCrossing(p, conicEdge.Control1, conicEdge.P2, ref crossing);
            }
            else if (edge is CubicEdge cubicEdge)
            {
                TestLineCrossing(p, cubicEdge.P1, cubicEdge.Control1, ref crossing);
                TestLineCrossing(p, cubicEdge.Control1, cubicEdge.Control2, ref crossing);
                TestLineCrossing(p, cubicEdge.Control2, cubicEdge.P2, ref crossing);
            }
        }

        private List<Contour>[] DetermineInsides(List<Contour> outsideContours, List<Contour> insideContours)
        {
            var insideContourLists = new List<Contour>[outsideContours.Count];

            for (int i = 0; i < outsideContours.Count; i++)
                insideContourLists[i] = new List<Contour>();

            var crossings = new CrossingInfo[outsideContours.Count];

            foreach (var contour in insideContours)
            {
                Point rightP = FindRightmostPoint(contour);

                Array.Clear(crossings, 0, crossings.Length);

                for (int j = 0; j < outsideContours.Count; j++)
                {
                    var outerContour = outsideContours[j];
                    var p = outerContour.FirstPoint;

                    do
                    {
                        TestCrossing(rightP, p.OutgoingEdge, ref crossings[j]);

                        p = p.OutgoingEdge.P2;
                    }
                    while (p != outerContour.FirstPoint);
                }

                int minX = int.MaxValue;
                int minContour = -1;

                for (int j = 0; j < crossings.Length; j++)
                {
                    if ((crossings[j].Number & 1) == 1
                        && crossings[j].Distance < minX)
                    {
                        minX = crossings[j].Distance;
                        minContour = j;
                    }
                }

                Debug.Assert(minContour >= 0);

                insideContourLists[minContour].Add(contour);
            }

            return insideContourLists;
        }

        private Shape[] PackageShapes(List<Contour> outsideContours, List<Contour>[] insideContourLists)
        {
            var shapes = new Shape[outsideContours.Count];

            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = new Shape(outsideContours[i], insideContourLists[i].ToArray());
            }

            return shapes;
        }

        public Shape[] CalculateShapes()
        {
            var (outsideContours, insideContours) = PartitionContours();
            var insideContourLists = DetermineInsides(outsideContours, insideContours);

            return PackageShapes(outsideContours, insideContourLists);
        }
    }
}
