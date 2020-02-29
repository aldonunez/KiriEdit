#pragma once

using namespace System;

namespace KiriFT
{
	public value struct Point
	{
	public:
		Int32 X;
		Int32 Y;
	};

	public delegate void MoveToHandler(Point to);
	public delegate void LineToHandler(Point to);
	public delegate void ConicToHandler(Point control, Point to);
	public delegate void CubicToHandler(Point control1, Point control2, Point to);

	public ref class OutlineHandlers
	{
	public:
		OutlineHandlers(
			MoveToHandler^ MoveTo,
			LineToHandler^ LineTo,
			ConicToHandler^ ConicTo,
			CubicToHandler^ CubicTo
		);

		MoveToHandler^ MoveTo;
		LineToHandler^ LineTo;
		ConicToHandler^ ConicTo;
		CubicToHandler^ CubicTo;
	};

	public ref class FontFace
	{
		FT_Face m_face = nullptr;

	internal:
		FontFace(FT_Face face);

	public:
		~FontFace();
		!FontFace();

		property UInt32 FaceIndex { UInt32 get(); }
		property UInt32 FaceCount { UInt32 get(); }
		property String^ FamilyName { String^ get(); }
		property String^ StyleName { String^ get(); }

		void SetPixelSizes(UInt32 width, UInt32 height);

		void LoadChar(UInt32 ch);
		void Decompose(OutlineHandlers^ handlers);
	};

	public ref class Library
	{
		FT_Library m_lib = nullptr;

	public:
		Library();
		~Library();
		!Library();

		FontFace^ OpenFace(String^ path, Int32 index);
	};

	public ref class FreeTypeException : Exception
	{
	};
}
