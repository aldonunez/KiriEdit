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
    }
}
