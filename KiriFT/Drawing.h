#pragma once

using namespace System;

namespace KiriFT
{
    namespace Drawing
    {
        public ref class CharGridRendererArgs
        {
            String^ m_fontFamily;
            wchar_t* m_nativeFontFamily = nullptr;

        internal:
            property wchar_t* NativeFontFamily { wchar_t* get(); }

        public:
            IntPtr Hdc;
            Int32 Width;
            Int32 Height;
            Int32 Columns;
            Single HeightToWidth;
            UInt32 FirstCodePoint;
            Int32 OnColor;
            Int32 OffColor;
            Int32 FontStyle;
            property String^ FontFamily { String^ get(); void set(String^ value); }

            Single OutCellWidth;
            Single OutCellHeight;
            Int32 OutRows;

            ~CharGridRendererArgs();
            !CharGridRendererArgs();
        };

        public ref class CharGridRenderer
        {
        public:
            static void Draw(CharGridRendererArgs^ args);
        };
    }
}
