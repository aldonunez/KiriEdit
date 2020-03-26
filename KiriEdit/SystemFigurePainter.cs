using KiriProj;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TryFreetype;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace KiriEdit
{
    public class SystemFigurePainter : IDisposable
    {
        private FigureDocument _document;
        private FigureWalker _figureWalker;
        private GraphicsPath _graphicsPath;
        private int _x, _y;

        public SystemFigurePainter(FigureDocument document)
        {
            _document = document;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            _graphicsPath = new GraphicsPath();
        }

        public void Dispose()
        {
            if (_graphicsPath != null)
            {
                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
        }

        public void SetTransform(Graphics g, Rectangle rect)
        {
            float pixHeight = _document.Figure.Height;

            float scale = (rect.Height - 1) / pixHeight;

            int bmpHeight = rect.Height;

            g.ResetTransform();
            g.ScaleTransform(scale, -scale, MatrixOrder.Append);
            g.TranslateTransform(
                rect.X + (float) -_document.Figure.OffsetX * scale,
                rect.Y + (bmpHeight - 1) + (float) _document.Figure.OffsetY * scale,
                MatrixOrder.Append);
        }

        private void PaintContour(Contour contour)
        {
            _graphicsPath.StartFigure();

            MoveTo(contour.FirstPoint);
            _figureWalker.WalkContour(contour);

            _graphicsPath.CloseFigure();
        }

        public void PaintFull()
        {
            _graphicsPath.Reset();

            foreach (var contour in _document.Figure.Contours)
            {
                PaintContour(contour);
            }
        }

        public void PaintShape(int index)
        {
            _graphicsPath.Reset();

            Figure figure = _document.Figure;

            var shape = _document.Shapes[index];

            foreach (var contourIndex in shape.Contours)
            {
                PaintContour(figure.Contours[contourIndex]);
            }
        }

        public void Draw(Graphics g)
        {
            Draw(g, Pens.Red);
        }

        public void Draw(Graphics g, Pen pen)
        {
            g.DrawPath(pen, _graphicsPath);
        }

        public void Fill(Graphics g)
        {
            Fill(g, Brushes.Black);
        }

        public void Fill(Graphics g, Brush brush)
        {
            g.FillPath(brush, _graphicsPath);
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
            var c1 = new PointF(
                (_x + 2 * control.X) / 3.0f,
                (_y + 2 * control.Y) / 3.0f
                );
            var c2 = new PointF(
                (to.X + 2 * control.X) / 3.0f,
                (to.Y + 2 * control.Y) / 3.0f
                );
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    c1,
                    c2,
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

        public static Rectangle CenterFigure(Figure figure, Size boundSize)
        {
            int height = boundSize.Height;
            int width = boundSize.Width;

            float figureWidthToHeight = figure.Width / (float) figure.Height;
            int scaledWidth = (int) (width * figureWidthToHeight);

            Rectangle rect = new Rectangle(
                (width - scaledWidth) / 2,
                0,
                scaledWidth,
                height);

            return rect;
        }
    }
}
