using SharpFont;
using System;
using System.Collections.Generic;
using TryFreetype.Model;

namespace TryFreetype
{
    // TODO: Make this internal.
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

            var bbox = glyphSlot.Outline.GetBBox();
            int width = glyphSlot.Metrics.Width.Ceiling();
            int height = glyphSlot.Metrics.Height.Ceiling();

            figure = new Figure(_pointGroups, width, height, bbox.Left / 64.0, bbox.Bottom / 64.0);
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
            firstPoint.Group.OriginalIncomingEdge = firstPoint.IncomingEdge;

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

            var edge = new LineEdge(curPoint, newPoint);
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            _pointGroups.Add(newGroup);

            curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            curPoint = newPoint;

            newPoint.Contour = curContour;

            return 0;
        }

        private int ConicToFunc(ref FTVector control, ref FTVector to, IntPtr user)
        {
            x = to.X.Value / 64.0;
            y = to.Y.Value / 64.0;
            double controlX = control.X.Value / 64.0;
            double controlY = control.Y.Value / 64.0;
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", controlX, controlY, x, y);

            var newPoint = new Point(x, y);
            var controlPoint = new Point(controlX, controlY);

            var edge = new ConicEdge(curPoint, controlPoint, newPoint);
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            _pointGroups.Add(newGroup);

            curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            curPoint = newPoint;

            newPoint.Contour = curContour;

            return 0;
        }

        private int CubicToFunc(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user)
        {
            x = to.X.Value / 64.0;
            y = to.Y.Value / 64.0;
            double controlX1 = control1.X.Value / 64.0;
            double controlY1 = control1.Y.Value / 64.0;
            double controlX2 = control2.X.Value / 64.0;
            double controlY2 = control2.Y.Value / 64.0;
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", controlX1, controlY1, controlX2, controlY2, x, y);

            var newPoint = new Point(x, y);
            var controlPoint1 = new Point(controlX1, controlY1);
            var controlPoint2 = new Point(controlX2, controlY2);

            var edge = new CubicEdge(curPoint, controlPoint1, controlPoint2, newPoint);
            curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add(newPoint);
            newPoint.Group = newGroup;
            _pointGroups.Add(newGroup);

            curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            curPoint = newPoint;

            newPoint.Contour = curContour;

            return 0;
        }
    }
}
