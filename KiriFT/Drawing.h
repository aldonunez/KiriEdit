#pragma once

using namespace System;

namespace KiriFT
{
    namespace Drawing
    {
        public value struct CharGridMetrics
        {
        public:
            Single CellWidth;
            Single CellHeight;
            Int32 Rows;
        };

        public ref class CharSet abstract
        {
        public:
            virtual property Int32 Length { Int32 get() abstract; };
            virtual void SetIncluded(Int32 index, Boolean value) abstract;
        };

        public ref class SequentialCharSet : CharSet
        {
        internal:
            array<Int32>^ _residencyMap;
            Int32 _columns;
            Int32 _firstCodePoint;
            Int32 _lastCodePoint;

        public:
            SequentialCharSet(
                array<Int32>^ residencyMap,
                Int32 columns,
                Int32 firstCodePoint,
                Int32 lastCodePoint);

            virtual property Int32 Length { Int32 get() override; };
            virtual void SetIncluded(Int32 index, Boolean value) override;
        };

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
            Int32 OnColor;
            Int32 OffColor;
            Int32 FontStyle;
            Int32 StartRow;
            property String^ FontFamily { String^ get(); void set(String^ value); }

            CharGridMetrics GetMetrics();

            ~CharGridRendererArgs();
            !CharGridRendererArgs();
        };

        public ref class CharGridRenderer
        {
        public:
            static void Draw(CharGridRendererArgs^ args, CharSet^ charSet);
        };
    }
}
