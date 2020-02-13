using SharpFont;
using System;
using System.Collections.Generic;
using TryFreetype.Model;

namespace TryFreetype
{
    public class GlyphWalker
    {
        private GlyphSlot glyphSlot;
        private Figure figure;
        private Contour curContour;
        private Point curPoint;

        double x, y;

        private List<Contour> _contours = new List<Contour>();
        private List<PointGroup> _pointGroups = new List<PointGroup>();

        public Figure Figure { get { return figure; } }

        public GlyphWalker(FontGlyph glyph) :
            this(glyph.Glyph)
        {
        }

        public GlyphWalker(GlyphSlot glyphSlot)
        {
            this.glyphSlot = glyphSlot;

            var bbox = glyphSlot.Outline.GetBBox();
            int width = glyphSlot.Metrics.Width.Ceiling();
            int height = glyphSlot.Metrics.Height.Ceiling();
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

            int width = glyphSlot.Metrics.Width.Ceiling();
            int height = glyphSlot.Metrics.Height.Ceiling();

            figure = new Figure(_pointGroups, width, height);
        }

        private void CloseCurrentContour()
        {
            if (curContour == null)
                return;

            if (curContour.FirstPoint == null || curContour.FirstPoint == curPoint)
            {
                _contours.Remove(curContour);
                return;
            }

            Point firstPoint = curContour.FirstPoint;
            Point lastPoint = curPoint;

            _pointGroups.Remove(lastPoint.Group);

            firstPoint.IncomingEdge = lastPoint.IncomingEdge;
            firstPoint.IncomingEdge.P2 = firstPoint;
            firstPoint.OriginalIncomingEdge = firstPoint.IncomingEdge;

            curContour = null;
            curPoint = null;
        }

        private int MoveToFunc(ref FTVector to, IntPtr user)
        {
            CloseCurrentContour();

            x = to.X.Value / 64.0;
            y = to.Y.Value / 64.0;
            Console.WriteLine("MoveTo: {0}, {1}", x, y);

            var newPoint = new Point(x, y);

            var newContour = new Contour();
            _contours.Add(newContour);

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            _pointGroups.Add(newGroup);

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

            var newPoint = new Point(x, y);

            var edge = new LineEdge { P1 = curPoint, P2 = newPoint };
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;
            curPoint.OriginalOutgoingEdge = edge;
            newPoint.OriginalIncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            _pointGroups.Add(newGroup);

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
}
