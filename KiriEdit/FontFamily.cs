using System;
using System.Collections.Generic;

namespace KiriEdit.Font
{
    public class FontFamily
    {
        private FontFace[] _fontFaces = new FontFace[4];

        public string Name { get; }

        public FontFamily(string familyName)
        {
            Name = familyName;
        }

        internal void AddFace(FontStyle style, FontFace face)
        {
            int index = (int) style;

            _fontFaces[index] = face;

            // TODO: handle this differently.
            if (_fontFaces[0] == null || style == FontStyle.Regular)
                _fontFaces[0] = face;
        }

        public FontFace GetFace(FontStyle style)
        {
            int index = (int) style;

            return _fontFaces[index];
        }
    }

    class FontFamilyCollection : Dictionary<string, FontFamily>
    {
    }

    public class FontFace
    {
        public FontFamily Family { get; }
        public FontStyle Style { get; }
        public string Path { get; }
        public int FaceIndex { get; }

        public FontFace(FontFamily fontFamily, FontStyle fontStyle, string path, int faceIndex)
        {
            Family = fontFamily;
            Style = fontStyle;
            Path = path;
            FaceIndex = faceIndex;
        }
    }

    [Flags]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
    }
}
