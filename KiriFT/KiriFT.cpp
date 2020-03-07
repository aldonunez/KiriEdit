#include "pch.h"

#include "KiriFT.h"

#define generic generic1
#include <ft2build.h>
#include FT_FREETYPE_H
#include FT_OUTLINE_H
#include FT_BBOX_H
#include FT_PARAMETER_TAGS_H
#include FT_SFNT_NAMES_H
#include FT_TRUETYPE_IDS_H
#undef generic

using namespace System;
using namespace System::Runtime::InteropServices;

namespace KiriFT
{
    Library::Library()
    {
        FT_Error error;
        FT_Library lib;

        error = FT_Init_FreeType(&lib);
        if (error)
            throw gcnew FreeTypeException(error);

        m_lib = lib;
    }

    Library::~Library()
    {
        this->!Library();
    }

    Library::!Library()
    {
        if (m_lib != nullptr)
        {
            FT_Done_FreeType(m_lib);
            m_lib = nullptr;
        }
    }

    Face^ Library::OpenFace(String^ path, Int32 index)
    {
        return OpenFace(path, index, OpenParams::None);
    }

    Face^ Library::OpenFace(String^ path, Int32 index, OpenParams openParams)
    {
        FT_Error error;
        FT_Parameter params[] =
        {
            { 0, NULL },
            { 0, NULL },
        };
        FT_Open_Args args;
        FT_Face face;

        msclr::interop::marshal_context context;
        const char* nativePath = context.marshal_as<const char*>(path);

        args.flags = FT_OPEN_PATHNAME | FT_OPEN_PARAMS;
        args.pathname = (char*) nativePath;
        args.params = params;
        args.num_params = 0;

        if ((openParams & OpenParams::IgnoreTypographicFamily) == OpenParams::IgnoreTypographicFamily)
        {
            params[args.num_params].tag = FT_PARAM_TAG_IGNORE_TYPOGRAPHIC_FAMILY;
            args.num_params++;
        }

        if ((openParams & OpenParams::IgnoreTypographicSubfamily) == OpenParams::IgnoreTypographicSubfamily)
        {
            params[args.num_params].tag = FT_PARAM_TAG_IGNORE_TYPOGRAPHIC_SUBFAMILY;
            args.num_params++;
        }

        error = FT_Open_Face(m_lib, &args, index, &face);
        if (error)
            throw gcnew FreeTypeException(error);

        Face^ fontFace = gcnew Face(face);

        return fontFace;
    }

    void Face::LoadSfntNames()
    {
        if (m_filteredSfntNames != nullptr)
            return;

        UInt32 grossCount = FT_Get_Sfnt_Name_Count(m_face);

        m_filteredSfntNames = gcnew System::Collections::Generic::List<SfntNameRef>();

        for (UInt32 i = 0; i < grossCount; i++)
        {
            FT_Error error;
            FT_SfntName ftName;
            SfntNameRef managedName;

            error = FT_Get_Sfnt_Name(m_face, i, &ftName);
            if (error)
                throw gcnew FreeTypeException(error);

            if (   ftName.platform_id == TT_PLATFORM_MICROSOFT
                && ftName.encoding_id == TT_MS_ID_UNICODE_CS)
            {
                managedName.Index = i;
                m_filteredSfntNames->Add( managedName );
            }
        }
    }

    Face::Face(FT_Face face) :
        m_face(face)
    {
    }

    Face::~Face()
    {
        this->!Face();
    }

    Face::!Face()
    {
        if (m_face != nullptr)
        {
            FT_Done_Face(m_face);
            m_face = nullptr;
        }
    }

    Int32 Face::FaceIndex::get()
    {
        return m_face->face_index;
    }

    Int32 Face::FaceCount::get()
    {
        return m_face->num_faces;
    }

    String^ Face::FamilyName::get()
    {
        return gcnew String(m_face->family_name);
    }

    String^ Face::StyleName::get()
    {
        return gcnew String(m_face->style_name);
    }

    FaceFlags Face::Flags::get()
    {
        return (FaceFlags) m_face->face_flags;
    }

    void Face::SetPixelSizes(UInt32 width, UInt32 height)
    {
        FT_Error error;

        error = FT_Set_Pixel_Sizes(m_face, width, height);
        if (error)
            throw gcnew FreeTypeException(error);
    }

    FTBBox^ Face::GetBBox()
    {
        FT_Error error;
        FT_BBox bbox;

        error = FT_Outline_Get_BBox(&m_face->glyph->outline, &bbox);
        if (error)
            throw gcnew FreeTypeException(error);

        FTBBox^ bboxFT = gcnew FTBBox();

        bboxFT->Left = bbox.xMin;
        bboxFT->Bottom = bbox.yMin;
        bboxFT->Right = bbox.xMax;
        bboxFT->Top = bbox.yMax;

        return bboxFT;
    }

