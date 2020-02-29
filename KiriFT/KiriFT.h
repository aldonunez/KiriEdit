#pragma once

using namespace System;

namespace KiriFT
{
	using namespace System::Runtime::InteropServices;

	public value struct FTVector
	{
	public:
		Int32 X;
		Int32 Y;
	};

	public ref struct FTBBox
	{
	public:
		Int32  Left, Bottom;
		Int32  Right, Top;
	};

	public ref struct FTGlyphMetrics
	{
	public:
		Int32 Width;
		Int32 Height;

		// There's a lot more to metrics. But we don't need it all.
	};

	[UnmanagedFunctionPointerAttribute(CallingConvention::Cdecl)]
	public delegate int MoveToHandler(FTVector% to, IntPtr user);

	[UnmanagedFunctionPointerAttribute(CallingConvention::Cdecl)]
	public delegate int LineToHandler(FTVector% to, IntPtr user);

	[UnmanagedFunctionPointerAttribute(CallingConvention::Cdecl)]
	public delegate int ConicToHandler(FTVector% control, FTVector% to, IntPtr user);

	[UnmanagedFunctionPointerAttribute(CallingConvention::Cdecl)]
	public delegate int CubicToHandler(FTVector% control1, FTVector% control2, FTVector% to, IntPtr user);

	public ref class OutlineHandlers
	{
	public:
		OutlineHandlers() { }

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

	public ref class Face
	{
		FT_Face m_face = nullptr;

	internal:
		Face(FT_Face face);

	public:
		~Face();
		!Face();

		property UInt32 FaceIndex { UInt32 get(); }
		property UInt32 FaceCount { UInt32 get(); }
		property String^ FamilyName { String^ get(); }
		property String^ StyleName { String^ get(); }

		void SetPixelSizes(UInt32 width, UInt32 height);
		FTBBox^ GetBBox();
		FTGlyphMetrics^ GetMetrics();

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

		Face^ OpenFace(String^ path, Int32 index);
	};

	public ref class FreeTypeException : Exception
	{
	};
}
