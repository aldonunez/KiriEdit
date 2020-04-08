using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using KiriFig;
using KiriFig.Model;
using Point = KiriFig.Model.Point;

namespace KiriFigDemo.Sample4
{
    static class Sample
    {
        public static void Run()
        {
            Figure figure;

            using (var lib = new FontLibrary())
            using (var face = lib.OpenFace(@"C:\Windows\Fonts\consola.ttf", 0))
            {
                figure = face.DecomposeGlyph('A');
            }

            {
                Console.WriteLine("Starting Shape count: {0}", figure.Shapes.Count);

#if !true
                Point p6 = figure.PointGroups[6].Points[0];
                Point p1 = new Point((p6.X + p6.OutgoingEdge.P2.X) / 2, p6.Y);
                var e = figure.PointGroups[6].Points[0].OutgoingEdge;
                var midPoint = figure.AddDiscardablePoint(p1, e);

#if true
                var cut = figure.AddCut(midPoint, figure.PointGroups[9].Points[0]);
#endif

#elif true
                Point p6 = figure.PointGroups[6].Points[0];
                Point p1 = new Point((p6.X + p6.OutgoingEdge.P2.X) / 2, p6.Y);
                var e = figure.PointGroups[6].Points[0].OutgoingEdge;
                var midPoint = figure.AddDiscardablePoint(p1, e);

                var (p2, p3) = Figure.FindPointsForCut(midPoint.Group, figure.PointGroups[9]);

                Debug.Assert(p2 == midPoint || p3 == midPoint);
                Debug.Assert(p2 == figure.PointGroups[9].Points[0] || p3 == figure.PointGroups[9].Points[0]);

#if true
                var cut = figure.AddCut(p2, p3);
                Console.WriteLine("Shape count: {0}", figure.Shapes.Count);
#endif

                p1 = new Point(559, 444);
                PointGroup newGroup2 = figure.AddDiscardablePoint(p1, figure.PointGroups[10].Points[0].OutgoingEdge).Group;

                p1 = new Point(559, 285);
                PointGroup newGroup3 = figure.AddDiscardablePoint(p1, figure.PointGroups[2].Points[0].OutgoingEdge).Group;

                (p2, p3) = Figure.FindPointsForCut(newGroup2, newGroup3);
                cut = figure.AddCut(p2, p3);
                Console.WriteLine("Shape count: {0}", figure.Shapes.Count);


                (p2, p3) = Figure.FindPointsForCut(figure.PointGroups[6], figure.PointGroups[9]);
                cut = figure.AddCut(p2, p3);
                Console.WriteLine("Shape count: {0}", figure.Shapes.Count);


                (p2, p3) = Figure.FindPointsForCut(figure.PointGroups[7], figure.PointGroups[9]);
                cut = figure.AddCut(p2, p3);
                Console.WriteLine("Shape count: {0}", figure.Shapes.Count);
#endif
            }

            Console.WriteLine("OffsetX: {0}", figure.OffsetX);
            Console.WriteLine("OffsetY: {0}", figure.OffsetY);
            Console.WriteLine("Shape count: {0}", figure.Shapes.Count);

            Bitmap bitmap = new Bitmap(figure.Width, figure.Height);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(bitmap))
            using (var painter = new SystemFigurePainter(figure))
            {
                painter.SetTransform(g, rect);
                painter.PaintFull();
                painter.Draw(g, Pens.Red);
            }

            bitmap.Save(@"C:\Temp\y.png");

            using (var writer = new StreamWriter(@"C:\Temp\y.fig"))
            {
                FigureSerializer.Serialize(figure, writer);
            }
        }
    }


    //------------------------------------------------------------------------

    public class SystemFigurePainter : IDisposable
    {
        private Figure _figure;
        private FigureWalker _figureWalker;
        private GraphicsPath _graphicsPath;
        private int _x, _y;

        public SystemFigurePainter(Figure figure)
        {
            _figure = figure;

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

        // TODO: Call this from SetTransform.
        public static Matrix BuildTransform(Figure figure, Rectangle rect)
        {
            float scale = (rect.Height - 1) / (float) figure.Height;

            Matrix m = new Matrix();

            m.Scale(scale, -scale, MatrixOrder.Append);
            m.Translate(
                rect.X + (float) -figure.OffsetX * scale,
                rect.Y + (rect.Height - 1) + (float) figure.OffsetY * scale,
                MatrixOrder.Append);

            return m;
        }

        public void SetTransform(Graphics g, Rectangle rect)
        {
            float pixHeight = _figure.Height;

            float scale = (rect.Height - 1) / pixHeight;

            int bmpHeight = rect.Height;

            g.ResetTransform();
            g.ScaleTransform(scale, -scale, MatrixOrder.Append);
            g.TranslateTransform(
                rect.X + (float) -_figure.OffsetX * scale,
                rect.Y + (bmpHeight - 1) + (float) _figure.OffsetY * scale,
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

            foreach (var contour in _figure.Contours)
            {
                PaintContour(contour);
            }
        }

        public void PaintShape(int index)
        {
            _graphicsPath.Reset();

            Figure figure = _figure;

            var shape = _figure.Shapes[index];

            foreach (var contour in shape.Contours)
            {
                PaintContour(contour);
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
#if !DRAW_COARSE_CURVES
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    c1,
                    c2,
                    new PointF((float) to.X, (float) to.Y)
                });
#else
            _graphicsPath.AddLine((float) _x, (float) _y, (float) control.X, (float) control.Y);
            _graphicsPath.AddLine((float) control.X, (float) control.Y, (float) to.X, (float) to.Y);
#endif
            _x = to.X;
            _y = to.Y;
        }

        private void CubicTo(Edge edge)
        {
            BeginEdge();
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
#if !DRAW_COARSE_CURVES
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    new PointF((float) control1.X, (float) control1.Y),
                    new PointF((float) control2.X, (float) control2.Y),
                    new PointF((float) to.X, (float) to.Y)
                });
#else
            _graphicsPath.AddLine((float) _x, (float) _y, (float) control1.X, (float) control1.Y);
            _graphicsPath.AddLine((float) control1.X, (float) control1.Y, (float) control2.X, (float) control2.Y);
            _graphicsPath.AddLine((float) control2.X, (float) control2.Y, (float) to.X, (float) to.Y);
#endif
            _x = to.X;
            _y = to.Y;
        }
    }
}
