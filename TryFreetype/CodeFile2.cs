using SharpFont;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TryFreetype.Sample2
{
    static class Sample
    {
        public static void Run()
        {
            using (Library lib = new Library())
            {
                using (var face = new Face(lib, @"C:\Windows\Fonts\consola.ttf"))
                {
                    // 32 -> ?
                    face.SetPixelSizes(0, 160);
                    //face.SetCharSize(,,)

                    //FT_Face face = m_face;
                    //FT_GlyphSlot slot = face->glyph;
                    //FT_Outline &outline = slot->outline;
                    //FT_Error error = FT_Outline_Decompose(&outline, &callbacks, this);

                    face.LoadChar('A', LoadFlags.NoBitmap
                        //| LoadFlags.NoScale
                        , LoadTarget.Normal);

                    Console.WriteLine("BitmapTop = {0}", face.Glyph.BitmapTop);
                    Console.WriteLine("BitmapLeft = {0}", face.Glyph.BitmapLeft);
                    Utils.PrintProperties(face.Glyph, "");

                    {
                        GlyphWalker walker = new GlyphWalker(face.Glyph);

                        walker.Decompose();

                        FigureRenderer renderer = new FigureRenderer(face.Glyph, walker.Figure);

                        renderer.Render();

                        renderer.Bitmap.Save(@"C:\Temp\b.bmp");
                    }

                    {
                        face.Glyph.RenderGlyph(RenderMode.Normal);
                        var bmp = face.Glyph.Bitmap.ToGdipBitmap();
                        bmp.Save(@"C:\Temp\a.bmp");
                    }

                    Console.WriteLine("abc");
                }
            }
        }
    }

    class GlyphWalker
    {
        private GlyphSlot glyphSlot;
        private Figure figure = new Figure();
        private Contour curContour;
        private Point curPoint;

        double x, y;
        const double XMult = 1000;
        const double YMult = 1000;

        public Figure Figure { get { return figure; } }

        public GlyphWalker(GlyphSlot glyphSlot)
        {
            this.glyphSlot = glyphSlot;

            var bbox = glyphSlot.Outline.GetBBox();
            int width = glyphSlot.Metrics.Width.ToInt32();
            int height = glyphSlot.Metrics.Height.ToInt32();
        }

        public void Decompose()
        {
            var outlineFuncs = new OutlineFuncs
            {
                MoveFunction = MoveToFunc,
                LineFunction = LineToFunc,
                ConicFunction = ConicToFunc,
                CubicFunction = CubicToFunc,
            };
            var o = glyphSlot.Outline;
            o.Decompose(outlineFuncs, IntPtr.Zero);

            CloseCurrentContour();
        }

        private void CloseCurrentContour()
        {
            if (curContour == null)
                return;

            if (curContour.Points.Count < 3)
            {
                figure.Contours.Remove(curContour);
                return;
            }

            Point firstPoint = curContour.Points[0];
            Point lastPoint = curContour.Points[curContour.Points.Count - 1];

            curContour.Points.Remove(lastPoint);
            figure.PointGroups.Remove(lastPoint.Group);

            firstPoint.IncomingEdge = lastPoint.IncomingEdge;
            firstPoint.IncomingEdge.P2 = firstPoint;

            curContour = null;
            curPoint = null;
        }

        private int MoveToFunc(ref FTVector to, IntPtr user)
        {
            CloseCurrentContour();

            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            x = to.X.ToDouble();
            y = to.Y.ToDouble();

            var newPoint = new Point { X = x, Y = y };

            var newContour = new Contour();
            figure.Contours.Add(newContour);

            var newGroup = new PointGroup();
            newGroup.Fixed = true;
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            figure.PointGroups.Add(newGroup);

            curPoint = newPoint;
            curContour = newContour;

            curContour.Points.Add(newPoint);

            return 0;
        }

        private int LineToFunc(ref FTVector to, IntPtr user)
        {
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            x = to.X.ToDouble();
            y = to.Y.ToDouble();

            var newPoint = new Point { X = x, Y = y };

            var edge = new Edge { P1 = curPoint, P2 = newPoint, Type = EdgeType.Line };
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup();
            newGroup.Fixed = true;
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            figure.PointGroups.Add(newGroup);

            curPoint = newPoint;

            curContour.Points.Add(newPoint);

            return 0;
        }

        private int ConicToFunc(ref FTVector control, ref FTVector to, IntPtr user)
        {
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            return 0;
        }

        private int CubicToFunc(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user)
        {
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            return 0;
        }
    }


    public class Figure
    {
        public List<Contour> Contours = new List<Contour>();
        public List<PointGroup> PointGroups = new List<PointGroup>();
    }

    public class Contour
    {
        public List<Point> Points = new List<Point>();
    }

    public enum EdgeType
    {
        Line,
        Conic,
        Cubic
    }

    public class Edge
    {
        public EdgeType Type;
        public Edge Companion;
        public Point P1;
        public Point P2;
        public Point Control1;
        public Point Control2;
    }

    public class PointGroup
    {
        public bool Fixed;
        public List<Point> Points { get; } = new List<Point>();
    }

    public class Point
    {
        public double X;
        public double Y;
        public PointGroup Group;
        public Edge OutgoingEdge;
        public Edge IncomingEdge;
    }

    class FigureRenderer
    {
        private readonly Bitmap bitmap;

        double x, y;
        Graphics g;
        Pen pen;
        const double XMult = 1000;
        const double YMult = 1000;

        private Figure figure;

        public Bitmap Bitmap { get { return bitmap; } }

        public FigureRenderer(GlyphSlot glyphSlot, Figure figure)
        {
            this.figure = figure;

            var bbox = glyphSlot.Outline.GetBBox();

            int width = glyphSlot.Metrics.Width.ToInt32();
            int height = glyphSlot.Metrics.Height.ToInt32();

            bitmap = new Bitmap(width, height);
            Pen borderPen = new Pen(Color.White);

            g = Graphics.FromImage(bitmap);
            //g.DrawRectangle(borderPen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            //g.DrawEllipse(borderPen, 128, 224, 10, 10);
            g.ScaleTransform(1, -1);
            //g.TranslateTransform(0, -103);
            g.TranslateTransform(0, -(height - 1));
            pen = new Pen(Color.Red);
        }

        public void Render()
        {
            foreach (var contour in figure.Contours)
            {
                MoveToFunc(contour.Points[0]);
                for (int i = 0; i < contour.Points.Count; i++)
                {
                    var point = contour.Points[i];
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
            g.DrawLine(
                pen,
                (float) (x * XMult),
                (float) (y * YMult),
                (float) (to.X * XMult),
                (float) (to.Y * YMult));
            x = to.X;
            y = to.Y;
            return 0;
        }

        private int ConicToFunc(Edge edge)
        {
            var control = edge.Control1;
            var to = edge.P2;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            return 0;
        }

        private int CubicToFunc(Edge edge)
        {
            var control1 = edge.Control1;
            var control2 = edge.Control2;
            var to = edge.P2;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            return 0;
        }
    }
}
