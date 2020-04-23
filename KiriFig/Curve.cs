/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;

namespace KiriFig
{
    public struct Curve
    {
        private const double Epsilon = .00001;

        private enum CurveType
        {
            Conic,
            Cubic,
        }

        private readonly CurveType _curveType;
        private PointD c0;
        private PointD c1;
        private PointD c2;
        private PointD c3;

        public PointD C0 => c0;
        public PointD C1 => c1;
        public PointD C2 => c2;
        public PointD C3 => c3;

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
            // Use the length of the line segments between each control point as an estimate of
            // the length of the curve.

            double length =
                GetLineLength(p0, p1)
                + GetLineLength(p1, p2);

            double dt = 1.0 / length;
            return dt;
        }

        public static double CalcCubicDeltaT(PointD p0, PointD p1, PointD p2, PointD p3)
        {
            // Use the length of the line segments between each control point as an estimate of
            // the length of the curve.

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
            // First use a rough pass to find the closest point among N samples along the curve
            // spaced at 1/(N-1) between each.

            const int Segments = 5;
            const float DeltaT = 1f / Segments;

            float t = 0;
            float minT = 0;
            double minD = c0.GetDistance(point);

            for (int i = 1; i < Segments; i++)
            {
                t += DeltaT;
                PointD p = CalcCurve(t);
                double d = p.GetDistance(point);

                if (d < minD)
                {
                    minD = d;
                    minT = t;
                }
            }

            // Now find the nearest point in detail between the approximate point found and
            // the next nearest approximate point.

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
            // Use a binary search to find p. End when the interval is 1 or less.

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

        public (Curve, Curve) Split(double t, PointD midP)
        {
            if (_curveType == CurveType.Conic)
                return SplitConic(t, midP);
            else
                return SplitCubic(t, midP);
        }

        private (Curve, Curve) SplitConic(double t, PointD midP)
        {
            // Use de Casteljau's algorithm.

            PointD b0 = new PointD(
                (int) ((1 - t) * c0.X + t * c1.X),
                (int) ((1 - t) * c0.Y + t * c1.Y)
            );

            PointD b1 = new PointD(
                (int) ((1 - t) * c1.X + t * c2.X),
                (int) ((1 - t) * c1.Y + t * c2.Y)
            );

            Curve curve1 = new Curve(c0, b0, midP);
            Curve curve2 = new Curve(midP, b1, c2);

            return (curve1, curve2);
        }

        private (Curve, Curve) SplitCubic(double t, PointD midP)
        {
            // Use de Casteljau's algorithm.

            PointD b0 = new PointD(
                (1 - t) * c0.X + t * c1.X,
                (1 - t) * c0.Y + t * c1.Y
            );

            PointD b1 = new PointD(
                (1 - t) * c1.X + t * c2.X,
                (1 - t) * c1.Y + t * c2.Y
            );

            PointD b2 = new PointD(
                (1 - t) * c2.X + t * c3.X,
                (1 - t) * c2.Y + t * c3.Y
            );


            PointD b3 = new PointD(
                (1 - t) * b0.X + t * b1.X,
                (1 - t) * b0.Y + t * b1.Y
            );

            PointD b4 = new PointD(
                (1 - t) * b1.X + t * b2.X,
                (1 - t) * b1.Y + t * b2.Y
            );

            Curve curve1 = new Curve(c0, b0, b3, midP);
            Curve curve2 = new Curve(midP, b2, b4, c3);

            return (curve1, curve2);
        }

        public int SolveConicWithX(int x, double[] roots)
        {
            return SolveConic(C0.X, C1.X, C2.X, x, roots);
        }

        public int SolveConicWithY(int y, double[] roots)
        {
            return SolveConic(C0.Y, C1.Y, C2.Y, y, roots);
        }

        private static int SolveConic(double c0, double c1, double c2, int coordinate, double[] roots)
        {
            var solutions = new Solutions(roots);

            double a = c0 - 2 * c1 + c2;
            double sqrt = Math.Sqrt(coordinate * a + c1 * c1 - c0 * c2);

            solutions.Add((c0 - c1 - sqrt) / a);
            solutions.Add((c0 - c1 + sqrt) / a);

            return solutions.Count;
        }

        public int SolveCubicWithX(int x, double[] roots)
        {
            return SolveCubic(C0.X, C1.X, C2.X, C3.X, x, roots);
        }

        public int SolveCubicWithY(int y, double[] roots)
        {
            return SolveCubic(C0.Y, C1.Y, C2.Y, C3.Y, y, roots);
        }

        private struct Solutions
        {
            public double[] Roots { get; }
            public int Count { get; private set; }

            public Solutions(double[] roots)
            {
                Roots = roots;
                Count = 0;
            }

            public void Add(double root)
            {
                if (root >= 0 && root <= 1)
                {
                    Roots[Count] = root;
                    Count++;
                }
            }
        }

        // Use Cardano's algorithm to solve the cubic component function for t.
        // This is based on explanations in:
        //  https://trans4mind.com/personal_development/mathematics/polynomials/cubicAlgebra.htm
        //  https://pomax.github.io/bezierinfo/#extremities

        private static int SolveCubic(double c0, double c1, double c2, double c3, int coordinate, double[] roots)
        {
            var solutions = new Solutions(roots);

            double a = 3 * c0 - 6 * c1 + 3 * c2;
            double b = -3 * c0 + 3 * c1;
            double c = c0 - coordinate;
            double d = -c0 + 3 * c1 - 3 * c2 + c3;

            double q;

            // Check the coefficients to see if the curve is really of a lower order.

            if (AboutEqual(d, 0))
            {
                if (AboutEqual(a, 0))
                {
                    if (AboutEqual(b, 0))
                        // The curve isn't even linear. There are no solutions.
                        return 0;

                    // The curve is linear. There's at most one solution.
                    solutions.Add(-c / b);
                    return solutions.Count;
                }

                // The curve is quardratic. There are at most two solutions.
                q = Math.Sqrt(b * b - 4 * a * c);
                double _2a = 2 * a;
                solutions.Add((q - b) / _2a);
                solutions.Add((-b - q) / _2a);
                return solutions.Count;
            }

            // A cubic solution is indeed needed.

            a /= d;
            b /= d;
            c /= d;

            double p = (3 * b - a * a) / 3;
            double p3 = p / 3;
            q = (2 * a * a * a - 9 * a * b + 27 * c) / 27;
            double q2 = q / 2;
            double discriminant = q2 * q2 + p3 * p3 * p3;

            // There are three possible real roots.
            if (discriminant < 0)
            {
                double mp3 = -p / 3;
                double r = Math.Sqrt(mp3 * mp3 * mp3);
                double t = -q / (2 * r);
                double cosphi;

                if (t < -1)
                    cosphi = -1;
                else if (t > 1)
                    cosphi = 1;
                else
                    cosphi = t;

                double phi = Math.Acos(cosphi);
                double t1 = 2 * CubeRoot(r);
                solutions.Add(t1 * Math.Cos(phi / 3) - a / 3);
                solutions.Add(t1 * Math.Cos((phi + 2 * Math.PI) / 3) - a / 3);
                solutions.Add(t1 * Math.Cos((phi + 4 * Math.PI) / 3) - a / 3);
                return solutions.Count;
            }

            // There are three possible real roots. But two of them are equal.
            if (discriminant == 0)
            {
                double u = -CubeRoot(q2);
                solutions.Add(2 * u - a / 3);
                solutions.Add(-u - a / 3);
                return solutions.Count;
            }

            // There's one real root and two complex roots.
            double sd = Math.Sqrt(discriminant);
            double u1 = CubeRoot(sd - q2);
            double v1 = CubeRoot(sd + q2);
            solutions.Add(u1 - v1 - a / 3);
            return solutions.Count;
        }

        private static bool AboutEqual(double a, double b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        private static double CubeRoot(double x)
        {
            if (x < 0)
                return -Math.Pow(-x, 1 / 3d);

            return Math.Pow(x, 1 / 3d);
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
