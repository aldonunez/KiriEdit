#pragma once

using namespace System;

namespace KiriFT
{
    namespace Drawing
    {
        public ref class CharGridRendererArgs
        {
        public:
            IntPtr Hdc;
            Int32 Width;
            Int32 Height;
            Int32 Columns;
            Single HeightToWidth;
            UInt32 FirstCodePoint;
            Int32 OnColor;
            Int32 OffColor;
            String^ FontFamily;
            Int32 FontStyle;

            Single OutCellWidth;
            Single OutCellHeight;
            Int32 OutRows;
        };

        public ref class CharGridRenderer
        {
        public:
            static void Draw(CharGridRendererArgs^ args);
        };
    }
}
