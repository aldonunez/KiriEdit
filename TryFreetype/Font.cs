using KiriFT;
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
            // TODO: Get the index from the user.
            _face = _library.OpenFace(path, 0);
        }

        public Model.Figure DecomposeGlyph(uint character, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            _face.SetPixelSizes(0, (uint) size);

            _face.LoadChar(character);

            var walker = new GlyphWalker(_face);

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
