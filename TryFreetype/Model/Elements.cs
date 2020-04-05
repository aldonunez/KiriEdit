using System;
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

        public bool IsPointInside(int x, int y)
        {
            return (x >= Left) && (x <= Right)
                && (y <= Top) && (y >= Bottom);
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

            nearestPoint.IncomingEdge = edgeBefore;
            nearestPoint.OutgoingEdge = edgeAfter;

            return new SplitResult(nearestPoint, edgeBefore, edgeAfter);
        }

        private ValuePoint FindNearestPoint(Point point, ValuePoint p1, ValuePoint p2)
        {
            throw new NotImplementedException();
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

        internal override SplitResult Split(Point point)
        {
            throw new NotImplementedException();
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

        internal override SplitResult Split(Point point)
        {
            throw new NotImplementedException();
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

        internal ValuePoint ToValuePoint()
        {
            return new ValuePoint { X = X, Y = Y };
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
