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

    public struct BBox
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        public bool IsPointInside(double x, double y)
        {
            return (x >= Left) && (x <= Right)
                && (y <= Top) && (y >= Bottom);
        }
    }

    public class LineEdge : Edge
    {
        public LineEdge(bool unbreakable = false) :
            base(EdgeType.Line, unbreakable)
        {
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
            var edgeBefore = new LineEdge { P1 = P1, P2 = nearestPoint };
            var edgeAfter = new LineEdge { P1 = nearestPoint, P2 = P2 };

            nearestPoint.IncomingEdge = edgeBefore;
            nearestPoint.OutgoingEdge = edgeAfter;

            return new SplitResult(nearestPoint, edgeBefore, edgeAfter);
        }

        private ValuePoint FindNearestPoint(Point point, ValuePoint p1, ValuePoint p2)
        {
            ValuePoint valuePoint = point.ToValuePoint();
            double distP1 = Math.Abs(valuePoint.GetDistance(p1));
            double distP2 = Math.Abs(valuePoint.GetDistance(p2));
            double distP1P2 = Math.Abs(p1.GetDistance(p2));

            if (distP1P2 <= 1.0)
            {
                if (distP1 <= distP2)
                {
                    return p1;
                }
                else
                {
                    return p2;
                }
            }

            ValuePoint midPoint = new ValuePoint
            {
                X = (p2.X + p1.X) / 2,
                Y = (p2.Y + p1.Y) / 2
            };

            if (distP1 <= distP2)
            {
                return FindNearestPoint(point, p1, midPoint);
            }
            else
            {
                return FindNearestPoint(point, midPoint, p2);
            }
        }
    }

    public class ConicEdge : Edge
    {
        public Point Control1;

        public ConicEdge() :
            base(EdgeType.Conic)
        {
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

        public CubicEdge() :
            base(EdgeType.Cubic)
        {
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
        public double X { get; }
        public double Y { get; }

        public PointGroup Group;
        public Contour Contour;
        public Edge OutgoingEdge;
        public Edge IncomingEdge;
        public Edge OriginalOutgoingEdge;
        public Edge OriginalIncomingEdge;

        public Point(double x, double y)
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
        public double X;
        public double Y;

        internal double GetDistance(ValuePoint otherPoint)
        {
            double w = otherPoint.X - X;
            double h = otherPoint.Y - Y;
            return Math.Sqrt(w * w + h * h);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