    FTGlyphMetrics^ Face::GetMetrics()
    {
        FTGlyphMetrics^ metrics = gcnew FTGlyphMetrics();

        metrics->Width = m_face->glyph->metrics.width;
        metrics->Height = m_face->glyph->metrics.height;

        return metrics;
    }

    UInt32 Face::GetSfntNameCount()
    {
        if (m_filteredSfntNames == nullptr)
        {
            LoadSfntNames();
        }

        return m_filteredSfntNames->Count;
    }

    SfntName Face::GetSfntName(UInt32 index)
    {
        if (m_filteredSfntNames == nullptr || index >= (UInt32) m_filteredSfntNames->Count)
            throw gcnew ArgumentException();

        SfntNameRef nameRef = m_filteredSfntNames[index];

        if (nameRef.Name == nullptr)
        {
            FT_Error error;
            FT_SfntName ftName;

            error = FT_Get_Sfnt_Name(m_face, nameRef.Index, &ftName);
            if (error)
                throw gcnew FreeTypeException(error);

            array<Char>^ charArray = gcnew array<Char>(ftName.string_len / 2);

            for (UInt32 i = 0; i < ftName.string_len; i += 2)
            {
                Char ch = (ftName.string[i] << 8) | ftName.string[i + 1];
                charArray[i / 2] = ch;
            }

            nameRef.Name = gcnew String(charArray);
            nameRef.LanguageId = ftName.language_id;
            nameRef.NameId = ftName.name_id;
            // The elements are of a value type. So copy it whole to update it.
            m_filteredSfntNames[index] = nameRef;
        }

        SfntName name;
        name.LanguageId = nameRef.LanguageId;
        name.NameId = nameRef.NameId;
        name.String = nameRef.Name;
        return name;
    }

    void Face::LoadChar(UInt32 ch)
    {
        FT_Error error;

        error = FT_Load_Char(m_face, ch, FT_LOAD_NO_BITMAP);
        if (error)
            throw gcnew FreeTypeException(error);
    }

    void Face::Decompose(OutlineHandlers^ handlers)
    {
        FT_Error error;
        FT_Outline_Funcs funcs = { 0 };

        GCHandle m = GCHandle::Alloc(handlers->MoveTo);
        GCHandle l = GCHandle::Alloc(handlers->LineTo);
        GCHandle o = GCHandle::Alloc(handlers->ConicTo);
        GCHandle u = GCHandle::Alloc(handlers->CubicTo);

        funcs.move_to = (FT_Outline_MoveTo_Func) Marshal::GetFunctionPointerForDelegate(handlers->MoveTo).ToPointer();
        funcs.line_to = (FT_Outline_LineTo_Func) Marshal::GetFunctionPointerForDelegate(handlers->LineTo).ToPointer();
        funcs.conic_to = (FT_Outline_ConicTo_Func) Marshal::GetFunctionPointerForDelegate(handlers->ConicTo).ToPointer();
        funcs.cubic_to = (FT_Outline_CubicTo_Func) Marshal::GetFunctionPointerForDelegate(handlers->CubicTo).ToPointer();

        error = FT_Outline_Decompose(&m_face->glyph->outline, &funcs, NULL);

        m.Free();
        l.Free();
        o.Free();
        u.Free();

        if (error)
            throw gcnew FreeTypeException(error);
    }

    Int32 Face::ParseLegacyStyle(String^ styleName)
    {
        // TODO: literal numbers
        Int32 style = 0;

        if (   styleName->IndexOf("Fat", StringComparison::OrdinalIgnoreCase) >= 0
            || styleName->IndexOf("Heavy", StringComparison::OrdinalIgnoreCase) >= 0
            || styleName->IndexOf("Thick", StringComparison::OrdinalIgnoreCase) >= 0
            || styleName->IndexOf("Bold", StringComparison::OrdinalIgnoreCase) >= 0
            || styleName->IndexOf("Black", StringComparison::OrdinalIgnoreCase) >= 0)
        {
            style |= 1;
        }

        if (   styleName->IndexOf("Oblique", StringComparison::OrdinalIgnoreCase) >= 0
            || styleName->IndexOf("Italic", StringComparison::OrdinalIgnoreCase) >= 0)
        {
            style |= 2;
        }

        return style;
    }

    OutlineHandlers::OutlineHandlers(MoveToHandler^ moveTo, LineToHandler^ lineTo, ConicToHandler^ conicTo, CubicToHandler^ cubicTo) :
        MoveTo(moveTo),
        LineTo(lineTo),
        ConicTo(conicTo),
        CubicTo(cubicTo)
    {
    }

    Int32 FreeTypeException::Error::get()
    {
        return m_error;
    }
}
