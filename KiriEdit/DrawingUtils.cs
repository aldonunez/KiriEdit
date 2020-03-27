﻿using KiriProj;
using System.Drawing;
using TryFreetype.Model;

namespace KiriEdit
{
    internal static class DrawingUtils
    {
        public static Rectangle CenterFigure(Figure figure, Size boundSize)
        {
            int height = boundSize.Height;
            int width = boundSize.Width;

            float figureWidthToHeight = figure.Width / (float) figure.Height;
            int scaledWidth = (int) (width * figureWidthToHeight);

            Rectangle rect = new Rectangle(
                (width - scaledWidth) / 2,
                0,
                scaledWidth,
                height);

            return rect;
        }

        public static void PaintPiece(FigureDocument doc, Graphics graphics, Rectangle rect, bool standOut = false)
        {
            Brush brush = standOut ? Brushes.Red : Brushes.Black;

            using (var painter = new SystemFigurePainter(doc))
            {
                painter.SetTransform(graphics, rect);

                for (int i = 0; i < doc.Shapes.Length; i++)
                {
                    if (doc.Shapes[i].Enabled)
                    {
                        painter.PaintShape(i);
                        painter.Fill(graphics, brush);
                    }
                }

                painter.PaintFull();
                painter.Draw(graphics);
            }
        }
    }
}
