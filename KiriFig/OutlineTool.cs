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
        private readonly FaceOrientation _faceOrientation;

        public OutlineTool( IReadOnlyList<Contour> contours, FaceOrientation faceOrientation )
        {
            _contours = contours;
            _faceOrientation = faceOrientation;
        }

        private static Orientation GetOrientation( Contour contour )
        {
            // Use the method described here to determine the orientation of a contour:
            // https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order

            // It involves calculating the area of the polygon (integration).
            // It's also the method used by Freetype 2.

            // For the purposes of determining orientation, it's enough to approximate conic
            // and cubic curves with the line segments that connect their control points.

            // Only use the TrueType convention of clockwise means outside, counter-clocwise means inside.
            // Assume that contours don't intersect themselves.

            int prevX = contour.FirstPoint.X;
            int prevY = contour.FirstPoint.Y;
            Point p = contour.FirstPoint;
            long area = 0;

            while ( true )
            {
                var edge = p.OutgoingEdge;

                switch ( edge.Type )
                {
                    case EdgeType.Line:
                        area += (edge.P2.Y - prevY) * (edge.P2.X + prevX);
                        break;

                    case EdgeType.Conic:
                        {
                            var conicEdge = (ConicEdge) edge;
                            area += (conicEdge.C1.Y - prevY) * (conicEdge.C1.X + prevX);
                            area += (conicEdge.P2.Y - conicEdge.C1.Y) * (conicEdge.P2.X + conicEdge.C1.X);
                        }
                        break;

                    case EdgeType.Cubic:
                        {
                            var conicEdge = (CubicEdge) edge;
                            area += (conicEdge.C1.Y - prevY) * (conicEdge.C1.X + prevX);
                            area += (conicEdge.C2.Y - conicEdge.C1.Y) * (conicEdge.C2.X + conicEdge.C1.X);
                            area += (conicEdge.P2.Y - conicEdge.C2.Y) * (conicEdge.P2.X + conicEdge.C2.X);
                        }
                        break;
                }

                prevX = edge.P2.X;
                prevY = edge.P2.Y;

                p = p.OutgoingEdge.P2;

                if ( p == contour.FirstPoint )
                    break;
            }

            if ( area > 0 )
                return Orientation.Inside;
            else if ( area < 0 )
                return Orientation.Outside;
            else
                return Orientation.Unknown;
        }

        private static Point FindRightmostPoint( Contour contour )
        {
            Point p = contour.FirstPoint;
            Point rightP = new Point(0, 0);

            while ( true )
            {
                if ( p.X > rightP.X )
                    rightP = p;

                p = p.OutgoingEdge.P2;

                if ( p == contour.FirstPoint )
                    break;
            }

            return rightP;
        }

        private (List<Contour> outsideContours, List<Contour> insideContours) PartitionContours()
        {
            var outsideContours = new List<Contour>();
            var insideContours = new List<Contour>();

            foreach ( var contour in _contours )
            {
                Orientation orientation = GetOrientation(contour);

                if ( _faceOrientation != FaceOrientation.ClockwiseOut )
                {
                    if ( orientation == Orientation.Outside )
                        orientation = Orientation.Inside;
                    else if ( orientation == Orientation.Inside )
                        orientation = Orientation.Outside;
                }

                switch ( orientation )
                {
                    case Orientation.Inside:
                        insideContours.Add( contour );
                        break;

                    case Orientation.Outside:
                        outsideContours.Add( contour );
                        break;

                    default:
                        Debug.Fail( "" );
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

        private void TestLineCrossing( Point p, PointD p1, PointD p2, ref CrossingInfo crossing )
        {
            if ( (p1.Y <= p.Y && p2.Y > p.Y)
                || (p1.Y > p.Y && p2.Y <= p.Y) )
            {
                float proportion = (float) ((p.Y - p1.Y) / (p2.Y - p1.Y));
                float intersectX = (float) (p1.X + proportion * (p2.X - p1.X));

                if ( p.X < intersectX )
                {
                    crossing.Number++;

                    if ( intersectX < crossing.Distance )
                        crossing.Distance = (int) intersectX;
                }
            }
        }

        private void TestCrossing( Point p, Edge edge, ref CrossingInfo crossing )
        {
            PointD prevPoint = edge.P1.ToPointD();

            foreach ( var pd in edge.Flatten() )
            {
                TestLineCrossing( p, prevPoint, pd, ref crossing );
                prevPoint = pd;
            }
        }

        // Determine which inside contours go with which outside contours.
        // For each inside contour, take its right-most point. Trace a ray to the right.
        // Find the nearest edge of an outside contour that crosses this ray an odd number of times.

        // The version of the even-odd rule used here stipulates that an odd number of crossings
        // means that a point is inside a shape. The closest of these edges belongs to the shape
        // that directly encloses the point.

        private List<Contour>[] DetermineInsides( List<Contour> outsideContours, List<Contour> insideContours )
        {
            var insideContourLists = new List<Contour>[outsideContours.Count];

            for ( int i = 0; i < outsideContours.Count; i++ )
                insideContourLists[i] = new List<Contour>();

            var crossings = new CrossingInfo[outsideContours.Count];

            foreach ( var contour in insideContours )
            {
                Point rightP = FindRightmostPoint(contour);

                Array.Clear( crossings, 0, crossings.Length );

                for ( int j = 0; j < outsideContours.Count; j++ )
                {
                    var outerContour = outsideContours[j];
                    var p = outerContour.FirstPoint;

                    crossings[j].Distance = int.MaxValue;

                    do
                    {
                        TestCrossing( rightP, p.OutgoingEdge, ref crossings[j] );

                        p = p.OutgoingEdge.P2;
                    }
                    while ( p != outerContour.FirstPoint );
                }

                int minX = int.MaxValue;
                int minContour = -1;

                for ( int j = 0; j < crossings.Length; j++ )
                {
                    if ( (crossings[j].Number & 1) == 1
                        && crossings[j].Distance < minX )
                    {
                        minX = crossings[j].Distance;
                        minContour = j;
                    }
                }

                if ( minContour < 0 )
                    return null;

                insideContourLists[minContour].Add( contour );
            }

            return insideContourLists;
        }

        private Shape[] PackageShapes( List<Contour> outsideContours, List<Contour>[] insideContourLists )
        {
            var shapes = new Shape[outsideContours.Count];

            for ( int i = 0; i < shapes.Length; i++ )
            {
                shapes[i] = new Shape( outsideContours[i], insideContourLists[i].ToArray() );
            }

            return shapes;
        }

        public Shape[] CalculateShapes()
        {
            var (outsideContours, insideContours) = PartitionContours();
            var insideContourLists = DetermineInsides(outsideContours, insideContours);

            if ( insideContourLists == null )
            {
                // These contours don't conform to the expected winding (face orientation).
                // Swap inside and outside contours, and try again.

                var temp = insideContours;
                insideContours = outsideContours;
                outsideContours = temp;

                insideContourLists = DetermineInsides( outsideContours, insideContours );

                Debug.Assert( insideContourLists != null );
            }

            return PackageShapes( outsideContours, insideContourLists );
        }
    }
}
