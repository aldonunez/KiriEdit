using System;
using System.Collections.Generic;
using System.Diagnostics;
using TryFreetype.Model;

namespace TryFreetype
{
    public class OutlineTool
    {
        public class Shape
        {
            public Contour OuterContour { get; }
            public Contour[] InnerContours { get; }

            public Shape(Contour outsideContour, Contour[] insideCountours)
            {
                OuterContour = outsideContour;
                InnerContours = insideCountours;
            }
        }

        private enum Orientation
        {
            Unknown,
            Inside,
            Outside
        }

        private Figure _figure;
        private OutlineRenderer _renderer;

        public OutlineTool(Figure figure)
        {
            _figure = figure;
            _renderer = new OutlineRenderer(figure);
        }

        private static Orientation GetOrientation(Contour contour)
        {
            int prevX = contour.FirstPoint.X;
            int prevY = contour.FirstPoint.Y;
            Point p = contour.FirstPoint;
            long area = 0;

            while (true)
            {
                var edge = p.OutgoingEdge;

                switch (edge.Type)
                {
                    case EdgeType.Line:
                        area += (edge.P2.Y - prevY) * (edge.P2.X + prevX);
                        break;

                    case EdgeType.Conic:
                        {
                            var conicEdge = (ConicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.P2.Y - prevY) * (conicEdge.P2.X + prevX);
                        }
                        break;

                    case EdgeType.Cubic:
                        {
                            var conicEdge = (CubicEdge) edge;
                            area += (conicEdge.Control1.Y - prevY) * (conicEdge.Control1.X + prevX);
                            area += (conicEdge.Control2.Y - prevY) * (conicEdge.Control2.X + prevX);
                            area += (conicEdge.P2.Y - prevY) * (conicEdge.P2.X + prevX);
                        }
                        break;
                }

                prevX = edge.P2.X;
                prevY = edge.P2.Y;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            if (area > 0)
                return Orientation.Inside;
            else if (area < 0)
                return Orientation.Outside;
            else
                return Orientation.Unknown;
        }

        private static Point FindRightmostPoint(Contour contour)
        {
            Point p = contour.FirstPoint;
            Point rightP = new Point(0, 0);

            while (true)
            {
                if (p.X > rightP.X)
                    rightP = p;

                p = p.OutgoingEdge.P2;

                if (p == contour.FirstPoint)
                    break;
            }

            return rightP;
        }

        private (List<Contour> outsideContours, List<Contour> insideContours) PartitionContours()
        {
            var outsideContours = new List<Contour>();
            var insideContours = new List<Contour>();

            foreach (var contour in _figure.Contours)
            {
                Orientation orientation = GetOrientation(contour);

                switch (orientation)
                {
                    case Orientation.Inside:
                        insideContours.Add(contour);
                        break;

                    case Orientation.Outside:
                        outsideContours.Add(contour);
                        break;

                    default:
                        Debug.Fail("");
                        break;
                }
            }

            return (outsideContours, insideContours);
        }

        private void DrawOutsides(List<Contour> outsideContours)
        {
            for (int i = 0; i < outsideContours.Count; i++)
            {
                var contour = outsideContours[i];
                byte color = (byte) (1 << i);

                _renderer.DrawContour(contour, color);
            }
        }

        private List<Contour>[] DetermineInsides(List<Contour> outsideContours, List<Contour> insideContours)
        {
            var insideContourLists = new List<Contour>[outsideContours.Count];

            for (int i = 0; i < outsideContours.Count; i++)
                insideContourLists[i] = new List<Contour>();

            foreach (var contour in insideContours)
            {
                Point rightP = FindRightmostPoint(contour);
                int y = _renderer.RoundAndClampY(_renderer.TransformY(rightP.Y));
                int x = _renderer.RoundAndClampX(_renderer.TransformX(rightP.X));

                for (; x < _renderer.MaskWidth; x++)
                {
                    byte b = _renderer.Mask[y, x];

                    if (b != 0)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if ((b & (1 << i)) != 0)
                            {
                                insideContourLists[i].Add(contour);

                                int outerIndex = _figure.Contours.IndexOf(outsideContours[i]);
                                Console.WriteLine("Found");
                                int innerIndex = _figure.Contours.IndexOf(contour);
                                Console.WriteLine("{0} goes with {1}", innerIndex, outerIndex);

                                goto Found;
                            }
                        }
                    }
                }

                Debug.Fail("");

            Found:
                ;
            }

            return insideContourLists;
        }

        private Shape[] PackageShapes(List<Contour> outsideContours, List<Contour>[] insideContourLists)
        {
            var shapes = new Shape[outsideContours.Count];

            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = new Shape(outsideContours[i], insideContourLists[i].ToArray());
            }

            return shapes;
        }

        public Shape[] CalculateShapes()
        {
            var (outsideContours, insideContours) = PartitionContours();
            DrawOutsides(outsideContours);
            var insideContourLists = DetermineInsides(outsideContours, insideContours);

            return PackageShapes(outsideContours, insideContourLists);
        }

#if DEBUG
        public OutlineRenderer OutlineRenderer { get => _renderer; }
#endif
    }
}
