using SharpFont;
using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public interface IFigureRenderer
    {
        Bitmap Bitmap { get; }

        void Render();
    }

    public abstract class FigureRendererBase : IFigureRenderer
    {
        private readonly Bitmap bitmap;

        double x, y;
        protected Graphics g { get; }
        Pen pen;

        protected Figure figure { get; }

        public Bitmap Bitmap { get { return bitmap; } }

        public FigureRendererBase(Figure figure)
        {
            this.figure = figure;

            int width = figure.Width;
            int height = figure.Height;

            bitmap = new Bitmap(width, height);
            Pen borderPen = new Pen(Color.White);

            g = Graphics.FromImage(bitmap);
            //g.DrawRectangle(borderPen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            //g.DrawEllipse(borderPen, 128, 224, 10, 10);
            g.ScaleTransform(1, -1);
            g.TranslateTransform(
                (float) -figure.OffsetX,
                -(height - 1) - (float) figure.OffsetY);
        }

        protected virtual void OnBeginContour(Contour contour)
        {
        }

        protected abstract Pen OnBeginEdge();

        protected virtual void OnEndFigure()
        {
        }

        public void Render()
        {
            foreach (var contour in figure.Contours)
            {
                OnBeginContour(contour);

                MoveToFunc(contour.FirstPoint);

                Point point = contour.FirstPoint;

                for (int i = 0; true; i++)
                {
                    var edge = point.OutgoingEdge;

                    pen = OnBeginEdge();

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

                    if ( point == contour.FirstPoint )
                        break;
                }
            }

            OnEndFigure();
        }

        private int MoveToFunc(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            x = to.X;
            y = to.Y;
            return 0;
        }

        private int LineToFunc(Edge edge)
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
            return 0;
        }

        private int ConicToFunc(Edge edge)
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
            return 0;
        }

        private int CubicToFunc(Edge edge)
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
            return 0;
        }
    }

    public class DebugFigureRenderer : FigureRendererBase
    {
        private Pen[] _pens;
        private int _nextPenIndex;

        public DebugFigureRenderer(Figure figure) :
            base(figure)
        {
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
    }

    public class FigureRenderer : FigureRendererBase
    {
        Pen _pen;

        public FigureRenderer(Figure figure) :
            base(figure)
        {
            _pen = new Pen(Color.Black);
        }

        protected override Pen OnBeginEdge()
        {
            return _pen;
        }
    }

    public class FigureRenderer2 : IFigureRenderer
    {
        private readonly Figure figure;
        private readonly Bitmap bitmap;
        private readonly Graphics g;

        private double x, y;
        private Pen pen;

        public Bitmap Bitmap => bitmap;

        public FigureRenderer2(Figure figure)
        {
            this.figure = figure;

            int width = figure.Width;
            int height = figure.Height;

            bitmap = new Bitmap(width, height);
            Pen borderPen = new Pen(Color.White);

            g = Graphics.FromImage(bitmap);
            //g.DrawRectangle(borderPen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            //g.DrawEllipse(borderPen, 128, 224, 10, 10);
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
            x = to.X;
            y = to.Y;
            return 0;
        }

        private int LineToFunc(Edge edge)
        {
            var to = edge.P2;
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
#if false
            g.DrawLine(
                pen,
                (float) x,
                (float) y,
                (float) to.X,
                (float) to.Y);
#else
            // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

            int x0 = (int) Math.Round(x);
            int y0 = (int) Math.Round(y);
            int x1 = (int) Math.Round(to.X);
            int y1 = (int) Math.Round(to.Y);

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = dx + dy;

            while (true)
            {
                int px = x0;
                int py = figure.Height - y0 - 1;

                bitmap.SetPixel(px, py, pen.Color);

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
#endif
            x = to.X;
            y = to.Y;
            return 0;
        }

        private int ConicToFunc(Edge edge)
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
            return 0;
        }

        private int CubicToFunc(Edge edge)
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
            return 0;
        }
    }
}
