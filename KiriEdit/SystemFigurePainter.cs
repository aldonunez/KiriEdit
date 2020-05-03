/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriProj;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using KiriFig;
using KiriFig.Model;
using Point = KiriFig.Model.Point;

namespace KiriEdit
{
    public class SystemFigurePainter : IDisposable
    {
        private FigureDocument _document;
        private FigureWalker _figureWalker;
        private GraphicsPath _graphicsPath;
        private int _x, _y;

        public SystemFigurePainter( FigureDocument document )
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
            if ( _graphicsPath != null )
            {
                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
        }

        public static Matrix BuildTransform( Figure figure, Rectangle rect )
        {
            float scale = (rect.Height - 1) / (float) figure.Height;

            Matrix m = new Matrix();

            m.Scale( scale, -scale, MatrixOrder.Append );
            m.Translate(
                rect.X + (float) -figure.OffsetX * scale,
                rect.Y + (rect.Height - 1) + (float) figure.OffsetY * scale,
                MatrixOrder.Append );

            return m;
        }

        public void SetTransform( Graphics g, Rectangle rect )
        {
            using ( Matrix m = BuildTransform( _document.Figure, rect ) )
            {
                g.Transform = m;
            }
        }

        private void PaintContour( Contour contour )
        {
            _graphicsPath.StartFigure();

            MoveTo( contour.FirstPoint );
            _figureWalker.WalkContour( contour );

            _graphicsPath.CloseFigure();
        }

        public void PaintFull()
        {
            _graphicsPath.Reset();

            foreach ( var contour in _document.Figure.Contours )
            {
                PaintContour( contour );
            }
        }

        public void PaintShape( int index )
        {
            _graphicsPath.Reset();

            var shape = _document.Figure.Shapes[index];

            foreach ( var contour in shape.Contours )
            {
                PaintContour( contour );
            }
        }

        public void Draw( Graphics g )
        {
            Draw( g, Pens.Red );
        }

        public void Draw( Graphics g, Pen pen )
        {
            g.DrawPath( pen, _graphicsPath );
        }

        public void Fill( Graphics g )
        {
            Fill( g, Brushes.Black );
        }

        public void Fill( Graphics g, Brush brush )
        {
            g.FillPath( brush, _graphicsPath );
        }

        private void BeginEdge()
        {
        }

        private void MoveTo( Point p )
        {
            var to = p;
            _x = to.X;
            _y = to.Y;
        }

        private void LineTo( Edge edge )
        {
            BeginEdge();
            var to = edge.P2;
            _graphicsPath.AddLine(
                (float) _x,
                (float) _y,
                (float) to.X,
                (float) to.Y );
            _x = to.X;
            _y = to.Y;
        }

        private void ConicTo( Edge edge )
        {
            BeginEdge();
            var control1 = ((ConicEdge) edge).C1;
            var to = edge.P2;
            var c1 = new PointF(
                (_x + 2 * control1.X) / 3.0f,
                (_y + 2 * control1.Y) / 3.0f
                );
            var c2 = new PointF(
                (to.X + 2 * control1.X) / 3.0f,
                (to.Y + 2 * control1.Y) / 3.0f
                );
#if !DRAW_COARSE_CURVES
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    c1,
                    c2,
                    new PointF((float) to.X, (float) to.Y)
                } );
#else
            DrawFlattenedCurve( edge );
#endif
            _x = to.X;
            _y = to.Y;
        }

        private void CubicTo( Edge edge )
        {
            BeginEdge();
            var c1 = ((CubicEdge) edge).C1;
            var c2 = ((CubicEdge) edge).C2;
            var to = edge.P2;
#if !DRAW_COARSE_CURVES
            _graphicsPath.AddBeziers(
                new PointF[]
                {
                    new PointF((float) _x, (float) _y),
                    new PointF((float) c1.X, (float) c1.Y),
                    new PointF((float) c2.X, (float) c2.Y),
                    new PointF((float) to.X, (float) to.Y)
                } );
#else
            DrawFlattenedCurve( edge );
#endif
            _x = to.X;
            _y = to.Y;
        }

        private void DrawFlattenedCurve( Edge edge )
        {
            PointF prevPoint = new PointF( _x, _y );

            foreach ( var pd in edge.Flatten() )
            {
                PointF pf = new PointF( (float) pd.X, (float) pd.Y );
                _graphicsPath.AddLine( prevPoint, pf );
                prevPoint = pf;
            }
        }
    }
}
