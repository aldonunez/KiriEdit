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

                    OnBeginEdge();

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

        protected virtual void OnBeginEdge()
        {
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

        public Bitmap Bitmap { get { return bitmap; } }

        public DebugFigureRenderer(Figure figure) :
            base(figure)
        {
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

        protected override void OnBeginEdge()
        {
            int penIndex = _nextPenIndex;
            _nextPenIndex = (_nextPenIndex + 1) % _pens.Length;
            pen = _pens[penIndex];
        }

        protected override void OnEndFigure()
        {
            base.OnEndFigure();

            Pen redPen = new Pen(Color.Red);
            Pen orangePen = new Pen(Color.Orange);
            Pen whitePen = new Pen(Color.White);
            int j = 0;

            foreach (var group in Figure.PointGroups)
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
}
