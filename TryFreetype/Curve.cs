using System;

namespace TryFreetype
{
    public class Curve
    {
        private delegate PointD CalcCurveDelegate(double t);

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

        public static PointD CalcConic(double t, PointD p0, PointD p1, PointD p2)
        {
            var result = new PointD();

            result.X =
                p0.X * Math.Pow((1 - t), 2) +
                p1.X * 2 * t * (1 - t) +
                p2.X * Math.Pow(t, 2)
                ;

            result.Y =
                p0.Y * Math.Pow((1 - t), 2) +
                p1.Y * 2 * t * (1 - t) +
                p2.Y * Math.Pow(t, 2)
                ;

            return result;
        }

        public static PointD CalcCubic(double t, PointD p0, PointD p1, PointD p2, PointD p3)
        {
            var result = new PointD();

            result.X =
                p0.X * Math.Pow((1 - t), 3) +
                p1.X * 3 * t * Math.Pow((1 - t), 2) +
                p2.X * 3 * Math.Pow(t, 2) * (1 - t) +
                p3.X * Math.Pow(t, 3)
                ;

            result.Y =
                p0.Y * Math.Pow((1 - t), 3) +
                p1.Y * 3 * t * Math.Pow((1 - t), 2) +
                p2.Y * 3 * Math.Pow(t, 2) * (1 - t) +
                p3.Y * Math.Pow(t, 3)
                ;

            return result;
        }

        public static PointD GetProjectedPoint(PointD point, PointD c0, PointD c1, PointD c2)
        {
            PointD CalcCurve(double t) => CalcConic(t, c0, c1, c2);

            return GetProjectedPoint(point, c0, CalcCurve);
        }

        public static PointD GetProjectedPoint(PointD point, PointD c0, PointD c1, PointD c2, PointD c3)
        {
            PointD CalcCurve(double t) => Curve.CalcCubic(t, c0, c1, c2, c3);

            return GetProjectedPoint(point, c0, CalcCurve);
        }

        private static PointD GetProjectedPoint(PointD point, PointD c0, CalcCurveDelegate calcCurve)
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
                PointD p = calcCurve(t);
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

            PointD pBefore = calcCurve(tBefore);
            PointD pAfter = calcCurve(tAfter);

            return FindNearestPoint(tBefore, tAfter, pBefore, pAfter, point, calcCurve);
        }

        private static PointD FindNearestPoint(
            float t1, float t2, PointD p1, PointD p2, PointD p,
            CalcCurveDelegate calcCurve)
        {
            double d1 = p1.GetDistance(p);
            double d2 = p2.GetDistance(p);
            double d12 = p1.GetDistance(p2);

            if (d12 <= 1.0)
            {
                if (d1 < d2)
                    return p1;
                else
                    return p2;
            }

            float midT = (t1 + t2) / 2;
            PointD midP = calcCurve(midT);

            if (d1 < d2)
                return FindNearestPoint(t1, midT, p1, midP, p, calcCurve);
            else
                return FindNearestPoint(midT, t2, midP, p2, p, calcCurve);
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
