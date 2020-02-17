using SharpFont;
using System;

namespace TryFreetype
{
    public class FontFace : IDisposable
    {
        private Library _library;
        private Face _face;

        public FontFace(string path)
        {
            _library = new Library();

            _face = new Face(_library, path);
        }

        public Model.Figure DecomposeGlyph(char character, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            _face.SetPixelSizes(0, (uint) size);

            _face.LoadChar(character, LoadFlags.NoBitmap, LoadTarget.Normal);

            var walker = new GlyphWalker(_face.Glyph);

            walker.Decompose();

            return walker.Figure;
        }

        public void Dispose()
        {
            if (_face != null)
            {
                _face.Dispose();
                _face = null;
            }

            if (_library != null)
            {
                _library.Dispose();
                _library = null;
            }
        }
    }
}
