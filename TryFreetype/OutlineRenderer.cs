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
        public class Shape
        {
            public Contour OuterContour { get; }
            public Contour[] InnerContours { get; }

            public Shape(Contour outsideContour, Contour[] insideCountours)
            {
                OuterContour = outsideContour;
                InnerContours = insideCountours;
            }
        }

        private Figure _figure;
        private FigureWalker _figureWalker;
        private int _x, _y;

        private int _curContourIndex;
        private byte[,] _maskBuf;
        private int _maskWidth;
        private int _maskHeight;

        public OutlineRenderer(Figure figure)
        {
            _figure = figure;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            int width = figure.Width / 64;
            int height = figure.Height / 64;

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
            if (iX >= _maskWidth)
                iX = _maskWidth - 1;
            else if (iX < 0)
                iX = 0;
            return iX;
        }

        private int RoundAndClampY(double y)
        {
            int iY = (int) Math.Round(y);
            if (iY >= _maskHeight)
                iY = _maskHeight - 1;
            else if (iY < 0)
                iY = 0;
            return iY;
        }

        private double TransformX(int x)
        {
            return (x - _figure.OffsetX) / 64.0;
        }

        private double TransformY(int y)
        {
            return (_figure.Height - y) / 64.0 - 1 + _figure.OffsetY / 64.0;
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
            int prevX = contour.FirstPoint.X;
            int prevY = contour.FirstPoint.Y;
            Point p = contour.FirstPoint;
            long area = 0;

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

        private (List<Contour> outsideContours, List<Contour> insideContours) PartitionContours()
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

            return (outsideContours, insideContours);
        }

        private void DrawOutsides(List<Contour> outsideContours)
        {
            for (int i = 0; i < outsideContours.Count; i++)
            {
                var contour = outsideContours[i];

                _curContourIndex = i;
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }
        }

        private List<Contour>[] DetermineInsides(List<Contour> outsideContours, List<Contour> insideContours)
        {
            var insideContourLists = new List<Contour>[outsideContours.Count];

            for (int i = 0; i < outsideContours.Count; i++)
                insideContourLists[i] = new List<Contour>();

            foreach (var contour in insideContours)
            {
                Point rightP = FindRightmostPoint(contour);
                int y = RoundAndClampY(TransformY(rightP.Y));

                for (int x = RoundAndClampX(TransformX(rightP.X)); x < _maskWidth; x++)
                {
                    byte b = _maskBuf[y, x];

                    if (b != 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if ((b & (1 << i)) != 0)
                            {
                                insideContourLists[i].Add(contour);

                                int outerIndex = _figure.Contours.IndexOf(outsideContours[i]);
                                Console.WriteLine("Found");
                                int innerIndex = _figure.Contours.IndexOf(contour);
                                Console.WriteLine("{0} goes with {1}", innerIndex, outerIndex);

                                goto Found;
                            }
                        }
                    }
                }

                Debug.Fail("");

            Found:
                ;
            }

            return insideContourLists;
        }

        private Shape[] PackageShapes(List<Contour> outsideContours, List<Contour>[] insideContourLists)
        {
            var shapes = new Shape[outsideContours.Count];

            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = new Shape(outsideContours[i], insideContourLists[i].ToArray());
            }

            return shapes;
        }

        public Shape[] CalculateShapes()
        {
            var (outsideContours, insideContours) = PartitionContours();
            DrawOutsides(outsideContours);
            var insideContourLists = DetermineInsides(outsideContours, insideContours);

            return PackageShapes(outsideContours, insideContourLists);
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
