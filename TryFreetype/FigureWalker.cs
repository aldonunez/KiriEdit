using SharpFont;
using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public abstract class FigureWalkerBase
    {
        protected Figure Figure { get; }

        protected FigureWalkerBase(Figure figure)
        {
            Figure = figure;
        }

        public void Render()
        {
            foreach (var contour in Figure.Contours)
            {
                OnBeginContour(contour);

                OnMoveTo(contour.FirstPoint);

                Point point = contour.FirstPoint;

                for (int i = 0; true; i++)
                {
                    var edge = point.OutgoingEdge;

                    Pen pen = OnBeginEdge();

                    switch (edge.Type)
                    {
                        case EdgeType.Line:
                            OnLineTo(edge);
                            break;

                        case EdgeType.Conic:
                            OnConicTo(edge);
                            break;

                        case EdgeType.Cubic:
                            OnCubicTo(edge);
                            break;
                    }

                    point = edge.P2;

                    if (point == contour.FirstPoint)
                        break;
                }
            }

            OnEndFigure();
        }

        protected virtual void OnMoveTo(Point point)
        {
        }

        protected virtual void OnLineTo(Edge edge)
        {
        }

        protected virtual void OnConicTo(Edge edge)
        {
        }

        protected virtual void OnCubicTo(Edge edge)
        {
        }

        protected virtual void OnBeginContour(Contour contour)
        {
        }

        protected virtual Pen OnBeginEdge()
        {
            return null;
        }

        protected virtual void OnEndFigure()
        {
        }
    }

    public class DebugFigureRenderer : FigureWalkerBase
    {
        double x, y;
        private readonly Bitmap bitmap;
        protected Graphics g { get; }
        Pen pen;

        private Pen[] _pens;
        private int _nextPenIndex;

        protected Figure figure { get; }

        public Bitmap Bitmap { get { return bitmap; } }

        public DebugFigureRenderer(Figure figure) :
            base(figure)
        {
            this.figure = figure;

            int width = figure.Width;
            int height = figure.Height;

            bitmap = new Bitmap(width, height);

            g = Graphics.FromImage(bitmap);
            g.ScaleTransform(1, -1);
            g.TranslateTransform(
                (float) -figure.OffsetX,
                -(height - 1) - (float) figure.OffsetY);

            _pens = new Pen[4]
            {
                new Pen(Color.Red),
                new Pen(Color.Blue),
                new Pen(Color.Yellow),
                new Pen(Color.Green)
            };
        }

        protected override void OnBeginContour(Contour contour)
        {
            _nextPenIndex = 0;
        }

        protected override Pen OnBeginEdge()
        {
            int penIndex = _nextPenIndex;
            _nextPenIndex = (_nextPenIndex + 1) % _pens.Length;
            return _pens[penIndex];
        }

        protected override void OnEndFigure()
        {
            base.OnEndFigure();

            Pen redPen = new Pen(Color.Red);
            Pen orangePen = new Pen(Color.Orange);
            Pen whitePen = new Pen(Color.White);
            int j = 0;

            foreach (var group in figure.PointGroups)
            {
                Point p = group.Points[0];
                Pen pen = null;
                float radius = 5f;
                float wideRadius = radius + 2f;

                if (group.IsFixed)
                {
                    pen = redPen;
                }
                else
                {
                    pen = orangePen;
                }
                j++;

                g.DrawEllipse(
                    pen,
                    (float) p.X - radius,
                    (float) p.Y - radius,
                    radius * 2,
                    radius * 2
                    );

                if (group.Points.Count > 1)
                {
                    g.DrawEllipse(
                        whitePen,
                        (float) p.X - wideRadius,
                        (float) p.Y - wideRadius,
                        wideRadius * 2,
                        wideRadius * 2
                        );
                }
            }
        }

        protected override void OnMoveTo(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            x = to.X;
            y = to.Y;
        }

        protected override void OnLineTo(Edge edge)
        {
            var to = edge.P2;
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            g.DrawLine(
                pen,
                (float) x,
                (float) y,
                (float) to.X,
                (float) to.Y);
            x = to.X;
            y = to.Y;
        }

        protected override void OnConicTo(Edge edge)
        {
            var control = ((ConicEdge) edge).Control1;
            var to = edge.P2;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            // TODO: Not quadratic.
            g.DrawBeziers(
                pen,
                new PointF[]
                {
                    new PointF((float) x, (float) y),
                    new PointF((float) control.X, (float) control.Y),
                    new PointF((float) control.X, (float) control.Y),
                    new PointF((float) to.X, (float) to.Y)
                });
            x = to.X;
            y = to.Y;
        }

        protected override void OnCubicTo(Edge edge)
        {
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            g.DrawBeziers(
                pen,
                new PointF[]
                {
                    new PointF((float) x, (float) y),
                    new PointF((float) control1.X, (float) control1.Y),
                    new PointF((float) control2.X, (float) control2.Y),
                    new PointF((float) to.X, (float) to.Y)
                });
            x = to.X;
            y = to.Y;
        }
    }

    public class OutlineRenderer : FigureWalkerBase
    {
        private readonly Figure figure;
        private readonly Bitmap bitmap;
        private readonly Graphics g;

        private double _x, _y;
        private Pen pen;

        public Bitmap Bitmap => bitmap;

        public OutlineRenderer(Figure figure) :
            base(figure)
        {
            this.figure = figure;

            int width = figure.Width;
            int height = figure.Height;

            bitmap = new Bitmap(width, height);
            Pen borderPen = new Pen(Color.White);

            g = Graphics.FromImage(bitmap);
            g.ScaleTransform(1, -1);
            g.TranslateTransform(
                (float) -figure.OffsetX,
                -(height - 1) - (float) figure.OffsetY);

            pen = new Pen(Color.Red);
        }

        public void Render()
        {
            foreach (var contour in figure.Contours)
            {
                MoveToFunc(contour.FirstPoint);

                Point point = contour.FirstPoint;

                for (int i = 0; true; i++)
                {
                    var edge = point.OutgoingEdge;

                    switch (edge.Type)
                    {
                        case EdgeType.Line:
                            LineToFunc(edge);
                            break;

                        case EdgeType.Conic:
                            ConicToFunc(edge);
                            break;

                        case EdgeType.Cubic:
                            CubicToFunc(edge);
                            break;
                    }

                    point = edge.P2;

                    if (point == contour.FirstPoint)
                        break;
                }
            }
        }

        private int MoveToFunc(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            _x = to.X;
            _y = to.Y;
            return 0;
        }

        private int LineToFunc(Edge edge)
        {
            var to = edge.P2;
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            DrawLine(to);
            _x = to.X;
            _y = to.Y;
            return 0;
        }

        private int ConicToFunc(Edge edge)
        {
            var control = ((ConicEdge) edge).Control1;
            var to = edge.P2;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            DrawConic(control, to);
            _x = to.X;
            _y = to.Y;
            return 0;
        }

        private int CubicToFunc(Edge edge)
        {
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            DrawCubic(control1, control2, to);
            _x = to.X;
            _y = to.Y;
            return 0;
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
                bitmap.SetPixel(x0, y0, pen.Color);

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
            if (iX >= figure.Width)
                iX = figure.Width - 1;
            else if (iX < 0)
                iX = 0;
            return iX;
        }

        private int RoundAndClampY(double y)
        {
            int iY = (int) Math.Round(y);
            if (iY >= figure.Height)
                iY = figure.Height - 1;
            else if (iY < 0)
                iY = 0;
            return iY;
        }

        private double TransformX(double x)
        {
            return x - figure.OffsetX;
        }

        private double TransformY(double y)
        {
            return figure.Height - y - 1 + figure.OffsetY;
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
                    bitmap.SetPixel(x, y, pen.Color);
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
                bitmap.SetPixel(x, y, pen.Color);
            }
            else
            {
                DrawLine(prevX, prevY, x, y);
            }

            bitmap.SetPixel(x, y, pen.Color);

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

                bitmap.SetPixel(x, y, pen.Color);
            }

            p = CalcCubic(1.0, tFrom, tControl1, tControl2, tTo);

            x = RoundAndClampX(p.X);
            y = RoundAndClampY(p.Y);

            bitmap.SetPixel(x, y, pen.Color);
        }
    }
}
