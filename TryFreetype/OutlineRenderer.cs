using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public class OutlineRenderer : FigureWalkerBase
    {
        private readonly Bitmap _bitmap;

        private double _x, _y;
        private Color _color = Color.Red;

        public Bitmap Bitmap => _bitmap;

        public OutlineRenderer(Figure figure) :
            base(figure)
        {
            int width = figure.Width;
            int height = figure.Height;

            _bitmap = new Bitmap(width, height);

            // Transformation:
            //g = Graphics.FromImage(bitmap);
            //g.ScaleTransform(1, -1);
            //g.TranslateTransform(
            //    (float) -figure.OffsetX,
            //    -(height - 1) - (float) figure.OffsetY);
        }

        protected override void OnMoveTo(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            _x = to.X;
            _y = to.Y;
        }

        protected override void OnLineTo(Edge edge)
        {
            var to = edge.P2;
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            DrawLine(to);
            _x = to.X;
            _y = to.Y;
        }

        protected override void OnConicTo(Edge edge)
        {
            var control = ((ConicEdge) edge).Control1;
            var to = edge.P2;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            DrawConic(control, to);
            _x = to.X;
            _y = to.Y;
        }

        protected override void OnCubicTo(Edge edge)
        {
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            DrawCubic(control1, control2, to);
            _x = to.X;
            _y = to.Y;
        }

        private static double GetLength(Point p1, Point p2)
        {
            return p1.ToValuePoint().GetDistance(p2.ToValuePoint());
        }

        private static ValuePoint CalcConic(double t, Point p0, Point p1, Point p2)
        {
            var result = new ValuePoint();

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

        private static ValuePoint CalcCubic(double t, Point p0, Point p1, Point p2, Point p3)
        {
            var result = new ValuePoint();

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

        private void DrawLine(Point to)
        {
            int x0 = RoundAndClampX(TransformX(_x));
            int y0 = RoundAndClampY(TransformY(_y));
            int x1 = RoundAndClampX(TransformX(to.X));
            int y1 = RoundAndClampY(TransformY(to.Y));

            DrawLine(x0, y0, x1, y1);
        }

        private void DrawLine(int x0, int y0, int x1, int y1)
        {
            // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = dx + dy;

            while (true)
            {
                _bitmap.SetPixel(x0, y0, _color);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;

                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private int RoundAndClampX(double x)
        {
            int iX = (int) Math.Round(x);
            if (iX >= Figure.Width)
                iX = Figure.Width - 1;
            else if (iX < 0)
                iX = 0;
            return iX;
        }

        private int RoundAndClampY(double y)
        {
            int iY = (int) Math.Round(y);
            if (iY >= Figure.Height)
                iY = Figure.Height - 1;
            else if (iY < 0)
                iY = 0;
            return iY;
        }

        private double TransformX(double x)
        {
            return x - Figure.OffsetX;
        }

        private double TransformY(double y)
        {
            return Figure.Height - y - 1 + Figure.OffsetY;
        }

        private Point TransformPoint(Point point)
        {
            return new Point(
                TransformX(point.X),
                TransformY(point.Y));
        }

        private void DrawConic(Point control, Point to)
        {
            Point tFrom = new Point(
                TransformX(_x),
                TransformY(_y));
            Point tControl = TransformPoint(control);
            Point tTo = TransformPoint(to);

            double length =
                GetLength(tFrom, tControl)
                + GetLength(tControl, tTo);

            double dt = 1.0 / length;
            ValuePoint p;
            int x;
            int y;
            int prevX = RoundAndClampX(tFrom.X);
            int prevY = RoundAndClampY(tFrom.Y);

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = CalcConic(t, tFrom, tControl, tTo);

                x = RoundAndClampX(p.X);
                y = RoundAndClampY(p.Y);

                if (((x - prevX) <= 1 && (x - prevX) >= -1)
                    && ((y - prevY) <= 1 && (y - prevY) >= -1))
                {
                    _bitmap.SetPixel(x, y, _color);
                }
                else
                {
                    DrawLine(prevX, prevY, x, y);
                }

                prevX = x;
                prevY = y;
            }

            p = CalcConic(1.0, tFrom, tControl, tTo);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            if (((x - prevX) <= 1 && (x - prevX) >= -1)
                && ((y - prevY) <= 1 && (y - prevY) >= -1))
            {
                _bitmap.SetPixel(x, y, _color);
            }
            else
            {
                DrawLine(prevX, prevY, x, y);
            }

            _bitmap.SetPixel(x, y, _color);

            // TODO: Find a way to get rid of bunches of pixels.
            //       If the current (a) and last two pixels (b, c) fit in a 2x2 square,
            //       then we can get rid of the previous one (b).
        }

        private void DrawCubic(Point control1, Point control2, Point to)
        {
            Point tFrom = new Point(
                TransformX(_x),
                TransformY(_y));
            Point tControl1 = TransformPoint(control1);
            Point tControl2 = TransformPoint(control2);
            Point tTo = TransformPoint(to);

            double length =
                GetLength(tFrom, tControl1)
                + GetLength(tControl1, tControl2)
                + GetLength(tControl2, tTo);

            double dt = 1.0 / length;
            ValuePoint p;
            int x;
            int y;

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = CalcCubic(t, tFrom, tControl1, tControl2, tTo);

                x = RoundAndClampX(p.X);
                y = RoundAndClampY(p.Y);

                _bitmap.SetPixel(x, y, _color);
            }

            p = CalcCubic(1.0, tFrom, tControl1, tControl2, tTo);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            _bitmap.SetPixel(x, y, _color);
        }
    }
}
