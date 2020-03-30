using TryFreetype;
using TryFreetype.Model;

namespace KiriProj
{
    public static class FigureUtils
    {
        public static Figure MakeMasterFigure(string fontPath, int faceIndex, uint character)
        {
            using (var lib = new FontLibrary())
            using (var face = lib.OpenFace(fontPath, faceIndex))
            {
                Figure figure = face.DecomposeGlyph(character);
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

                shape.Contours = new int[toolShape.InnerContours.Length + 1];

                shape.Contours[0] = figure.Contours.IndexOf(toolShape.OuterContour);

                for (int j = 0; j < toolShape.InnerContours.Length; j++)
                {
                    shape.Contours[j + 1] = figure.Contours.IndexOf(toolShape.InnerContours[j]);
                }
            }

            return shapes;
        }
    }
}
