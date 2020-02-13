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

        public Edge(EdgeType type, bool unbreakable = false)
        {
            Type = type;
            Unbreakable = unbreakable;
        }

        internal abstract SplitResult Split(Point point);
        public abstract object Clone();
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

            //Console.WriteLine("Checking {0}, {1} (dP1={2}, dP2={3}, dP1P2={4}", p1, p2, distP1, distP2, distP1P2);

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

            ValuePoint midPoint = new ValuePoint();

            midPoint.X = (p2.X + p1.X) / 2;
            midPoint.Y = (p2.Y + p1.Y) / 2;

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

        internal override SplitResult Split(Point point)
        {
            throw new NotImplementedException();
        }
    }

    public class PointGroup
    {
        public bool IsFixed;
        public List<Point> Points { get; } = new List<Point>();

        public Point MakePoint()
        {
            Point p = new Point(Points[0].X, Points[0].Y);
            p.Group = this;

            return p;
        }
    }

    public class Point
    {
        public double X;
        public double Y;
        public PointGroup Group;
        public bool IsFixed;
        public Edge OutgoingEdge;
        public Edge IncomingEdge;
        public Edge OriginalOutgoingEdge;
        public Edge OriginalIncomingEdge;
        public Contour Contour;

        public Point()
        {
        }

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

    internal struct ValuePoint
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
