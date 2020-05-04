/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriProj;
using KiriFig.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KiriEdit
{
    internal static class DrawingUtils
    {
        public const float GlyphImageHeightRatio = 0.95f;

        public static double GetLineLength( int x1, int y1, int x2, int y2 )
        {
            float dX = x2 - x1;
            float dY = y2 - y1;

            return Math.Sqrt( dX * dX + dY * dY );
        }

        public static Rectangle CenterFigure( Figure figure, Size boundSize )
        {
            int height = boundSize.Height;
            int width = boundSize.Width;

            float figureWidthToHeight = figure.Width / (float) figure.Height;
            int scaledWidth = (int) (width * figureWidthToHeight);

            Rectangle rect = new Rectangle(
                (width - scaledWidth) / 2,
                0,
                scaledWidth,
                height );

            return rect;
        }

        public static void PaintPiece( FigureDocument doc, Graphics graphics, Rectangle rect, bool standOut = false )
        {
            Brush brush = standOut ? Brushes.Red : Brushes.Black;

            using ( var painter = new SystemFigurePainter( doc ) )
            {
                painter.SetTransform( graphics, rect );

                for ( int i = 0; i < doc.Figure.Shapes.Count; i++ )
                {
                    if ( doc.Figure.Shapes[i].Enabled )
                    {
                        painter.PaintShape( i );
                        painter.Fill( graphics, brush );
                    }
                }

                painter.PaintFull();
                painter.Draw( graphics );
            }
        }

        public static void LoadMasterPicture( PictureBox masterPictureBox, FigureDocument masterDoc )
        {
            Size picBoxSize = masterPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * GlyphImageHeightRatio);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure( masterDoc.Figure, new Size( width, height ) );

            Bitmap bitmap = new Bitmap( width, height );

            using ( var graphics = Graphics.FromImage( bitmap ) )
            using ( var painter = new SystemFigurePainter( masterDoc ) )
            {
                painter.SetTransform( graphics, rect );
                painter.PaintFull();
                painter.Fill( graphics );
            }

            masterPictureBox.Image = bitmap;
        }

        public static void LoadProgressPicture(
            PictureBox progressPictureBox,
            CharacterItem charItem,
            FigureDocument masterDoc,
            FigureItem standOutPiece,
            FigureDocument standOutPieceDoc )
        {
            Image oldImage = progressPictureBox.BackgroundImage;

            if ( oldImage != null )
            {
                progressPictureBox.BackgroundImage = null;
                oldImage.Dispose();
            }

            Size picBoxSize = progressPictureBox.ClientSize;
            int height = (int) (picBoxSize.Height * GlyphImageHeightRatio);
            int width = height;

            Rectangle rect = DrawingUtils.CenterFigure( masterDoc.Figure, new Size( width, height ) );

            Bitmap bitmap = new Bitmap( width, height );

            using ( var graphics = Graphics.FromImage( bitmap ) )
            {
                foreach ( var pieceItem in charItem.PieceFigureItems )
                {
                    FigureDocument pieceDoc;
                    bool standOut;

                    if ( pieceItem == standOutPiece )
                    {
                        standOut = true;
                        pieceDoc = standOutPieceDoc;
                    }
                    else
                    {
                        standOut = false;
                        pieceDoc = pieceItem.Open();
                    }

                    PaintPiece( pieceDoc, graphics, rect, standOut );
                }
            }

            progressPictureBox.BackgroundImage = bitmap;
            progressPictureBox.Invalidate();
        }
    }
}
