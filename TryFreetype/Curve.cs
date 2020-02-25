using System;

namespace TryFreetype
{
    public class Curve
    {
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
    }
}
