using System;
using System.IO;
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
                var outlineTool = new OutlineTool(figure);

                var shapes = outlineTool.CalculateShapes();

                //outlineTool.RenderOutline();
                var bitmap = outlineTool.OutlineRenderer.RenderBitmap();
                bitmap.Save(@"C:\Temp\b.bmp");

                FigureSerializer.Serialize(figure, System.Console.Out);
#endif


                Console.WriteLine( "-----------------------" );
#if false
                const string S =
@"
figure begin
  11 pointgroup 0
  12 pointgroup 1
  contour begin
    9 point 43.671875 89.296875 11
    1 point 53.671875 9.296875 12
    edge line 9 1
    edge line 10 11
  end
  original-edge line 0 1
  original-edge line 1 2
end";
#else
                var stringWriter = new StringWriter();

                FigureSerializer.Serialize(figure, stringWriter);

                string S = stringWriter.ToString();
#endif
                TextReader reader = new StringReader(S);
                figure = FigureDeserialzer.Deserialize(reader);

                outlineTool = new OutlineTool(figure);

                outlineTool.CalculateShapes();

                outlineTool.OutlineRenderer.DrawOutline();
                bitmap = outlineTool.OutlineRenderer.RenderBitmap();
                bitmap.Save(@"C:\Temp\c.bmp");
            }
        }
    }
}
