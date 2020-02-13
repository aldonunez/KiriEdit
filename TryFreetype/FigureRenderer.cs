using SharpFont;
using System;
using System.Drawing;
using TryFreetype.Model;
using Point = TryFreetype.Model.Point;

namespace TryFreetype
{
    public class FigureRenderer
    {
        private readonly Bitmap bitmap;

        double x, y;
        Graphics g;
        Pen[] pens;
        Pen pen;

        private Figure figure;

        public Bitmap Bitmap { get { return bitmap; } }

        public FigureRenderer(Figure figure)
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
            //g.TranslateTransform(0, -103);
            g.TranslateTransform(0, -(height - 1));
            pens = new Pen[4]
                {
                    new Pen(Color.Red),
                    new Pen(Color.Blue),
                    new Pen(Color.Yellow),
                    new Pen(Color.Green)
                };
            pen = pens[0];
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

                    pen = pens[i % pens.Length];

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

#if !false
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
#endif
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
            return 0;
        }

        private int CubicToFunc(Edge edge)
        {
            var control1 = ((CubicEdge) edge).Control1;
            var control2 = ((CubicEdge) edge).Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            return 0;
        }
    }
}
