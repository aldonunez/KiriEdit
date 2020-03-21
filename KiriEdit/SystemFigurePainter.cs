using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;
using TryFreetype;
using System.Drawing.Drawing2D;

namespace KiriEdit
{
    public enum FigurePainterSection
    {
        Enabled,
        Disabled,
        Full,
    }

    public class SystemFigurePainter : IDisposable
    {
        private FigureDocument _document;
        private FigureWalker _figureWalker;
        private Graphics _graphics;
        private GraphicsPath _graphicsPath;
        private int _x, _y;
        private FigurePainterSection _section;
        private bool _preparedPath;

        public SystemFigurePainter(
            FigureDocument document,
            Graphics g,
            Rectangle rect,
            FigurePainterSection section)
        {
            _document = document;
            _section = section;

            _figureWalker = new FigureWalker();
            _figureWalker.LineTo += LineTo;
            _figureWalker.ConicTo += ConicTo;
            _figureWalker.CubicTo += CubicTo;

            _graphics = g;
            _graphicsPath = new GraphicsPath();

            float pixWidth = (int) (document.Figure.Width / 64f);
            float pixHeight = (int) Math.Ceiling(document.Figure.Height / 64f);

            float scale = (rect.Height - 1) / pixHeight;

            int bmpWidth = rect.Width;
            int bmpHeight = rect.Height;

            g.ResetTransform();
            g.ScaleTransform(scale / 64f, -scale / 64f, MatrixOrder.Append);
            g.TranslateTransform(
                rect.X + (float) -document.Figure.OffsetX * scale / 64f,
                rect.Y + (bmpHeight - 1) + (float) document.Figure.OffsetY * scale / 64f,
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

        private void PaintFull()
        {
            foreach (var contour in _document.Figure.Contours)
            {
                _graphicsPath.StartFigure();
                MoveTo(contour.FirstPoint);

                _figureWalker.WalkContour(contour);
            }

            _graphicsPath.CloseAllFigures();
        }

        private void PaintPart(bool enabled)
        {
            Figure figure = _document.Figure;

            foreach (var shape in _document.Shapes)
            {
                if (shape.Enabled != enabled)
                    continue;

                foreach (var contourIndex in shape.Contours)
                {
                    Contour contour = figure.Contours[contourIndex];

                    _graphicsPath.StartFigure();

                    MoveTo(contour.FirstPoint);
                    _figureWalker.WalkContour(contour);

                    _graphicsPath.CloseFigure();
                }
            }
        }

        private void Paint()
        {
            if (_preparedPath)
                return;

            switch (_section)
            {
                case FigurePainterSection.Full:
                    PaintFull();
                    break;

                case FigurePainterSection.Enabled:
                    PaintPart(true);
                    break;

                case FigurePainterSection.Disabled:
                    PaintPart(false);
                    break;
            }

            _preparedPath = true;
        }

        public void Draw()
        {
            Paint();
            _graphics.DrawPath(Pens.Red, _graphicsPath);
        }

        public void Fill()
        {
            Paint();
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
