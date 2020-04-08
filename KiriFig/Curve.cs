﻿using System;

namespace TryFreetype
{
    public struct Curve
    {
        private enum CurveType
        {
            Conic,
            Cubic,
        }

        private CurveType _curveType;
        private PointD c0;
        private PointD c1;
        private PointD c2;
        private PointD c3;

        public Curve(PointD c0, PointD c1, PointD c2)
        {
            _curveType = CurveType.Conic;
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = new PointD();
        }

        public Curve(PointD c0, PointD c1, PointD c2, PointD c3)
        {
            _curveType = CurveType.Cubic;
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
        }

        private static double GetLineLength(PointD p1, PointD p2)
        {
            double w = p2.X - p1.X;
            double h = p2.Y - p1.Y;
            return Math.Sqrt(w * w + h * h);
        }

        public static double CalcConicDeltaT(PointD p0, PointD p1, PointD p2)
        {
            double length =
                GetLineLength(p0, p1)
                + GetLineLength(p1, p2);

            double dt = 1.0 / length;
            return dt;
        }

        public static double CalcCubicDeltaT(PointD p0, PointD p1, PointD p2, PointD p3)
        {
            double length =
                GetLineLength(p0, p1)
                + GetLineLength(p1, p2)
                + GetLineLength(p2, p3);

            double dt = 1.0 / length;
            return dt;
        }

        public PointD CalcConic(double t)
        {
            var result = new PointD();

            result.X =
                c0.X * Math.Pow((1 - t), 2) +
                c1.X * 2 * t * (1 - t) +
                c2.X * Math.Pow(t, 2)
                ;

            result.Y =
                c0.Y * Math.Pow((1 - t), 2) +
                c1.Y * 2 * t * (1 - t) +
                c2.Y * Math.Pow(t, 2)
                ;

            return result;
        }

        public PointD CalcCubic(double t)
        {
            var result = new PointD();

            result.X =
                c0.X * Math.Pow((1 - t), 3) +
                c1.X * 3 * t * Math.Pow((1 - t), 2) +
                c2.X * 3 * Math.Pow(t, 2) * (1 - t) +
                c3.X * Math.Pow(t, 3)
                ;

            result.Y =
                c0.Y * Math.Pow((1 - t), 3) +
                c1.Y * 3 * t * Math.Pow((1 - t), 2) +
                c2.Y * 3 * Math.Pow(t, 2) * (1 - t) +
                c3.Y * Math.Pow(t, 3)
                ;

            return result;
        }

        public PointD CalcCurve(double t)
        {
            if (_curveType == CurveType.Conic)
                return CalcConic(t);
            else
                return CalcCubic(t);
        }

        public (double, PointD) GetProjectedPoint(PointD point)
        {
            const int Segments = 5;
            const float DeltaT = 1f / Segments;

            float t = 0;
            float minT = 0;
            var minP = c0;
            double minD = minP.GetDistance(point);

            for (int i = 1; i < Segments; i++)
            {
                t += DeltaT;
                PointD p = CalcCurve(t);
                double d = p.GetDistance(point);

                if (d < minD)
                {
                    minD = d;
                    minP = p;
                    minT = t;
                }
            }

            float tBefore = minT - DeltaT;
            float tAfter = minT + DeltaT;

            if (tBefore < 0)
                tBefore = 0;

            if (tAfter > 1)
                tAfter = 1;

            PointD pBefore = CalcCurve(tBefore);
            PointD pAfter = CalcCurve(tAfter);

            return FindNearestPoint(tBefore, tAfter, pBefore, pAfter, point);
        }

        private (double, PointD) FindNearestPoint(float t1, float t2, PointD p1, PointD p2, PointD p)
        {
            double d1 = p1.GetDistance(p);
            double d2 = p2.GetDistance(p);
            double d12 = p1.GetDistance(p2);

            if (d12 <= 1.0)
            {
                if (d1 < d2)
                    return (t1, p1);
                else
                    return (t2, p2);
            }

            float midT = (t1 + t2) / 2;
            PointD midP = CalcCurve(midT);

            if (d1 < d2)
                return FindNearestPoint(t1, midT, p1, midP, p);
            else
                return FindNearestPoint(midT, t2, midP, p2, p);
        }
    }

    public struct PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double GetDistance(PointD otherPoint)
        {
            double w = otherPoint.X - X;
            double h = otherPoint.Y - Y;
            return Math.Sqrt(w * w + h * h);
        }
    }
}