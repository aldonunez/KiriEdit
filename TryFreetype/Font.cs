﻿using KiriFT;
using System;

namespace TryFreetype
{
    public class FontLibrary : IDisposable
    {
        private Library _library = new Library();

        public FontFace OpenFace(string path, int index, bool ignoreTypographicNames = false)
        {
            Face face = _library.OpenFace(path, index, ignoreTypographicNames);

            FontFace fontFace = new FontFace(face);

            return fontFace;
        }

        public void Dispose()
        {
            if (_library != null)
            {
                _library.Dispose();
                _library = null;
            }
        }
    }

    public class FontFace : IDisposable
    {
        private Face _face;

        internal FontFace(Face face)
        {
            _face = face;
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
        }
    }
}
