using TryFreetype.Model;

namespace TryFreetype.Sample3
{
    static class Sample
    {
        public static void Run()
        {
            using (var face = new FontFace(@"C:\Windows\Fonts\consola.ttf"))
            {
                Figure figure = face.DecomposeGlyph('A', 160);

                {
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

#if false
                var renderer = new DebugFigureRenderer(walker.Figure);

                renderer.Render();
                renderer.Bitmap.Save(@"C:\Temp\b.bmp");
#else
                var renderer = new OutlineRenderer(figure);

                renderer.CalculateShapes();

                //renderer.RenderOutline();
                var bitmap = renderer.RenderBitmap();
                bitmap.Save(@"C:\Temp\b.bmp");
#endif
            }
        }
    }
}
