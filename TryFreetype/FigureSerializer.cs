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
        private Dictionary<Contour, int> _contourToId = new Dictionary<Contour, int>();

        internal void Serialize()
        {
            // Figure, PointGroups, Contours, Points, Edges, OriginalEdges, Shapes

            CollectObjects();

            _writer.WriteLine("figure begin");

            WriteFields();
            WritePointGroups();
            WriteContours();
            WriteOriginalEdges();
            WriteCuts();
            WriteShapes();

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
            int id = _contourToId[contour];

            _writer.WriteLine("  {0} contour begin", id);

            WritePoints(contour);
            WriteEdges(contour);

            _writer.WriteLine("  end");
        }

        private void WritePoints(Contour contour)
        {
            Edge edge = contour.FirstPoint.OutgoingEdge;

            // Sort points by ID.
            var sortedTable = new SortedDictionary<int, Point>();

            sortedTable.Add(edge.P1.Id, edge.P1);

            while (edge.P2 != contour.FirstPoint)
            {
                sortedTable.Add(edge.P2.Id, edge.P2);

                edge = edge.P2.OutgoingEdge;
            }

            foreach (var pair in sortedTable)
            {
                WritePoint(pair.Key, pair.Value);
            }
        }

        private void WritePoint(int id, Point point)
        {
            int groupId = _pointGroupToId[point.Group];
            _writer.WriteLine("    {0} point {1} {2} {3}", id, point.X, point.Y, groupId);
        }

        private void WriteEdges(Contour contour)
        {
            Edge edge = contour.FirstPoint.OutgoingEdge;

            // Sort edges by ID of P1.
            var sortedTable = new SortedDictionary<int, Edge>();

            while (true)
            {
                int id0 = edge.P1.Id;

                sortedTable.Add(id0, edge);

                edge = edge.P2.OutgoingEdge;

                if (edge.P1 == contour.FirstPoint)
                    break;
            }

            foreach (var pair in sortedTable)
            {
                _writer.Write("    edge");

                int id0, id1;
                Point c1, c2;
                bool unbreakable;

                edge = pair.Value;
                id0 = pair.Key;
                id1 = edge.P2.Id;

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
            }
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
                int idE1P1 = cut.PairedEdge1.P1.Id;
                int idE1P2 = cut.PairedEdge1.P2.Id;
                int idE2P1 = cut.PairedEdge2.P1.Id;
                int idE2P2 = cut.PairedEdge2.P2.Id;

                _writer.WriteLine( "  cut {0} {1} {2} {3}", idE1P1, idE1P2, idE2P1, idE2P2 );
            }
        }

        private void WriteShapes()
        {
            foreach (var shape in _figure.Shapes)
            {
                WriteShape(shape);
            }
        }

        private void WriteShape(Shape shape)
        {
            _writer.WriteLine("  shape begin");

            _writer.WriteLine("    enabled {0}", shape.Enabled ? 1 : 0);

            foreach (var contour in shape.Contours)
            {
                int id = _contourToId[contour];

                _writer.WriteLine("    contour {0}", id);
            }

            _writer.WriteLine("  end");
        }

        private int GetIdForPointGroup(PointGroup pointGroup)
        {
            return _pointGroupToId.Count;
        }

        private int GetIdForPoint(Point point)
        {
            return point.Id;
        }

        private int GetIdForContour(Contour contour)
        {
            return _contourToId.Count;
        }

        private void CollectObjects()
        {
            foreach (var pointGroup in _figure.PointGroups)
            {
                _pointGroupToId.Add(pointGroup, GetIdForPointGroup(pointGroup));
            }

            foreach (var contour in _figure.Contours)
            {
                _contourToId.Add(contour, GetIdForContour(contour));
            }
        }

        public static void Serialize(Figure figure, TextWriter writer)
        {
            var serializer = new FigureSerializer(figure, writer);

            serializer.Serialize();
        }
    }
}
