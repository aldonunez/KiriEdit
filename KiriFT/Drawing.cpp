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
        UINT Utf32ToWideChar(UINT32 codePoint32, wchar_t& high, wchar_t& low)
        {
            if (codePoint32 < 0x10000)
            {
                high = 0;
                low = (wchar_t) codePoint32;
            }
            else
            {
                UINT32 temp = codePoint32 - 0x10000;
                high = ((temp >> 10) & 0x3FF) + 0xD800;
                low = ((temp >> 0) & 0x3FF) + 0xDC00;
            }

            return (high == 0) ? 1 : 2;
        }

        void CharGridRenderer::Draw(CharGridRendererArgs^ args)
        {
            BOOL    bRet = FALSE;
            HGDIOBJ hOldObj = NULL;

            const int COLUMNS = args->Columns;

            HDC hdc = (HDC) args->Hdc.ToPointer();
            int contentWidth = args->Width;
            int contentHeight = args->Height;
            float cellWidth = (contentWidth / (float) args->Columns);
            float cellHeight = cellWidth * args->HeightToWidth;
            int rows = (int) ceilf(contentHeight / cellHeight);

            args->OutCellWidth = cellWidth;
            args->OutCellHeight = cellHeight;
            args->OutRows = rows;

            msclr::interop::marshal_context context;
            const wchar_t* nativeFontFamily = context.marshal_as<const wchar_t*>(args->FontFamily);

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
                nativeFontFamily
            );

            hOldObj = SelectObject(hdc, hFont);

            COLORREF onColor =
                  ((args->OnColor & 0x0000FF) << 16)
                | ((args->OnColor & 0x00FF00) << 0)
                | ((args->OnColor & 0xFF0000) >> 16)
                ;

            SetBkMode(hdc, TRANSPARENT);
            SetTextAlign(hdc, TA_TOP | TA_LEFT);
            SetTextColor(hdc, onColor);
            System::Diagnostics::Debug::WriteLine("OnColor:");
            System::Diagnostics::Debug::WriteLine((UInt32) args->OnColor);

            UINT32 codePoint = args->FirstCodePoint;
            float xcell = 0;
            float ycell = 0;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < COLUMNS; c++)
                {
                    wchar_t str[2] = L"";
                    int len = 0;
                    int logWidth = 0;

                    len = Utf32ToWideChar(codePoint, str[1], str[0]);

                    SIZE size;
                    bRet = GetTextExtentPoint32(hdc, str, len, &size);
                    logWidth = size.cx;
                    assert(bRet);

                    int x = (int) (xcell + (cellWidth - logWidth) / 2);
                    int y = (int) (ycell + (cellHeight - logHeight) / 2);

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
    }
}
