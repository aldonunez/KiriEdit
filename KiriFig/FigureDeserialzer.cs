﻿/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using KiriFig.Model;

namespace KiriFig
{
    internal class FigureParser : Parser
    {
        private enum Node
        {
            None,
            Figure,
            Contour,
            Shape,
        }

        private int _level;
        private Node[] _nodeStack = new Node[3];

        private Dictionary<int, PointGroup> _pointGroups = new Dictionary<int, PointGroup>();
        private Dictionary<int, Point> _points = new Dictionary<int, Point>();
        private Dictionary<int, Contour> _contours = new Dictionary<int, Contour>();
        private List<Cut> _cuts = new List<Cut>();
        private Contour _curContour;
        private Shape _curShape;
        private int _width;
        private int _height;
        private int _offsetX;
        private int _offsetY;
        private FaceOrientation _faceOrientation;

        public Figure Figure { get; private set; }

        public FigureParser( TextReader reader ) :
            base( reader )
        {
            _nodeStack[0] = Node.None;
        }

        private void PushLevel( Node node )
        {
            if ( _level == _nodeStack.Length - 1 )
                throw new ApplicationException();

            _level++;
            _nodeStack[_level] = node;
        }

        private void PopLevel()
        {
            if ( _level == 0 )
                throw new ApplicationException();

            _level--;
        }

        // An open record has more than one line. The first line ends with "begin".

        protected override void OnBeginRecord( int id, string head, IList<Token> attrs, bool open )
        {
            Debug.WriteLine( "{2} {0} ({1})", head, id, open ? "begin" : "record" );

            switch ( _nodeStack[_level] )
            {
                case Node.None:
                    if ( head != "figure" || !open )
                        throw new ApplicationException();

                    PushLevel( Node.Figure );
                    break;

                case Node.Figure:
                    HandleRecordInFigure( id, head, attrs, open );
                    break;

                case Node.Contour:
                    HandleRecordInContour( id, head, attrs, open );
                    break;

                case Node.Shape:
                    HandleRecordInShape( id, head, attrs, open );
                    break;
            }
        }

        private void HandleRecordInFigure( int id, string head, IList<Token> attrs, bool open )
        {
            bool stayOpen = false;

            if ( head == "width" )
            {
                if ( attrs.Count != 1 )
                    throw new ApplicationException();

                _width = attrs[0].GetInteger();
            }
            else if ( head == "height" )
            {
                if ( attrs.Count != 1 )
                    throw new ApplicationException();

                _height = attrs[0].GetInteger();
            }
            else if ( head == "offsetx" )
            {
                if ( attrs.Count != 1 )
                    throw new ApplicationException();

                _offsetX = attrs[0].GetInteger();
            }
            else if ( head == "offsety" )
            {
                if ( attrs.Count != 1 )
                    throw new ApplicationException();

                _offsetY = attrs[0].GetInteger();
            }
            else if ( head == "faceOrientation" )
            {
                if ( attrs.Count != 1 )
                    throw new ApplicationException();

                _faceOrientation = (FaceOrientation) attrs[0].GetInteger();
            }
            else if ( head == "pointgroup" )
            {
                if ( attrs.Count < 1 )
                    throw new ApplicationException();

                bool isFixed = attrs[0].GetInteger() != 0;
                PointGroup pointGroup;

                if ( isFixed )
                {
                    pointGroup = new PointGroup( isFixed );
                }
                else
                {
                    if ( attrs.Count != 2 )
                        throw new ApplicationException();

                    int normalT = attrs[1].GetInteger();
                    pointGroup = new PointGroup( normalT );
                }

                _pointGroups.Add( id, pointGroup );
            }
            else if ( head == "contour" )
            {
                PushLevel( Node.Contour );
                _curContour = new Contour();
                stayOpen = true;

                _contours.Add( id, _curContour );
            }
            else if ( head == "original-edge" )
            {
                if ( attrs.Count < 3 )
                    throw new ApplicationException();

                string type = attrs[0].GetWord();
                int id0 = attrs[1].GetInteger();
                int id1 = attrs[2].GetInteger();
                PointGroup pg0 = _pointGroups[id0];
                PointGroup pg1 = _pointGroups[id1];
                Edge edge;

                switch ( type )
                {
                    case "line":
                        {
                            LineEdge lineEdge = new LineEdge( pg0.Points[0], pg1.Points[0], id );
                            edge = lineEdge;
                        }
                        break;

                    case "conic":
                        {
                            int c1X = attrs[3].GetInteger();
                            int c1Y = attrs[4].GetInteger();
                            Point c1 = new Point( c1X, c1Y );
                            ConicEdge conicEdge = new ConicEdge( pg0.Points[0], c1, pg1.Points[0], id );
                            edge = conicEdge;
                        }
                        break;

                    case "cubic":
                        {
                            int c1X = attrs[3].GetInteger();
                            int c1Y = attrs[4].GetInteger();
                            int c2X = attrs[5].GetInteger();
                            int c2Y = attrs[6].GetInteger();
                            Point c1 = new Point( c1X, c1Y );
                            Point c2 = new Point( c2X, c2Y );
                            CubicEdge cubicEdge = new CubicEdge( pg0.Points[0], c1, c2, pg1.Points[0], id );
                            edge = cubicEdge;
                        }
                        break;

                    default:
                        throw new ApplicationException();
                }

                pg0.OriginalOutgoingEdge = edge;
                pg1.OriginalIncomingEdge = edge;
            }
            else if ( head == "cut" )
            {
                if ( attrs.Count < 4 )
                    throw new ApplicationException();

                int idE1P1 = attrs[0].GetInteger();
                int idE1P2 = attrs[1].GetInteger();
                int idE2P1 = attrs[2].GetInteger();
                int idE2P2 = attrs[3].GetInteger();
                Point p0 = _points[idE1P1];
                Point p1 = _points[idE1P2];
                Point p2 = _points[idE2P1];
                Point p3 = _points[idE2P2];

                Edge edge1 = p0.OutgoingEdge;
                Edge edge2 = p2.OutgoingEdge;

                if ( edge1.Type != EdgeType.Line
                    || edge2.Type != EdgeType.Line )
                    throw new ApplicationException();

                if ( edge1.P2 != p1 || edge2.P2 != p3 )
                    throw new ApplicationException();

                Cut cut = new Cut( (LineEdge) edge1, (LineEdge) edge2 );

                _cuts.Add( cut );
            }
            else if ( head == "shape" )
            {
                PushLevel( Node.Shape );
                _curShape = new Shape();
                stayOpen = true;
            }
            else
            {
                throw new ApplicationException();
            }

            if ( stayOpen != open )
                throw new ApplicationException();
        }

