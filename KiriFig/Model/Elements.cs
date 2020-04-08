﻿using System;
using System.Collections.Generic;

namespace TryFreetype.Model
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
        public Point P1;
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
            var valueNearestPoint = FindNearestPoint(point, P1.ToValuePoint(), P2.ToValuePoint());

            var nearestPoint = new Point(valueNearestPoint);
            var edgeBefore = new LineEdge(P1, nearestPoint);
            var edgeAfter = new LineEdge(nearestPoint, P2);

            return new SplitResult(nearestPoint, edgeBefore, edgeAfter);
        }

        private ValuePoint FindNearestPoint(Point point, ValuePoint p1, ValuePoint p2)
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
        public Point Control1;

        public ConicEdge(Point p1, Point c1, Point p2) :
            base(EdgeType.Conic)
        {
            P1 = p1;
            Control1 = c1;
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
                Left = Math.Min(P1.X, Math.Min(P2.X, Control1.X)),
                Right = Math.Max(P1.X, Math.Max(P2.X, Control1.X)),
                Top = Math.Max(P1.Y, Math.Max(P2.Y, Control1.Y)),
                Bottom = Math.Min(P1.Y, Math.Min(P2.Y, Control1.Y)),
            };
            return bbox;
        }

        public override PointD? GetProjectedPoint(int x, int y)
        {
            var (t, p) = GetProjectedPointAndT(x, y);
            return p;
        }

        internal (double, PointD) GetProjectedPointAndT(int x, int y)
        {
            var curve = new Curve(
                P1.ToPointD(),
                Control1.ToPointD(),
                P2.ToPointD());

            return curve.GetProjectedPoint(new PointD(x, y));
        }

        internal override SplitResult Split(Point point)
        {
            var (t, midP) = GetProjectedPointAndT(point.X, point.Y);

            Point b0 = new Point(
                (int) ((1 - t) * P1.X + t * Control1.X),
                (int) ((1 - t) * P1.Y + t * Control1.Y)
            );

            Point b1 = new Point(
                (int) ((1 - t) * Control1.X + t * P2.X),
                (int) ((1 - t) * Control1.Y + t * P2.Y)
            );

            Point midPoint = Point.Trunc(midP);

            ConicEdge edgeBefore = new ConicEdge(P1, b0, midPoint);
            ConicEdge edgeAfter = new ConicEdge(midPoint, b1, P2);

            return new SplitResult(midPoint, edgeBefore, edgeAfter);
        }
    }

    public class CubicEdge : Edge
    {
        public Point Control1;
        public Point Control2;

        public CubicEdge(Point p1, Point c1, Point c2, Point p2) :
            base(EdgeType.Cubic)
        {
            P1 = p1;
            Control1 = c1;
            Control2 = c2;
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
                Left = Math.Min(P1.X, Math.Min(P2.X, Math.Min(Control1.X, Control2.X))),
                Right = Math.Max(P1.X, Math.Max(P2.X, Math.Max(Control1.X, Control2.X))),
                Top = Math.Max(P1.Y, Math.Max(P2.Y, Math.Max(Control1.Y, Control2.Y))),
                Bottom = Math.Min(P1.Y, Math.Min(P2.Y, Math.Min(Control1.Y, Control2.Y))),
            };
            return bbox;
        }

        public override PointD? GetProjectedPoint(int x, int y)
        {
            var (t, p) = GetProjectedPointAndT(x, y);
            return p;
        }

        internal (double, PointD) GetProjectedPointAndT(int x, int y)
        {
            var curve = new Curve(
                P1.ToPointD(),
                Control1.ToPointD(),
                Control2.ToPointD(),
                P2.ToPointD());

            return curve.GetProjectedPoint(new PointD(x, y));
        }

        internal override SplitResult Split(Point point)
        {
            var (t, midP) = GetProjectedPointAndT(point.X, point.Y);

            PointD b0d = new PointD(
                (1 - t) * P1.X + t * Control1.X,
                (1 - t) * P1.Y + t * Control1.Y
            );

            PointD b1d = new PointD(
                (1 - t) * Control1.X + t * Control2.X,
                (1 - t) * Control1.Y + t * Control2.Y
            );

            PointD b2d = new PointD(
                (1 - t) * Control2.X + t * P2.X,
                (1 - t) * Control2.Y + t * P2.Y
            );


            Point b3 = new Point(
                (int) ((1 - t) * b0d.X + t * b1d.X),
                (int) ((1 - t) * b0d.Y + t * b1d.Y)
            );

            Point b4 = new Point(
                (int) ((1 - t) * b1d.X + t * b2d.X),
                (int) ((1 - t) * b1d.Y + t * b2d.Y)
            );

            Point midPoint = Point.Trunc(midP);
            Point b0 = Point.Trunc(b0d);
            Point b2 = Point.Trunc(b2d);

            CubicEdge edgeBefore = new CubicEdge(P1, b0, b3, midPoint);
            CubicEdge edgeAfter = new CubicEdge(midPoint, b2, b4, P2);

            return new SplitResult(midPoint, edgeBefore, edgeAfter);
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

        internal double GetDistance(ValuePoint otherPoint)
        {
            int w = otherPoint.X - X;
            int h = otherPoint.Y - Y;
            return Math.Sqrt(w * w + h * h);
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