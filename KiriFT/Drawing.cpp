#include "pch.h"
#include <assert.h>

#include <msclr/marshal.h>
#include "Drawing.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace KiriFT
{
    namespace Drawing
    {
        UINT Utf32ToWideChar(UINT32 codePoint32, wchar_t& first, wchar_t& second)
        {
            if (codePoint32 < 0x10000)
            {
                first = (wchar_t) codePoint32;
                second = 0;
            }
            else
            {
                UINT32 temp = codePoint32 - 0x10000;
                first  = ((temp >> 10) & 0x3FF) + 0xD800;   // high
                second = ((temp >>  0) & 0x3FF) + 0xDC00;   // low
            }

            return (second == 0) ? 1 : 2;
        }

        COLORREF ConvertWinFormsColorToGdi(int wfColor)
        {
            COLORREF colorref =
                  ((wfColor & 0x0000FF) << 16)
                | ((wfColor & 0x00FF00) << 0)
                | ((wfColor & 0xFF0000) >> 16)
                ;
            return colorref;
        }

        void CharGridRenderer::Draw(CharGridRendererArgs^ args)
        {
            BOOL    bRet = FALSE;
            HGDIOBJ hOldObj = NULL;

            const int COLUMNS = args->Columns;

            HDC hdc = (HDC) args->Hdc.ToPointer();
            int contentWidth = args->Width;
            int contentHeight = args->Height;
            auto metrics = args->GetMetrics();
            float cellWidth = metrics.CellWidth;
            float cellHeight = metrics.CellHeight;
            int rows = metrics.Rows;

            // Lines

            HPEN hPen = CreatePen(PS_SOLID, 1, RGB(0, 0, 0));
            hOldObj = SelectObject(hdc, hPen);

            float x = 0;

            for (int i = 0; i < COLUMNS; i++)
            {
                MoveToEx(hdc, (int) x, 0, NULL);
                LineTo(hdc, (int) x, contentHeight);
                x += cellWidth;
            }

            x--;
            MoveToEx(hdc, (int) x, 0, NULL);
            LineTo(hdc, (int) x, contentHeight);

            float y = 0;

            for (int i = 0; i < rows; i++)
            {
                MoveToEx(hdc, 0, (int) y, NULL);
                LineTo(hdc, contentWidth, (int) y);
                y += cellHeight;
            }

            MoveToEx(hdc, 0, args->Height - 1, NULL);
            LineTo(hdc, contentWidth, args->Height - 1);

            SelectObject(hdc, hOldObj);
            DeleteObject(hPen);

            // Characters

            int logHeight = (int) (cellHeight * 0.7f);

            HFONT hFont = CreateFontW(
                logHeight,
                0,
                0,
                0,
                (args->FontStyle & 1) ? 700 : 400,
                (args->FontStyle & 2) ? TRUE : FALSE,
                FALSE,
                FALSE,
                DEFAULT_CHARSET,
                OUT_DEFAULT_PRECIS,
                CLIP_DEFAULT_PRECIS,
                DEFAULT_QUALITY,
                DEFAULT_PITCH,
                args->NativeFontFamily
            );

            hOldObj = SelectObject(hdc, hFont);

            COLORREF onColor = ConvertWinFormsColorToGdi(args->OnColor);
            COLORREF offColor = ConvertWinFormsColorToGdi(args->OffColor);

            SetBkMode(hdc, TRANSPARENT);
            SetTextAlign(hdc, TA_TOP | TA_LEFT);

            UINT32 lastCodePoint = args->LastCodePoint;

            if (lastCodePoint == 0)
                lastCodePoint = 0x10FFFF;

            array<Byte>^ residencyMap = args->ResidencyMap;
            Int32 residencyOffset = args->ResidencyOffset;
            UInt32 residencyWord = 0xFFFFFFFF;

            UINT32 codePoint = args->FirstCodePoint;
            float xcell = 0;
            float ycell = 0;

            for (int r = 0; r < rows; r++)
            {
                if (residencyMap != nullptr)
                {
                    if (residencyOffset + 3 >= residencyMap->Length)
                        break;

                    residencyWord =
                        (residencyMap[residencyOffset + 0])
                        | (residencyMap[residencyOffset + 1] << 8)
                        | (residencyMap[residencyOffset + 2] << 16)
                        ;
                    residencyOffset += 3;
                }

                for (int c = 0; c < COLUMNS; c++)
                {
                    if (codePoint > lastCodePoint)
                        break;

                    wchar_t str[2] = L"";
                    int len = 0;
                    int logWidth = 0;

                    len = Utf32ToWideChar(codePoint, str[0], str[1]);

                    SIZE size;
                    bRet = GetTextExtentPoint32(hdc, str, len, &size);
                    assert(bRet);
                    logWidth = size.cx;

                    int x = (int) (xcell + (cellWidth - logWidth) / 2);
                    int y = (int) (ycell + (cellHeight - logHeight) / 2);
                    COLORREF color;

                    if ((residencyWord & 1) != 0)
                        color = onColor;
                    else
                        color = offColor;

                    residencyWord >>= 1;

                    SetTextColor(hdc, color);
                    TextOutW(hdc, x, y, str, len);

                    xcell += cellWidth;
                    codePoint++;
                }

                xcell = 0;
                ycell += cellHeight;
            }

            SelectObject(hdc, hOldObj);
            DeleteObject(hFont);
        }

        wchar_t* CharGridRendererArgs::NativeFontFamily::get()
        {
            return m_nativeFontFamily;
        }

        String^ CharGridRendererArgs::FontFamily::get()
        {
            return m_fontFamily;
        }

        void CharGridRendererArgs::FontFamily::set(String^ value)
        {
            if (!String::Equals(value, m_fontFamily))
            {
                if (m_nativeFontFamily != nullptr)
                {
                    Marshal::FreeHGlobal(IntPtr(m_nativeFontFamily));
                    m_nativeFontFamily = nullptr;
                }

                if (value != nullptr)
                {
                    m_nativeFontFamily = (wchar_t*) Marshal::StringToHGlobalUni(value).ToPointer();
                }

                m_fontFamily = value;
            }
        }

        CharGridMetrics CharGridRendererArgs::GetMetrics()
        {
            CharGridMetrics metrics;

            float cellWidth = (Width / (float) Columns);
            float cellHeight = cellWidth * HeightToWidth;
            int rows = (int) ceilf(Height / cellHeight);

            metrics.CellHeight = cellHeight;
            metrics.CellWidth = cellWidth;
            metrics.Rows = rows;

            return metrics;
        }

        CharGridRendererArgs::~CharGridRendererArgs()
        {
            this->!CharGridRendererArgs();
        }

        CharGridRendererArgs::!CharGridRendererArgs()
        {
            if (m_nativeFontFamily != nullptr)
            {
                Marshal::FreeHGlobal(IntPtr(m_nativeFontFamily));
                m_nativeFontFamily = nullptr;
            }
        }
    }
}
