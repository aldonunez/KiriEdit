/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;
using System;
using System.Collections.Generic;
using KiriFig.Model;
using FaceOrientation = KiriFig.Model.FaceOrientation;

namespace KiriFig
{
    internal class GlyphWalker
    {
        private Face _face;
        private FaceOrientation _faceOrientation;
        private Contour _curContour;
        private Point _curPoint;
        private int _x, _y;
        private int _nextEdge;

        private List<Contour> _contours = new List<Contour>();
        private List<PointGroup> _pointGroups = new List<PointGroup>();

        public Figure Figure { get; private set; }

        public GlyphWalker( Face face )
        {
            this._face = face;
            _faceOrientation = (FaceOrientation) Face.GetOrientation( _face.Format );
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

            _face.Decompose( outlineFuncs );

            CloseCurrentContour();
            AssignShapes();

            var faceBbox = _face.GetFaceBBox();
            var bbox = _face.GetBBox();
            var metrics = _face.GetMetrics();

            int width = ((metrics.Width + 63) / 64) * 64;
            int height = ((faceBbox.Top - faceBbox.Bottom + 63) / 64) * 64;

            int offsetX = bbox.Left;
            int offsetY = faceBbox.Bottom;

            Figure = new Figure( _pointGroups, new Cut[0], width, height, offsetX, offsetY, _faceOrientation );
        }

        private void AssignShapes()
        {
            var tool = new OutlineTool(_contours, _faceOrientation);
            var shapes = tool.CalculateShapes();

            foreach ( var shape in shapes )
            {
                foreach ( var contour in shape.Contours )
                {
                    contour.Shape = shape;
                }
            }
        }

        private void CloseCurrentContour()
        {
            if ( _curContour == null )
                return;

            if ( _curContour.FirstPoint == _curPoint )
            {
                _contours.Remove( _curContour );
                _pointGroups.Remove( _curPoint.Group );
                return;
            }

            Point firstPoint = _curContour.FirstPoint;
            Point lastPoint = _curPoint;

            _pointGroups.Remove( lastPoint.Group );

            firstPoint.IncomingEdge = lastPoint.IncomingEdge;
            firstPoint.IncomingEdge.P2 = firstPoint;
            firstPoint.Group.OriginalIncomingEdge = firstPoint.IncomingEdge;

            _curContour = null;
            _curPoint = null;
        }

        private int MoveToFunc( ref FTVector to, IntPtr user )
        {
            CloseCurrentContour();

            _x = to.X;
            _y = to.Y;

            var newPoint = new Point(_x, _y);

            var newContour = new Contour();
            _contours.Add( newContour );

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add( newPoint );
            newPoint.Group = newGroup;
            _pointGroups.Add( newGroup );

            _curPoint = newPoint;
            _curContour = newContour;

            _curContour.FirstPoint = newPoint;
            newPoint.Contour = _curContour;

            return 0;
        }

        private int LineToFunc( ref FTVector to, IntPtr user )
        {
            if ( to.X == _curPoint.X && to.Y == _curPoint.Y )
                return 0;

            _x = to.X;
            _y = to.Y;

            var newPoint = new Point(_x, _y);

            var edge = new LineEdge(_curPoint, newPoint, _nextEdge++);
            _curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add( newPoint );
            newPoint.Group = newGroup;
            _pointGroups.Add( newGroup );

            _curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            _curPoint = newPoint;

            newPoint.Contour = _curContour;

            return 0;
        }

        private int ConicToFunc( ref FTVector control, ref FTVector to, IntPtr user )
        {
            _x = to.X;
            _y = to.Y;
            int controlX = control.X;
            int controlY = control.Y;

            var newPoint = new Point(_x, _y);
            var controlPoint = new Point(controlX, controlY);

            var edge = new ConicEdge(_curPoint, controlPoint, newPoint, _nextEdge++);
            _curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add( newPoint );
            newPoint.Group = newGroup;
            _pointGroups.Add( newGroup );

            _curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            _curPoint = newPoint;

            newPoint.Contour = _curContour;

            return 0;
        }

        private int CubicToFunc( ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user )
        {
            _x = to.X;
            _y = to.Y;
            int controlX1 = control1.X;
            int controlY1 = control1.Y;
            int controlX2 = control2.X;
            int controlY2 = control2.Y;

            var newPoint = new Point(_x, _y);
            var controlPoint1 = new Point(controlX1, controlY1);
            var controlPoint2 = new Point(controlX2, controlY2);

            var edge = new CubicEdge(_curPoint, controlPoint1, controlPoint2, newPoint, _nextEdge++);
            _curPoint.OutgoingEdge = edge;
            newPoint.IncomingEdge = edge;

            var newGroup = new PointGroup(isFixed: true);
            newGroup.Points.Add( newPoint );
            newPoint.Group = newGroup;
            _pointGroups.Add( newGroup );

            _curPoint.Group.OriginalOutgoingEdge = edge;
            newPoint.Group.OriginalIncomingEdge = edge;

            _curPoint = newPoint;

            newPoint.Contour = _curContour;

            return 0;
        }
    }
}
