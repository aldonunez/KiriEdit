using SharpFont;
using System;

namespace TryFreetype
{
    public class FontGlyph
    {
        private GlyphSlot _glyphSlot;

        internal FontGlyph(GlyphSlot glyphSlot)
        {
            _glyphSlot = glyphSlot;
        }

        internal GlyphSlot Glyph { get => _glyphSlot; }
    }

    public class FontFace : IDisposable
    {
        private Library _library;
        private Face _face;

        public FontFace(string path)
        {
            _library = new Library();

            _face = new Face(_library, path);
        }

        public FontGlyph GetGlyph(char character, uint size)
        {
            _face.SetPixelSizes(0, size);

            _face.LoadChar(character, LoadFlags.NoBitmap, LoadTarget.Normal);

            return new FontGlyph(_face.Glyph);
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
