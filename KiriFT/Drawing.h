/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

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
            virtual Int32 MapToIndex(UInt32 codePoint) abstract;
            virtual UInt32 MapToCodePoint(Int32 index) abstract;
        };

        public ref class SequentialCharSet : CharSet
        {
        internal:
            array<Int32>^ _residencyMap;
            Int32 _firstCodePoint;
            Int32 _lastCodePoint;

        public:
            SequentialCharSet(
                array<Int32>^ residencyMap,
                Int32 firstCodePoint,
                Int32 lastCodePoint);

            virtual property Int32 Length { Int32 get() override; };
            virtual void SetIncluded(Int32 index, Boolean value) override;
            virtual Int32 MapToIndex(UInt32 codePoint) override;
            virtual UInt32 MapToCodePoint(Int32 index) override;

            static Int32 GetRecommendedMapSize(Int32 charCount);
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
            Int32 Left;
            Int32 Top;
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

            // TODO: Consider splitting the line drawing code into its own method.

            static const Int32 MinimumColumns = 1;
            static const Int32 MaximumColumns = 32;
        };
    }
}
