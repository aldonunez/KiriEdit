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
    }
}
