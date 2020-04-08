/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;
using System;

namespace KiriFig
{
    public class FontLibrary : IDisposable
    {
        private Library _library = new Library();

        public FontFace OpenFace(string path, int index, bool ignoreTypographicNames = false)
        {
            OpenParams @params =
                ignoreTypographicNames ? OpenParams.IgnoreTypographicFamily : OpenParams.None;

            Face face = _library.OpenFace(path, index, @params);

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

        public Model.Figure DecomposeGlyph(uint character, int size = 0)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            if (size > 0)
                _face.SetPixelSizes(0, (uint) size);

            _face.LoadChar(character, size == 0);

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
