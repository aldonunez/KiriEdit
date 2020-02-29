using SharpFont;
using System;
using TryFreetype.Model;

namespace TryFreetype.Sample2
{
#if false
    static class Sample
    {
        public static void Run()
        {
#if false
            Point p1 = new Point { X = 10, Y = 20 };
            Point p2 = new Point { X = p1.X + 1000000, Y = p1.Y + 1000000 };
            LineEdge lineEdge = new LineEdge { P1 = p1, P2 = p2 };

            var result = lineEdge.Split(new Point { X = 13, Y = 24 });
            return;
#endif

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
                    Utils.PrintProperties(face.Glyph, "");

                    {
                        GlyphWalker walker = new GlyphWalker(face.Glyph);

                        walker.Decompose();

                        {
                            Figure figure = walker.Figure;
#if true
                            Point p6 = figure.PointGroups[6].Points[0];
                            Point p1 = new Point((p6.X + p6.OutgoingEdge.P2.X) / 2, p6.Y);
                            var e = figure.PointGroups[6].Points[0].OutgoingEdge;
                            var midPoint = figure.AddDiscardablePoint(p1, e);
#endif

#if true
                            var cut = figure.AddCut(midPoint, figure.PointGroups[9].Points[0]);
#if !true
                            figure.DeleteCut(cut);
#endif
#endif

#if false
                            figure.DeleteDiscardablePoint(midPoint.Group);
#endif
                        }

                        DebugFigureRenderer renderer = new DebugFigureRenderer(walker.Figure);

                        renderer.Render();

                        renderer.Bitmap.Save(@"C:\Temp\b.bmp");
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
#endif
}
