﻿/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;

namespace KiriFig.Model
{
    public class Cut
    {
        public LineEdge PairedEdge1 { get; }
        public LineEdge PairedEdge2 { get; }

        public Cut(LineEdge pairedEdge1, LineEdge pairedEdge2)
        {
            PairedEdge1 = pairedEdge1;
            PairedEdge2 = pairedEdge2;
        }
    }

    public class Contour
    {
        public Point FirstPoint;
        public Shape Shape;
    }

    internal struct SplitResult
    {
        public Point nearestPoint;
        public Edge edgeBefore;
        public Edge edgeAfter;

        public SplitResult(Point nearestPoint, Edge edgeBefore, Edge edgeAfter)
        {
            this.nearestPoint = nearestPoint;
            this.edgeBefore = edgeBefore;
            this.edgeAfter = edgeAfter;
        }
    }

    public enum EdgeType
    {
        Line,
        Conic,
        Cubic
    }

    public struct BBox
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public bool Contains(int x, int y)
        {
            return (x >= Left) && (x <= Right)
                && (y <= Top) && (y >= Bottom);
        }

        public void Inflate(int width, int height)
        {
            Left -= width;
            Right += width;
            Top += height;
            Bottom -= height;
        }
    }

    public abstract class Edge : ICloneable
    {
        public EdgeType Type { get; }
        public bool Unbreakable { get; }

        // P1 is the first control point, C0.

        public Point P1;

        // P2 is the last control point.
        // This is C1 for lines, C2 for conics, and C3 for cubics.

        public Point P2;

        protected Edge(EdgeType type, bool unbreakable = false)
        {
            Type = type;
            Unbreakable = unbreakable;
        }

        public abstract BBox GetBBox();
        internal abstract SplitResult Split(Point point);
        public abstract PointD? GetProjectedPoint(int x, int y);
        public abstract object Clone();
    }

    public class LineEdge : Edge
    {
        public LineEdge(Point p1, Point p2, bool unbreakable = false) :
            base(EdgeType.Line, unbreakable)
        {
            P1 = p1;
            P2 = p2;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override BBox GetBBox()
        {
            BBox bbox = new BBox
            {
                Left = Math.Min(P1.X, P2.X),
                Right = Math.Max(P1.X, P2.X),
                Top = Math.Max(P1.Y, P2.Y),
                Bottom = Math.Min(P1.Y, P2.Y)
            };

            return bbox;
        }

        internal override SplitResult Split(Point point)
        {
            var valueNearestPoint = FindNearestPoint(point);

            var nearestPoint = new Point(valueNearestPoint);
            var edgeBefore = new LineEdge(P1, nearestPoint);
            var edgeAfter = new LineEdge(nearestPoint, P2);

            return new SplitResult(nearestPoint, edgeBefore, edgeAfter);
        }

        private ValuePoint FindNearestPoint(Point point)
        {
            PointD? optNearestPoint = GetProjectedPoint(point.X, point.Y);

            if (optNearestPoint.HasValue)
            {
                return new ValuePoint((int) optNearestPoint.Value.X, (int) optNearestPoint.Value.Y);
            }

            return P1.ToValuePoint();
        }

        public override PointD? GetProjectedPoint(int x, int y)
        {
            int dX = x - P1.X;
            int dY = y - P1.Y;
            int dEdgeX = P2.X - P1.X;
            int dEdgeY = P2.Y - P1.Y;

            int dotProduct = dX * dEdgeX + dY * dEdgeY;
            int lengthSquared = dEdgeX * dEdgeX + dEdgeY * dEdgeY;

            if (lengthSquared != 0)
            {
                float t = dotProduct / (float) lengthSquared;

                if (t >= 0 && t <= 1)
                {
                    return new PointD(
                        P1.X + t * dEdgeX,
                        P1.Y + t * dEdgeY);
                }
            }

            return null;
        }
    }

    public class ConicEdge : Edge
    {
        public Point C1;

        public ConicEdge(Point p1, Point c1, Point p2) :
            base(EdgeType.Conic)
        {
            P1 = p1;
            C1 = c1;
            P2 = p2;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override BBox GetBBox()
        {
            BBox bbox = new BBox
            {
                Left = Math.Min(P1.X, Math.Min(P2.X, C1.X)),
                Right = Math.Max(P1.X, Math.Max(P2.X, C1.X)),
                Top = Math.Max(P1.Y, Math.Max(P2.Y, C1.Y)),
                Bottom = Math.Min(P1.Y, Math.Min(P2.Y, C1.Y)),
            };
            return bbox;
        }

        public override PointD? GetProjectedPoint(int x, int y)
        {
            var (_, p) = GetProjectedPointAndT(x, y);
            return p;
        }

        internal (double, PointD) GetProjectedPointAndT(int x, int y)
        {
            var curve = new Curve(
                P1.ToPointD(),
                C1.ToPointD(),
                P2.ToPointD());

            return curve.GetProjectedPoint(new PointD(x, y));
        }

        internal override SplitResult Split(Point point)
        {
            var curve = new Curve(
                P1.ToPointD(),
                C1.ToPointD(),
                P2.ToPointD());

            var (t, midP) = curve.GetProjectedPoint(new PointD(point.X, point.Y));

            var (curve1, curve2) = curve.Split(t, midP);

            Point a0 = Point.Trunc(curve1.C1);
            Point b0 = Point.Trunc(curve2.C1);
            Point midPoint = Point.Trunc(curve2.C0);

            ConicEdge edge1 = new ConicEdge(P1, a0, midPoint);
            ConicEdge edge2 = new ConicEdge(midPoint, b0, P2);

            return new SplitResult(midPoint, edge1, edge2);
        }
    }

    public class CubicEdge : Edge
    {
        public Point C1;
        public Point C2;

        public CubicEdge(Point p1, Point c1, Point c2, Point p2) :
            base(EdgeType.Cubic)
        {
            P1 = p1;
            C1 = c1;
            C2 = c2;
            P2 = p2;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override BBox GetBBox()
        {
            BBox bbox = new BBox
            {
                Left = Math.Min(P1.X, Math.Min(P2.X, Math.Min(C1.X, C2.X))),
                Right = Math.Max(P1.X, Math.Max(P2.X, Math.Max(C1.X, C2.X))),
                Top = Math.Max(P1.Y, Math.Max(P2.Y, Math.Max(C1.Y, C2.Y))),
                Bottom = Math.Min(P1.Y, Math.Min(P2.Y, Math.Min(C1.Y, C2.Y))),
            };
            return bbox;
        }

        public override PointD? GetProjectedPoint(int x, int y)
        {
            var (_, p) = GetProjectedPointAndT(x, y);
            return p;
        }

        internal (double, PointD) GetProjectedPointAndT(int x, int y)
        {
            var curve = new Curve(
                P1.ToPointD(),
                C1.ToPointD(),
                C2.ToPointD(),
                P2.ToPointD());

            return curve.GetProjectedPoint(new PointD(x, y));
        }

        internal override SplitResult Split(Point point)
        {
            var curve = new Curve(
                P1.ToPointD(),
                C1.ToPointD(),
                C2.ToPointD(),
                P2.ToPointD());

            var (t, midP) = curve.GetProjectedPoint(new PointD(point.X, point.Y));

            var (curve1, curve2) = curve.Split(t, midP);

            Point a0 = Point.Trunc(curve1.C1);
            Point a1 = Point.Trunc(curve1.C2);
            Point b0 = Point.Trunc(curve2.C1);
            Point b1 = Point.Trunc(curve2.C2);
            Point midPoint = Point.Trunc(curve2.C0);

            CubicEdge edge1 = new CubicEdge(P1, a0, a1, midPoint);
            CubicEdge edge2 = new CubicEdge(midPoint, b0, b1, P2);

            return new SplitResult(midPoint, edge1, edge2);
        }
    }

    public class PointGroup
    {
        public bool IsFixed { get; }
        public Edge OriginalIncomingEdge { get; set; }
        public Edge OriginalOutgoingEdge { get; set; }
        public List<Point> Points { get; } = new List<Point>();

        public PointGroup()
        {
        }

        public PointGroup(bool isFixed)
        {
            IsFixed = isFixed;
        }

        public Point MakePoint()
        {
            Point p = new Point(Points[0].X, Points[0].Y);
            p.Group = this;

            return p;
        }
    }

    public class Point
    {
        public int X { get; }
        public int Y { get; }

        public PointGroup Group;
        public Contour Contour;
        public Edge OutgoingEdge;
        public Edge IncomingEdge;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal Point(ValuePoint valuePoint)
        {
            X = valuePoint.X;
            Y = valuePoint.Y;
        }

        public static Point Trunc(PointD pointD)
        {
            return new Point((int) pointD.X, (int) pointD.Y);
        }

        internal ValuePoint ToValuePoint()
        {
            return new ValuePoint { X = X, Y = Y };
        }

        public PointD ToPointD()
        {
            return new PointD(X, Y);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }

    public struct ValuePoint
    {
        public int X;
        public int Y;

        public ValuePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }

    public class Shape
    {
        public bool Enabled;

        // The first element is the outside contour.
        public List<Contour> Contours { get; } = new List<Contour>();

        public Shape()
        {
        }

        public Shape(Contour outside, IReadOnlyCollection<Contour> insides)
        {
            Contours.Add(outside);
            Contours.AddRange(insides);
        }
    }
}
