/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;
using System;
using System.Collections.Generic;
using KiriFig.Model;

namespace KiriFig
{
    // TODO: Make this internal.
    public class GlyphWalker
    {
        // TODO: consistency in names
        private Face face;
        private Figure figure;
        private Contour curContour;
        private Point curPoint;

        int x, y;

        private List<Contour> _contours = new List<Contour>();
        private List<PointGroup> _pointGroups = new List<PointGroup>();

        public Figure Figure { get { return figure; } }

        public GlyphWalker(Face face)
        {
            this.face = face;
        }

        public void Decompose()
        {
            var outlineFuncs = new OutlineHandlers
            {
                MoveTo = MoveToFunc,
                LineTo = LineToFunc,
                ConicTo = ConicToFunc,
                CubicTo = CubicToFunc,
            };

            face.Decompose(outlineFuncs);

            CloseCurrentContour();
            AssignShapes();

            var faceBbox = face.GetFaceBBox();
            var bbox = face.GetBBox();
            var metrics = face.GetMetrics();

            int width = ((metrics.Width + 63) / 64) * 64;
            int height = ((faceBbox.Top - faceBbox.Bottom + 63) / 64) * 64;

            int offsetX = bbox.Left;
            int offsetY = faceBbox.Bottom;

            figure = new Figure(_pointGroups, new Cut[0], width, height, offsetX, offsetY);
        }

        private void AssignShapes()
        {
            var tool = new OutlineTool(_contours);
            var shapes = tool.CalculateShapes();

            foreach (var shape in shapes)
            {
                foreach (var contour in shape.Contours)
                {
                    contour.Shape = shape;
                }
            }
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

            x = to.X;
            y = to.Y;
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
            x = to.X;
            y = to.Y;
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
            x = to.X;
            y = to.Y;
            int controlX = control.X;
            int controlY = control.Y;
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
            x = to.X;
            y = to.Y;
            int controlX1 = control1.X;
            int controlY1 = control1.Y;
            int controlX2 = control2.X;
            int controlY2 = control2.Y;
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