        private void HandleRecordInContour( int id, string head, IList<Token> attrs, bool open )
        {
            if ( head == "point" )
            {
                if ( attrs.Count < 3 )
                    throw new ApplicationException();

                int x = attrs[0].GetInteger();
                int y = attrs[1].GetInteger();
                int groupId = attrs[2].GetInteger();
                Point point = new Point( x, y );
                PointGroup group = _pointGroups[groupId];

                _points.Add( id, point );

                point.Group = group;
                point.Contour = _curContour;
                group.Points.Add( point );

                if ( _curContour.FirstPoint == null )
                    _curContour.FirstPoint = point;
            }
            else if ( head == "edge" )
            {
                if ( attrs.Count < 4 )
                    throw new ApplicationException();

                string type = attrs[0].GetWord();
                int id0 = attrs[1].GetInteger();
                int id1 = attrs[2].GetInteger();
                Point p0 = _points[id0];
                Point p1 = _points[id1];
                Edge edge;

                switch ( type )
                {
                    case "line":
                        {
                            bool unbreakable = attrs[3].GetInteger() != 0;
                            LineEdge lineEdge = new LineEdge( p0, p1, id, unbreakable );
                            edge = lineEdge;
                        }
                        break;

                    case "conic":
                        {
                            int c1X = attrs[3].GetInteger();
                            int c1Y = attrs[4].GetInteger();
                            Point c1 = new Point( c1X, c1Y );
                            ConicEdge conicEdge = new ConicEdge( p0, c1, p1, id );
                            edge = conicEdge;
                        }
                        break;

                    case "cubic":
                        {
                            int c1X = attrs[3].GetInteger();
                            int c1Y = attrs[4].GetInteger();
                            int c2X = attrs[5].GetInteger();
                            int c2Y = attrs[6].GetInteger();
                            Point c1 = new Point( c1X, c1Y );
                            Point c2 = new Point( c2X, c2Y );
                            CubicEdge cubicEdge = new CubicEdge( p0, c1, c2, p1, id );
                            edge = cubicEdge;
                        }
                        break;

                    default:
                        throw new ApplicationException();
                }

                p0.OutgoingEdge = edge;
                p1.IncomingEdge = edge;
            }
            else
            {
                throw new ApplicationException();
            }

            if ( open )
                throw new ApplicationException();
        }

        private void HandleRecordInShape( int id, string head, IList<Token> attrs, bool open )
        {
            if ( head == "contour" )
            {
                if ( attrs.Count < 1 )
                    throw new ApplicationException();

                int contourId = attrs[0].GetInteger();
                Contour contour = _contours[contourId];

                _curShape.Contours.Add( contour );
                contour.Shape = _curShape;
            }
            else if ( head == "enabled" )
            {
                if ( attrs.Count < 1 )
                    throw new ApplicationException();

                _curShape.Enabled = (attrs[0].GetInteger() != 0);
            }
            else
            {
                throw new ApplicationException();
            }

            if ( open )
                throw new ApplicationException();
        }

        protected override void OnEndRecord()
        {
            Debug.WriteLine( "end" );

            PopLevel();
        }

        internal void Deserialize()
        {
            Parse();

            Figure = new Figure(
                _pointGroups.Values,
                _cuts,
                _width, _height, _offsetX, _offsetY,
                _faceOrientation );
        }
    }

    public class FigureDeserialzer
    {
        public static Figure Deserialize( TextReader reader )
        {
            var deserializer = new FigureParser( reader );

            deserializer.Deserialize();

            return deserializer.Figure;
        }
    }
}
