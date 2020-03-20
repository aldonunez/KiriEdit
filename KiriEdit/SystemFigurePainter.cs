using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;
using TryFreetype;
using System.Drawing.Drawing2D;

namespace KiriEdit
{
    public class SystemFigurePainter : IDisposable
    {
        private Figure _figure;
        private FigureWalker _figureWalker;
        private Graphics _graphics;
        private GraphicsPath _graphicsPath;
        private int _x, _y;

        public SystemFigurePainter(Figure figure, Graphics g, Rectangle rect)
        {
            _figure = figure;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            _graphics = g;
            _graphicsPath = new GraphicsPath();

            float pixWidth = (int) (figure.Width / 64f);
            float pixHeight = (int) (figure.Height / 64f);

            float scale = rect.Height / pixHeight;

            int bmpWidth = (int) (pixWidth * scale);
            int bmpHeight = (int) (pixHeight * scale);

            g.ScaleTransform(scale / 64f, -scale / 64f, MatrixOrder.Append);
            g.TranslateTransform(
                rect.X + (float) -figure.OffsetX * scale / 64f,
                rect.Y + (bmpHeight - 1) + (float) figure.OffsetY * scale / 64f,
                MatrixOrder.Append);
        }

        public void Dispose()
        {
            if (_graphicsPath != null)
            {
                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
        }

        public void Paint()
        {
            foreach (var contour in _figure.Contours)
            {
                _graphicsPath.StartFigure();
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }

            _graphicsPath.CloseAllFigures();
            _graphics.FillPath(Brushes.Black, _graphicsPath);
        }

        private void BeginEdge()
        {
        }

        private void MoveTo(Point p)
        {
            var to = p;
            _x = to.X;
            _y = to.Y;
        }

        private void LineTo(Edge edge)
        {
            BeginEdge();
            var to = edge.P2;
            _graphicsPath.AddLine(
                (float) _x,
                (float) _y,
                (float) to.X,
                (float) to.Y);
            _x = to.X;
            _y = to.Y;
        }

        private void ConicTo(Edge edge)
        {
            BeginEdge();
            var control = ((ConicEdge) edge).Control1;
            var to = edge.P2;
            // TODO: Not quadratic.
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    new PointF((float) control.X, (float) control.Y),
                    new PointF((float) control.X, (float) control.Y),
                    new PointF((float) to.X, (float) to.Y)
                });
            _x = to.X;
            _y = to.Y;
        }

        private void CubicTo(Edge edge)
        {
            BeginEdge();
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    new PointF((float) control1.X, (float) control1.Y),
                    new PointF((float) control2.X, (float) control2.Y),
                    new PointF((float) to.X, (float) to.Y)
                });
            _x = to.X;
            _y = to.Y;
        }
    }
}
