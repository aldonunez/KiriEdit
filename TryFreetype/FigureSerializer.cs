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

            _writer.WriteLine("figure");

            WritePointGroups();
            WriteContours();
            WriteOriginalEdges();

            _writer.WriteLine("end figure");
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
            _writer.WriteLine("  {0} pointgoup", id);
            _writer.WriteLine("    isFixed {0}", pointGroup.IsFixed);
            _writer.WriteLine("  end pointgroup");
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
            _writer.WriteLine("  contour");

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
                _writer.WriteLine("    edge");

                int id0, id1, id2, id3;

                id0 = _pointToId[edge.P1];

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        id1 = _pointToId[edge.P2];
                        _writer.WriteLine("      type line");
                        _writer.WriteLine("      points {0} {1}", id0, id1);
                        break;

                    case EdgeType.Conic:
                        id1 = _pointToId[((ConicEdge) edge).Control1];
                        id2 = _pointToId[edge.P2];
                        _writer.WriteLine("      type conic");
                        _writer.WriteLine("      points {0} {1} {2}", id0, id1, id2);
                        WritePointValue(((ConicEdge) edge).Control1);
                        break;

                    case EdgeType.Cubic:
                        id1 = _pointToId[((CubicEdge) edge).Control1];
                        id2 = _pointToId[((CubicEdge) edge).Control2];
                        id3 = _pointToId[edge.P2];
                        _writer.WriteLine("      type cubic");
                        _writer.WriteLine("      points {0} {1} {2} {3}", id0, id1, id2, id3);
                        WritePointValue(((CubicEdge) edge).Control1);
                        WritePointValue(((CubicEdge) edge).Control2);
                        break;
                }

                _writer.WriteLine("    end edge");

                edge = edge.P2.OutgoingEdge;

                if (edge.P1 == contour.FirstPoint)
                    break;
            }

            _writer.WriteLine("  end contour");
        }

        private void WriteOriginalEdges()
        {
            foreach (var pointGroup in _figure.PointGroups)
            {
                if (!pointGroup.IsFixed)
                    continue;

                _writer.WriteLine("  original-edge");

                Edge edge = pointGroup.Points[0].OriginalOutgoingEdge;

                int id0, id1, id2, id3;

                id0 = _pointGroupToId[pointGroup];

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        id1 = _pointGroupToId[edge.P2.Group];
                        _writer.WriteLine("    type line");
                        _writer.WriteLine("    pointgroups {0} {1}", id0, id1);
                        break;

                    case EdgeType.Conic:
                        id2 = _pointGroupToId[edge.P2.Group];
                        _writer.WriteLine("    type conic");
                        _writer.WriteLine("    pointgroups {0} {1}", id0, id2);
                        WritePointValue(((ConicEdge) edge).Control1);
                        break;

                    case EdgeType.Cubic:
                        id3 = _pointGroupToId[edge.P2.Group];
                        _writer.WriteLine("    type cubic");
                        _writer.WriteLine("    pointgroups {0} {1}", id0, id3);
                        WritePointValue(((CubicEdge) edge).Control1);
                        WritePointValue(((CubicEdge) edge).Control2);
                        break;
                }

                _writer.WriteLine("  end original-edge");
            }
        }

        private void WritePoint(Point point)
        {
            int id = _pointToId[point];
            int groupId = _pointGroupToId[point.Group];
            _writer.WriteLine("    {0} point {1} {2}", id, point.X, point.Y);
            _writer.WriteLine("      group {0}", groupId);
            _writer.WriteLine("    end point");
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
