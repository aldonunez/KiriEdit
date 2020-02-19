using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public class OutlineRenderer
    {
        private Figure _figure;
        private FigureWalker _figureWalker;
        private double _x, _y;
        private Color _color = Color.Red;

        private int _curContourIndex;
        private byte[,] _maskBuf;

        public OutlineRenderer(Figure figure)
        {
            _figure = figure;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            int width = figure.Width;
            int height = figure.Height;

            _maskBuf = new byte[height, width];

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
            _maskBuf[y, x] = (byte) (1 << _curContourIndex);
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

        private int RoundAndClampX(double x)
        {
            int iX = (int) Math.Round(x);
            if (iX >= _figure.Width)
                iX = _figure.Width - 1;
            else if (iX < 0)
                iX = 0;
            return iX;
        }

        private int RoundAndClampY(double y)
        {
            int iY = (int) Math.Round(y);
            if (iY >= _figure.Height)
                iY = _figure.Height - 1;
            else if (iY < 0)
                iY = 0;
            return iY;
        }

        private double TransformX(double x)
        {
            return x - _figure.OffsetX;
        }

        private double TransformY(double y)
        {
            return _figure.Height - y - 1 + _figure.OffsetY;
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

            double dt = Curve.CalcConicDeltaT(tFrom, tControl, tTo);
            ValuePoint p;
            int x;
            int y;
            int prevX = RoundAndClampX(tFrom.X);
            int prevY = RoundAndClampY(tFrom.Y);

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = Curve.CalcConic(t, tFrom, tControl, tTo);

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

            p = Curve.CalcConic(1.0, tFrom, tControl, tTo);

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

            SetPixel(x, y);

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

            double dt = Curve.CalcCubicDeltaT(tFrom, tControl1, tControl2, tTo);
            ValuePoint p;
            int x;
            int y;

            for (double t = 0.0; t < 1.0; t += dt)
            {
                p = Curve.CalcCubic(t, tFrom, tControl1, tControl2, tTo);

                x = RoundAndClampX(p.X);
                y = RoundAndClampY(p.Y);

                SetPixel(x, y);
            }

            p = Curve.CalcCubic(1.0, tFrom, tControl1, tControl2, tTo);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            SetPixel(x, y);
        }

        //--------------------------------------------------------------------

        private enum Orientation
        {
            Unknown,
            Inside,
            Outside
        }

        private static Orientation GetOrientation(Contour contour)
        {
            double prevX = contour.FirstPoint.X;
            double prevY = contour.FirstPoint.Y;
            Point p = contour.FirstPoint;
            double area = 0;

            while (true)
            {
                var edge = p.OutgoingEdge;

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        area += (edge.P2.Y - prevY) * (edge.P2.X + prevX);
                        break;

                    case EdgeType.Conic:
                        {
                            var conicEdge = (ConicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.P2.Y - prevY) * (conicEdge.P2.X + prevX);
                        }
                        break;

                    case EdgeType.Cubic:
                        {
                            var conicEdge = (CubicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.Control2.Y - prevY) * (conicEdge.Control2.X + prevX);
                            area += (conicEdge.P2.Y - prevY) * (conicEdge.P2.X + prevX);
                        }
                        break;
                }

                prevX = edge.P2.X;
                prevY = edge.P2.Y;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            if (area > 0)
                return Orientation.Inside;
            else if (area < 0)
                return Orientation.Outside;
            else
                return Orientation.Unknown;
        }

        private static Point FindRightmostPoint(Contour contour)
        {
            Point p = contour.FirstPoint;
            Point rightP = new Point(0, 0);

            while (true)
            {
                if (p.X > rightP.X)
                    rightP = p;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            return rightP;
        }

        public void CalculateShapes()
        {
            var outsideContours = new List<Contour>();
            var insideContours = new List<Contour>();

            foreach (var contour in _figure.Contours)
            {
                Orientation orientation = GetOrientation(contour);

                switch (orientation)
                {
                    case Orientation.Inside:
                        insideContours.Add(contour);
                        break;

                    case Orientation.Outside:
                        outsideContours.Add(contour);
                        break;

                    default:
                        Debug.Fail("");
                        break;
                }
            }

            foreach (var contour in _figure.Contours)
            {
                if (insideContours.Contains(contour))
                    continue;

                _curContourIndex = _figure.Contours.IndexOf(contour);
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }

            foreach (var contour in insideContours)
            {
                Point rightP = FindRightmostPoint(contour);
                int y = (int) Math.Round(rightP.Y);
                int outerIndex = 0;

                for (int x = (int) Math.Round(rightP.X); x < _figure.Width - 1; x++)
                {
                    byte b = _maskBuf[y, x];

                    if (b != 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if ((b & (1 << i)) != 0)
                            {
                                outerIndex = i;
                                Console.WriteLine("Found");
                                goto Found;
                            }
                        }
                    }
                }
            Found:
                int innerIndex = _figure.Contours.IndexOf(contour);
                Console.WriteLine("{0} goes with {1}", innerIndex, outerIndex);

                // TODO: At this point we can:
                //      1. Match all inner contours with outer ones.
                //      2. Serialize SVG files with matching sets of inner and outer contours.
            }
        }

#if DEBUG
        public void RenderOutline()
        {
            foreach (var contour in _figure.Contours)
            {
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }
        }

        public Bitmap RenderBitmap()
        {
            var color = Color.Red;
            var bitmap = new Bitmap(_figure.Width, _figure.Height);

            for (int y = 0; y < _figure.Height; y++)
            {
                for (int x = 0; x < _figure.Width; x++)
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
