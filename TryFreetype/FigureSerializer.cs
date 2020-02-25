using System.Collections.Generic;
using System.IO;
using TryFreetype.Model;

namespace TryFreetype
{
    public class FigureSerializer
    {
        private Figure _figure;
        private TextWriter _writer;

        internal FigureSerializer(Figure figure, TextWriter writer)
        {
            _figure = figure;
            _writer = writer;
        }

        private Dictionary<PointGroup, int> _pointGroupToId = new Dictionary<PointGroup, int>();
        private Dictionary<Point, int> _pointToId = new Dictionary<Point, int>();

        internal void Serialize()
        {
            // Figure, PointGroups, Contours, Points, Edges, OriginalEdges

            CollectObjects();

            _writer.WriteLine("figure begin");

            WriteFields();
            WritePointGroups();
            WriteContours();
            WriteOriginalEdges();
            WriteCuts();

            _writer.WriteLine("end");
        }

        private void WriteFields()
        {
            _writer.WriteLine("  width {0}", _figure.Width);
            _writer.WriteLine("  height {0}", _figure.Height);
            _writer.WriteLine("  offsetx {0}", _figure.OffsetX);
            _writer.WriteLine("  offsety {0}", _figure.OffsetY);
        }

        private void WritePointGroups()
        {
            foreach (var pointGroup in _figure.PointGroups)
            {
                WritePointGroup(pointGroup);
            }
        }

        private void WritePointGroup(PointGroup pointGroup)
        {
            int id = _pointGroupToId[pointGroup];
            _writer.WriteLine("  {0} pointgroup {1}", id, pointGroup.IsFixed ? 1 : 0);
        }

        private void WriteContours()
        {
            foreach (var contour in _figure.Contours)
            {
                WriteContour(contour);
            }
        }

        private void WriteContour(Contour contour)
        {
            _writer.WriteLine("  contour begin");

            Edge edge = contour.FirstPoint.OutgoingEdge;

            WritePoint(edge.P1);

            while (edge.P2 != contour.FirstPoint)
            {
                WritePoint(edge.P2);

                edge = edge.P2.OutgoingEdge;
            }

            edge = contour.FirstPoint.OutgoingEdge;

            while (true)
            {
                _writer.Write("    edge");

                int id0, id1;
                Point c1, c2;
                bool unbreakable;

                id0 = _pointToId[edge.P1];
                id1 = _pointToId[edge.P2];

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        unbreakable = ((LineEdge) edge).Unbreakable;
                        _writer.Write(" line {0} {1} {2}", id0, id1, unbreakable ? 1 : 0);
                        break;

                    case EdgeType.Conic:
                        c1 = ((ConicEdge) edge).Control1;
                        _writer.Write(" conic {0} {1} {2} {3}", id0, id1, c1.X, c1.Y);
                        break;

                    case EdgeType.Cubic:
                        c1 = ((CubicEdge) edge).Control1;
                        c2 = ((CubicEdge) edge).Control2;
                        _writer.Write(" cubic {0} {1} {2} {3} {4} {5}", id0, id1, c1.X, c1.Y, c2.X, c2.Y);
                        break;
                }

                _writer.WriteLine();

                edge = edge.P2.OutgoingEdge;

                if (edge.P1 == contour.FirstPoint)
                    break;
            }

            _writer.WriteLine("  end");
        }

        private void WriteOriginalEdges()
        {
            foreach (var pointGroup in _figure.PointGroups)
            {
                if (!pointGroup.IsFixed)
                    continue;

                _writer.Write("  original-edge");

                Edge edge = pointGroup.OriginalOutgoingEdge;

                int id0, id1, id2, id3;
                Point c1, c2;

                id0 = _pointGroupToId[pointGroup];

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        id1 = _pointGroupToId[edge.P2.Group];
                        _writer.Write(" line {0} {1}", id0, id1);
                        break;

                    case EdgeType.Conic:
                        id2 = _pointGroupToId[edge.P2.Group];
                        c1 = ((ConicEdge) edge).Control1;
                        _writer.Write(" conic {0} {1} {2} {3}", id0, id2, c1.X, c1.Y);
                        break;

                    case EdgeType.Cubic:
                        id3 = _pointGroupToId[edge.P2.Group];
                        c1 = ((CubicEdge) edge).Control1;
                        c2 = ((CubicEdge) edge).Control2;
                        _writer.Write(" cubic {0} {1} {2} {3} {4} {5}", id0, id3, c1.X, c1.Y, c2.X, c2.Y);
                        break;
                }

                _writer.WriteLine();
            }
        }

        private void WriteCuts()
        {
            foreach ( var cut in _figure.Cuts )
            {
                int idE1P1 = _pointToId[cut.PairedEdge1.P1];
                int idE1P2 = _pointToId[cut.PairedEdge1.P2];
                int idE2P1 = _pointToId[cut.PairedEdge2.P1];
                int idE2P2 = _pointToId[cut.PairedEdge2.P2];

                _writer.WriteLine( "  cut {0} {1} {2} {3}", idE1P1, idE1P2, idE2P1, idE2P2 );
            }
        }

        private void WritePoint(Point point)
        {
            int id = _pointToId[point];
            int groupId = _pointGroupToId[point.Group];
            _writer.WriteLine("    {0} point {1} {2} {3}", id, point.X, point.Y, groupId);
        }

        private void WritePointValue(Point point)
        {
            _writer.WriteLine("      point {0} {1}", point.X, point.Y);
        }

        private int GetIdForPointGroup(PointGroup pointGroup)
        {
            return _pointGroupToId.Count;
        }

        private int GetIdForPoint(Point point)
        {
            return _pointToId.Count;
        }

        private void CollectObjects()
        {
            foreach (var pointGroup in _figure.PointGroups)
            {
                _pointGroupToId.Add(pointGroup, GetIdForPointGroup(pointGroup));

                foreach (var point in pointGroup.Points)
                {
                    _pointToId.Add(point, GetIdForPoint(point));
                }
            }
        }

        public static void Serialize(Figure figure, TextWriter writer)
        {
            var serializer = new FigureSerializer(figure, writer);

            serializer.Serialize();
        }
    }
}
