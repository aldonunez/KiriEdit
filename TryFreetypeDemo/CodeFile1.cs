using SharpFont;
using System;
using System.Drawing;

namespace TryFreetype.Sample1
{
    static class Sample
    {
        public static void Run()
        {
            using (Library lib = new Library())
            {
                using (var face = new Face(lib, @"C:\Windows\Fonts\consola.ttf"))
                {
                    // 32 -> ?
                    face.SetPixelSizes(0, 160);
                    //face.SetCharSize(,,)

                    //FT_Face face = m_face;
                    //FT_GlyphSlot slot = face->glyph;
                    //FT_Outline &outline = slot->outline;
                    //FT_Error error = FT_Outline_Decompose(&outline, &callbacks, this);

                    face.LoadChar('A', LoadFlags.NoBitmap
                        //| LoadFlags.NoScale
                        , LoadTarget.Normal);

                    Console.WriteLine("BitmapTop = {0}", face.Glyph.BitmapTop);
                    Console.WriteLine("BitmapLeft = {0}", face.Glyph.BitmapLeft);
                    TryFreetypeDemo.Utils.PrintProperties(face.Glyph, "");

                    Console.WriteLine("------------------");
                    BBox bbox = face.Glyph.Outline.GetBBox();
                    TryFreetypeDemo.Utils.PrintProperties(bbox, "");

                    {
                        //Graphics g;
                        //g.fil

                        Recorder recorder = new Recorder(face.Glyph);

                        var outlineFuncs = new OutlineFuncs
                        {
                            MoveFunction = recorder.MoveToFunc,
                            LineFunction = recorder.LineToFunc,
                            ConicFunction = recorder.ConicToFunc,
                            CubicFunction = recorder.CubicToFunc,
                        };
                        var o = face.Glyph.Outline;
                        o.Decompose(outlineFuncs, IntPtr.Zero);

                        recorder.Bitmap.Save(@"C:\Temp\b.bmp");
                    }

                    {
                        face.Glyph.RenderGlyph(RenderMode.Normal);
                        var bmp = face.Glyph.Bitmap.ToGdipBitmap();
                        bmp.Save(@"C:\Temp\a.bmp");
                    }

                    Console.WriteLine("abc");
                }
            }
        }
    }

    class Recorder
    {
        private readonly Bitmap bitmap;

        double x, y;
        Graphics g;
        Pen pen;
        const double XMult = 1000;
        const double YMult = 1000;

        public Bitmap Bitmap { get { return bitmap; } }

        public Recorder(GlyphSlot glyphSlot)
        {
            var bbox = glyphSlot.Outline.GetBBox();

            int width = glyphSlot.Metrics.Width.ToInt32();
            int height = glyphSlot.Metrics.Height.ToInt32();

            bitmap = new Bitmap(width, height);
            Pen borderPen = new Pen(Color.White);

            g = Graphics.FromImage(bitmap);
            //g.DrawRectangle(borderPen, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            //g.DrawEllipse(borderPen, 128, 224, 10, 10);
            g.ScaleTransform(1, -1);
            //g.TranslateTransform(0, -103);
            g.TranslateTransform(0, -(height - 1));
            pen = new Pen(Color.Red);
        }

        public int MoveToFunc(ref FTVector to, IntPtr user)
        {
            Console.WriteLine("MoveTo: {0}, {1}", to.X, to.Y);
            x = to.X.ToDouble();
            y = to.Y.ToDouble();
            return 0;
        }

        public int LineToFunc(ref FTVector to, IntPtr user)
        {
            Console.WriteLine("LineTo: {0}, {1}", to.X, to.Y);
            g.DrawLine(
                pen,
                (float) (x * XMult),
                (float) (y * YMult),
                (float) (to.X.ToDouble() * XMult),
                (float) (to.Y.ToDouble() * YMult));
            x = to.X.ToDouble();
            y = to.Y.ToDouble();
            return 0;
        }

        public int ConicToFunc(ref FTVector control, ref FTVector to, IntPtr user)
        {
            Console.WriteLine("ConicTo: {0},{1} {2},{3}", control.X, control.Y, to.X, to.Y);
            return 0;
        }

        public int CubicToFunc(ref FTVector control1, ref FTVector control2, ref FTVector to, IntPtr user)
        {
            Console.WriteLine("CubicTo: {0},{1} {2},{3} {4},{5}", control1.X, control1.Y, control2.X, control2.Y, to.X, to.Y);
            return 0;
        }
    }
}
