using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public delegate void LineToHandler(Edge edge);
    public delegate void ConicToHandler(Edge edge);
    public delegate void CubicToHandler(Edge edge);

    public class FigureWalker
    {
        public event LineToHandler LineTo;
        public event ConicToHandler ConicTo;
        public event CubicToHandler CubicTo;

        public void WalkContour(Contour contour)
        {
            Point point = contour.FirstPoint;

            while (true)
            {
                var edge = point.OutgoingEdge;

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

        protected virtual void OnLineTo(Edge edge)
        {
            LineTo?.Invoke(edge);
        }

        protected virtual void OnConicTo(Edge edge)
        {
            ConicTo?.Invoke(edge);
        }

        protected virtual void OnCubicTo(Edge edge)
        {
            CubicTo?.Invoke(edge);
        }
    }

    public class DebugFigureRenderer
    {
        private Figure _figure;
        private FigureWalker _figureWalker;

        double x, y;
        private readonly Bitmap bitmap;
        protected Graphics g { get; }
        Pen pen;

        private Pen[] _pens;
        private int _nextPenIndex;

        public Bitmap Bitmap { get { return bitmap; } }

        public DebugFigureRenderer(Figure figure)
        {
            _figure = figure;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

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

        public void Render()
        {
            foreach (var contour in _figure.Contours)
            {
                BeginContour(contour);
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }

            EndFigure();
        }

        private void BeginContour(Contour contour)
        {
            _nextPenIndex = 0;
        }

        private void BeginEdge()
        {
            int penIndex = _nextPenIndex;
            _nextPenIndex = (_nextPenIndex + 1) % _pens.Length;
            pen = _pens[penIndex];
        }

        private void EndFigure()
        {
            Pen redPen = new Pen(Color.Red);
            Pen orangePen = new Pen(Color.Orange);
            Pen whitePen = new Pen(Color.White);
            int j = 0;

            foreach (var group in _figure.PointGroups)
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

        private void MoveTo(Point p)
        {
            var to = p;
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            x = to.X;
            y = to.Y;
        }

        private void LineTo(Edge edge)
        {
            BeginEdge();
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

        private void ConicTo(Edge edge)
        {
            BeginEdge();
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

        private void CubicTo(Edge edge)
        {
            BeginEdge();
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
}
