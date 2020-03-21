using System.IO;
using TryFreetype;
using TryFreetype.Model;

namespace KiriEdit
{
    public static class FigureUtils
    {
        private const int StandardSize = 160;

        public static Figure MakeMasterFigure(string fontPath, int faceIndex, uint character)
        {
            using (var lib = new FontLibrary())
            using (var face = lib.OpenFace(fontPath, faceIndex))
            {
                Figure figure = face.DecomposeGlyph(character, StandardSize);
                return figure;
            }
        }

        public static FigureDocument.Shape[] CalculateShapes(Figure figure)
        {
            var outlineTool = new OutlineTool(figure);

            var toolShapes = outlineTool.CalculateShapes();

            var shapes = new FigureDocument.Shape[toolShapes.Length];

            for (int i = 0; i < shapes.Length; i++)
            {
                var toolShape = toolShapes[i];
                var shape = new FigureDocument.Shape();

                shapes[i] = shape;

                shape.OuterContour = figure.Contours.IndexOf(toolShape.OuterContour);

                shape.InnerContours = new int[toolShape.InnerContours.Length];

                for (int j = 0; j < toolShape.InnerContours.Length; j++)
                {
                    shape.InnerContours[j] = figure.Contours.IndexOf(toolShape.InnerContours[j]);
                }
            }

            return shapes;
        }
    }
}
