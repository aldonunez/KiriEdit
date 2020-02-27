using System;
using System.Collections.Generic;

namespace KiriEdit.Font
{
    internal class FontFamily
    {
        private FontFace[] _fontFaces = new FontFace[4];

        public string FamilyName { get; }

        public FontFamily(string familyName)
        {
            FamilyName = familyName;
        }

        internal void AddFace(FontStyle style, FontFace face)
        {
            int index = (int) style;

            _fontFaces[index] = face;

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

    internal class FontFace
    {
        public FontFamily FontFamily { get; }
        public FontStyle FontStyle { get; }
        public string Path { get; }
        public int FaceIndex { get; }

        public FontFace(FontFamily fontFamily, FontStyle fontStyle, string path, int faceIndex)
        {
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            Path = path;
            FaceIndex = faceIndex;
        }
    }

    [Flags]
    internal enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
    }
}
