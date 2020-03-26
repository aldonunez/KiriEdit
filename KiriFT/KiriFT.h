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

    [Flags]
    public enum class FaceFlags
    {
        Scalable = 1 << 0,
    };

    public value struct SfntName
    {
    public:
        UInt16 LanguageId;
        UInt16 NameId;
        String^ String;
    };

	public ref class Face
	{
        value struct SfntNameRef
        {
        public:
            UInt32 Index;
            UInt16 LanguageId;
            UInt16 NameId;
            String^ Name;
        };

        FT_Face m_face = nullptr;
        System::Collections::Generic::List<SfntNameRef>^ m_filteredSfntNames;

    private:
        void LoadSfntNames();

	internal:
		Face(FT_Face face);

	public:
		~Face();
		!Face();

		property Int32 FaceIndex { Int32 get(); }
		property Int32 FaceCount { Int32 get(); }
		property String^ FamilyName { String^ get(); }
		property String^ StyleName { String^ get(); }
        property FaceFlags Flags { FaceFlags get(); }

		void SetPixelSizes(UInt32 width, UInt32 height);
		FTBBox^ GetBBox();
		FTGlyphMetrics^ GetMetrics();
        UInt32 GetSfntNameCount();
        SfntName GetSfntName( UInt32 index );

		void LoadChar(UInt32 ch, Boolean noScale);
		void Decompose(OutlineHandlers^ handlers);

        static Int32 ParseLegacyStyle(String^ styleName);
	};

    [Flags]
    public enum class OpenParams
    {
        None,
        IgnoreTypographicFamily,
        IgnoreTypographicSubfamily,
    };

	public ref class Library
	{
		FT_Library m_lib = nullptr;

	public:
		Library();
		~Library();
		!Library();

		Face^ OpenFace(String^ path, Int32 index);
        Face^ OpenFace( String^ path, Int32 index, OpenParams openParams );
	};

	public ref class FreeTypeException : Exception
	{
        FT_Error m_error;

    internal:
        FreeTypeException(FT_Error error) : m_error(error) { }

    public:
        property Int32 Error { Int32 get(); }
	};
}
