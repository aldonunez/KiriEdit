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
#if false
            Point p1 = new Point { X = 10, Y = 20 };
            Point p2 = new Point { X = p1.X + 1000000, Y = p1.Y + 1000000 };
            LineEdge lineEdge = new LineEdge { P1 = p1, P2 = p2 };

            var result = lineEdge.Split(new Point { X = 13, Y = 24 });
            return;
#endif

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

            if (curContour.FirstPoint == null || curContour.FirstPoint == curPoint)
            {
                figure.Contours.Remove(curContour);
                return;
            }

            Point firstPoint = curContour.FirstPoint;
            Point lastPoint = curPoint;

            figure.PointGroups.Remove(lastPoint.Group);

            firstPoint.IncomingEdge = lastPoint.IncomingEdge;
            firstPoint.IncomingEdge.P2 = firstPoint;

            curContour = null;
            curPoint = null;
        }

        private int MoveToFunc(ref FTVector to, IntPtr user)
        {
            CloseCurrentContour();

            x = to.X.Value / 64.0;
            y = to.Y.Value / 64.0;
            Console.WriteLine("MoveTo: {0}, {1}", x, y);

            var newPoint = new Point { X = x, Y = y };

            var newContour = new Contour();
            figure.Contours.Add(newContour);

            var newGroup = new PointGroup();
            newGroup.IsFixed = true;
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            figure.PointGroups.Add(newGroup);

            curPoint = newPoint;
            curContour = newContour;

            curContour.FirstPoint = newPoint;
            newPoint.Contour = curContour;

            return 0;
        }

        private int LineToFunc(ref FTVector to, IntPtr user)
        {
            x = to.X.Value / 64.0;
            y = to.Y.Value / 64.0;
            Console.WriteLine("LineTo: {0}, {1}", x, y);

            var newPoint = new Point { X = x, Y = y };

            var edge = new LineEdge { P1 = curPoint, P2 = newPoint };
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;
            curPoint.OriginalOutgoingEdge = edge;
            newPoint.OriginalIncomingEdge = edge;

            var newGroup = new PointGroup();
            newGroup.IsFixed = true;
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            figure.PointGroups.Add(newGroup);

            curPoint = newPoint;

            newPoint.Contour = curContour;

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

        public Point AddDiscardablePoint(Point point, Edge edge)
        {
            var splitResult = edge.Split(point);

            edge.P1.OutgoingEdge = splitResult.edgeBefore;
            edge.P2.IncomingEdge = splitResult.edgeAfter;

            var newGroup = new PointGroup();
            newGroup.IsFixed = false;
            newGroup.Points.Add(splitResult.nearestPoint);
            splitResult.nearestPoint.Group = newGroup;
            splitResult.nearestPoint.Contour = edge.P1.Contour;
            PointGroups.Add(newGroup);

            return splitResult.nearestPoint;
        }

        public void DeleteDiscardablePoint(PointGroup pointGroup)
        {
            if (pointGroup.IsFixed)
                throw new ApplicationException("Can't delete a fixed point group.");

            if (pointGroup.Points.Count > 1)
                throw new ApplicationException("Can't delete a point group that has more than one point.");

            Point pointToDelete = pointGroup.Points[0];

            // Find the original edge and endpoints.

            Point fixedPointBefore = pointToDelete;

            while (!fixedPointBefore.Group.IsFixed)
            {
                fixedPointBefore = fixedPointBefore.IncomingEdge.P1;
            }

            Point fixedPointAfter = fixedPointBefore.OriginalOutgoingEdge.P2;

            // Collect the points between the endpoints.

            Edge directEdge = (Edge) fixedPointBefore.OriginalOutgoingEdge.Clone();
            Edge firstEdgeToReplace = fixedPointBefore.OutgoingEdge;

            // Replace the path between the original endpoints with the original edge.

            fixedPointBefore.OutgoingEdge = directEdge;
            fixedPointAfter.IncomingEdge = directEdge;

            // Add the points we collected except for the one to remove.

            var edge = directEdge;

            for ( Point p = firstEdgeToReplace.P2; p != fixedPointAfter; p = p.OutgoingEdge.P2 )
            {
                if (p == pointToDelete)
                    continue;

                var splitResult = edge.Split(p);

                edge.P1.OutgoingEdge = splitResult.edgeBefore;
                edge.P2.IncomingEdge = splitResult.edgeAfter;

                edge = splitResult.edgeAfter;
            }

            PointGroups.Remove(pointGroup);
        }

        public void AddCut(Point point1, Point point2)
        {
            // Only allow one cut between the same point groups.

            // TODO: Do this in the caller.
            // Find a point in each group that share the same contour: old points

            // Add a point to each point group: new points

            Point newPoint1 = point1.Group.MakePoint();
            point1.Group.Points.Add(newPoint1);

            Point newPoint2 = point2.Group.MakePoint();
            point2.Group.Points.Add(newPoint2);

            // Move half the edges from the old points to the new points.

            newPoint1.IncomingEdge = point1.IncomingEdge;
            point1.IncomingEdge = null;

            newPoint2.OutgoingEdge = point2.OutgoingEdge;
            point2.OutgoingEdge = null;

            // Add a line edge between old points and between new points.

            LineEdge lineForNew = new LineEdge();
            lineForNew.P1 = newPoint1;
            lineForNew.P2 = newPoint2;

            LineEdge lineForOld = new LineEdge();
            lineForOld.P1 = point2;
            lineForOld.P2 = point1;

            newPoint1.OutgoingEdge = lineForNew;
            newPoint2.IncomingEdge = lineForNew;

            point1.IncomingEdge = lineForOld;
            point2.OutgoingEdge = lineForOld;

            // Split or combine contours.

            ModifyContours(point1, newPoint1);

            // Save cut object.
        }

        private void ModifyContours(Point point1, Point point2)
        {
            Point p = point1.OutgoingEdge.P2;

            while (true)
            {
                if (p == point1)
                {
                    // The points are in different contours.
                }
                else if (p == point2)
                {
                    // The points are in the same contour.
                }

                p = p.OutgoingEdge.P2;
            }
        }

        private Contour FindContour(Point childPoint)
        {
            foreach (var contour in Contours)
            {
                Point p = contour.FirstPoint;
                bool found = false;

                do
                {
                    if (p == childPoint)
                    {
                        found = true;
                        break;
                    }

                    p = p.OutgoingEdge.P2;

                } while (p != contour.FirstPoint);

                if (found)
                    return contour;
            }

            return null;
        }
    }

    public class Contour
    {
        public Point FirstPoint;
    }

    public enum EdgeType
    {
        Line,
        Conic,
        Cubic
    }

    public abstract class Edge : ICloneable
    {
        public EdgeType Type { get; protected set; }
        //public Edge Companion;
        public Point P1;
        public Point P2;

        internal abstract (Point nearestPoint, Edge edgeBefore, Edge edgeAfter) Split(Point point);
        public abstract object Clone();
    }

    public class LineEdge : Edge
    {
        public LineEdge()
        {
            Type = EdgeType.Line;
        }

        public override object Clone()
        {
            LineEdge newEdge = new LineEdge
            {
                P1 = P1,
                P2 = P2
            };

            return newEdge;
        }

        internal override (Point nearestPoint, Edge edgeBefore, Edge edgeAfter) Split(Point point)
        {
            var valueNearestPoint = FindNearestPoint(point, P1.ToValuePoint(), P2.ToValuePoint());

            var nearestPoint = new Point(valueNearestPoint);
            var edgeBefore = new LineEdge { P1 = P1, P2 = nearestPoint };
            var edgeAfter = new LineEdge { P1 = nearestPoint, P2 = P2 };

            nearestPoint.IncomingEdge = edgeBefore;
            nearestPoint.OutgoingEdge = edgeAfter;

            return (nearestPoint, edgeBefore, edgeAfter);
        }

        private ValuePoint FindNearestPoint(Point point, ValuePoint p1, ValuePoint p2)
        {
            ValuePoint valuePoint = point.ToValuePoint();
            double distP1 = Math.Abs(valuePoint.GetDistance(p1));
            double distP2 = Math.Abs(valuePoint.GetDistance(p2));
            double distP1P2 = Math.Abs(p1.GetDistance(p2));

            //Console.WriteLine("Checking {0}, {1} (dP1={2}, dP2={3}, dP1P2={4}", p1, p2, distP1, distP2, distP1P2);

            if (distP1P2 <= 1.0)
            {
                if (distP1 <= distP2)
                {
                    return p1;
                }
                else
                {
                    return p2;
                }
            }

            ValuePoint midPoint = new ValuePoint();

            midPoint.X = (p2.X + p1.X) / 2;
            midPoint.Y = (p2.Y + p1.Y) / 2;

            if (distP1 <= distP2)
            {
                return FindNearestPoint(point, p1, midPoint);
            }
            else
            {
                return FindNearestPoint(point, midPoint, p2);
            }
        }
    }

    public class ConicEdge : Edge
    {
        public Point Control1;

        public ConicEdge()
        {
            Type = EdgeType.Conic;
        }

        public override object Clone()
        {
            ConicEdge newEdge = new ConicEdge
            {
                P1 = P1,
                P2 = P2,
                Control1 = Control1
            };

            return newEdge;
        }

        internal override (Point nearestPoint, Edge edgeBefore, Edge edgeAfter) Split(Point point)
        {
            throw new NotImplementedException();
        }
    }

    public class CubicEdge : Edge
    {
        public Point Control1;
        public Point Control2;

        public CubicEdge()
        {
            Type = EdgeType.Cubic;
        }

        public override object Clone()
        {
            CubicEdge newEdge = new CubicEdge
            {
                P1 = P1,
                P2 = P2,
                Control1 = Control1,
                Control2 = Control2
            };

            return newEdge;
        }

        internal override (Point nearestPoint, Edge edgeBefore, Edge edgeAfter) Split(Point point)
        {
            throw new NotImplementedException();
        }
    }

    public class PointGroup
    {
        public bool IsFixed;
        public List<Point> Points { get; } = new List<Point>();

        public Point MakePoint()
        {
            Point p = new Point(Points[0].X, Points[0].Y);
            p.Group = this;

            return p;
        }
    }

    public class Point
    {
        public double X;
        public double Y;
        public PointGroup Group;
        public bool IsFixed;
        public Edge OutgoingEdge;
        public Edge IncomingEdge;
        public Edge OriginalOutgoingEdge;
        public Edge OriginalIncomingEdge;
        public Contour Contour;

        public Point()
        {
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        internal Point(ValuePoint valuePoint)
        {
            X = valuePoint.X;
            Y = valuePoint.Y;
        }

        internal ValuePoint ToValuePoint()
        {
            return new ValuePoint { X = X, Y = Y };
        }
    }

    internal struct ValuePoint
    {
        public double X;
        public double Y;

        internal double GetDistance(ValuePoint otherPoint)
        {
            double w = otherPoint.X - X;
            double h = otherPoint.Y - Y;
            return Math.Sqrt(w * w + h * h);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }

    class FigureRenderer
    {
        private readonly Bitmap bitmap;

        double x, y;
        Graphics g;
        Pen[] pens;
        Pen pen;

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
            Point p6 = figure.PointGroups[6].Points[0];
            Point p1 = new Point { X = (p6.X + p6.OutgoingEdge.P2.X) / 2, Y = p6.Y };
            var e = figure.PointGroups[6].Points[0].OutgoingEdge;
            var midPoint = figure.AddDiscardablePoint( p1, e );

            figure.DeleteDiscardablePoint(midPoint.Group);


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
            Pen bluePen = new Pen(Color.Blue);
            int j = 0;

            foreach (var group in figure.PointGroups)
            {
                Point p = group.Points[0];
                Pen pen = null;
                float radius = 5f;

                if (group.IsFixed)
                {
                    pen = redPen;
                }
                else
                {
                    pen = bluePen;
                }
                j++;

                g.DrawEllipse(
                    pen,
                    (float) p.X - radius,
                    (float) p.Y - radius,
                    radius * 2,
                    radius * 2
                    );
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
