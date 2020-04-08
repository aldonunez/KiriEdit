using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
#if DEBUG
    public
#else
    internal
#endif
    class OutlineRenderer
    {
        private const float Divisor = 8f;

        private Figure _figure;
        private FigureWalker _figureWalker;
        private int _x, _y;

        private byte _color;
        private byte[,] _maskBuf;
        private int _maskWidth;
        private int _maskHeight;

        public byte[,] Mask { get => _maskBuf; }
        public int MaskWidth { get => _maskWidth; }
        public int MaskHeight { get => _maskHeight; }

        public OutlineRenderer(Figure figure)
        {
            _figure = figure;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            int width = (int) (figure.Width / Divisor);
            int height = (int) (figure.Height / Divisor);

            _maskBuf = new byte[height, width];
            _maskWidth = width;
            _maskHeight = height;

            // Transformation:
            //g = Graphics.FromImage(bitmap);
            //g.ScaleTransform(1, -1);
            //g.TranslateTransform(
            //    (float) -figure.OffsetX,
            //    -(height - 1) - (float) figure.OffsetY);
        }

        private void MoveTo(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            _x = to.X;
            _y = to.Y;
        }

        private void LineTo(Edge edge)
        {
            var to = edge.P2;
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            DrawLine(to);
            _x = to.X;
            _y = to.Y;
        }

        private void ConicTo(Edge edge)
        {
            var control = ((ConicEdge) edge).Control1;
            var to = edge.P2;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            DrawConic(control, to);
            _x = to.X;
            _y = to.Y;
        }

        private void CubicTo(Edge edge)
        {
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            DrawCubic(control1, control2, to);
            _x = to.X;
            _y = to.Y;
        }

        private void SetPixel(int x, int y)
        {
            _maskBuf[y, x] = _color;
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
                SetPixel(x0, y0);

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
        public int RoundAndClampX(double x)
        {
            int iX = (int) Math.Round(x);
            if (iX >= _maskWidth)
                iX = _maskWidth - 1;
            else if (iX < 0)
                iX = 0;
            return iX;
        }

        public int RoundAndClampY(double y)
        {
            int iY = (int) Math.Round(y);
            if (iY >= _maskHeight)
                iY = _maskHeight - 1;
            else if (iY < 0)
                iY = 0;
            return iY;
        }

        public double TransformX(int x)
        {
            return (x - _figure.OffsetX) / Divisor;
        }

        public double TransformY(int y)
        {
            return (_figure.Height - y) / Divisor - 1 + _figure.OffsetY / Divisor;
        }

        private PointD TransformPoint(Point point)
        {
            return new PointD(
                TransformX(point.X),
                TransformY(point.Y));
        }

        private void DrawConic(Point control, Point to)
        {
            PointD tFrom = new PointD(
                TransformX(_x),
                TransformY(_y));
            PointD tControl = TransformPoint(control);
            PointD tTo = TransformPoint(to);

            double dt = Curve.CalcConicDeltaT(tFrom, tControl, tTo);
            PointD p;
            int x;
            int y;
            int prevX = RoundAndClampX(tFrom.X);
            int prevY = RoundAndClampY(tFrom.Y);

            var curve = new Curve(tFrom, tControl, tTo);

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = curve.CalcConic(t);

                x = RoundAndClampX(p.X);
                y = RoundAndClampY(p.Y);

                if (((x - prevX) <= 1 && (x - prevX) >= -1)
                    && ((y - prevY) <= 1 && (y - prevY) >= -1))
                {
                    SetPixel(x, y);
                }
                else
                {
                    DrawLine(prevX, prevY, x, y);
                }

                prevX = x;
                prevY = y;
            }

            p = curve.CalcConic(1.0);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            if (((x - prevX) <= 1 && (x - prevX) >= -1)
                && ((y - prevY) <= 1 && (y - prevY) >= -1))
            {
                SetPixel(x, y);
            }
            else
            {
                DrawLine(prevX, prevY, x, y);
            }

            // TODO: Find a way to get rid of bunches of pixels.
            //       If the current (a) and last two pixels (b, c) fit in a 2x2 square,
            //       then we can get rid of the previous one (b).
        }

        private void DrawCubic(Point control1, Point control2, Point to)
        {
            PointD tFrom = new PointD(
                TransformX(_x),
                TransformY(_y));
            PointD tControl1 = TransformPoint(control1);
            PointD tControl2 = TransformPoint(control2);
            PointD tTo = TransformPoint(to);

            double dt = Curve.CalcCubicDeltaT(tFrom, tControl1, tControl2, tTo);
            PointD p;
            int x;
            int y;

            var curve = new Curve(tFrom, tControl1, tControl2, tTo);

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = curve.CalcCubic(t);

                x = RoundAndClampX(p.X);
                y = RoundAndClampY(p.Y);

                SetPixel(x, y);
            }

            p = curve.CalcCubic(1.0);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            SetPixel(x, y);
        }

        public void DrawContour(Contour contour, byte color)
        {
            _color = color;
            MoveTo(contour.FirstPoint);

            _figureWalker.WalkContour(contour);
        }

#if DEBUG
        public void DrawOutline(byte color = 1)
        {
            foreach (var contour in _figure.Contours)
            {
                DrawContour(contour, color);
            }
        }

        public Bitmap RenderBitmap()
        {
            var color = Color.Red;
            var bitmap = new Bitmap(_maskWidth, _maskHeight);

            for (int y = 0; y < _maskHeight; y++)
            {
                for (int x = 0; x < _maskWidth; x++)
                {
                    if (_maskBuf[y, x] != 0)
                        bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
#endif
    }
}
