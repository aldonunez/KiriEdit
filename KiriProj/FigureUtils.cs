/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFig;
using KiriFig.Model;
using KiriFT;
using System.Collections.Generic;
using System.IO;

namespace KiriProj
{
    public static class FigureUtils
    {
        public static Figure MakeMasterFigure( string fontPath, int faceIndex, uint character )
        {
            using ( var lib = new FontLibrary() )
            using ( var face = lib.OpenFace( fontPath, faceIndex ) )
            {
                Figure figure = face.DecomposeGlyph( character );
                return figure;
            }
        }

        public static void CompileProject( Project project, string path )
        {
            ProjectCompiler.CompileProject( project, path );
        }
    }

    internal class ProjectCompiler
    {
        public static void CompileProject( Project project, string path )
        {
            using ( var writer = File.CreateText( path ) )
            {
                writer.WriteLine( "charset" );

                foreach ( var charItem in project.Characters )
                {
                    WriteGlyph( writer, charItem );
                }

                writer.WriteLine( "end charset" );
            }
        }

        private static void WriteGlyph( StreamWriter writer, CharacterItem charItem )
        {
            List<Figure> figures = CalcFigures( charItem );

            if ( figures.Count == 0 )
                return;

            writer.WriteLine( "glyph {0}", charItem.CodePoint );

            var masterDoc = charItem.MasterFigureItem.Open();
            var masterFig = masterDoc.Figure;

            writer.WriteLine(
                " fbox {0} {1} {2} {3}",
                masterFig.OffsetX,
                masterFig.OffsetY,
                masterFig.Width,
                masterFig.Height );

            WriteMaster( writer, masterFig );

            foreach ( var pieceFigure in figures )
            {
                WritePiece( writer, pieceFigure );
            }

            writer.WriteLine( "end glyph" );
        }

        private static List<Figure> CalcFigures( CharacterItem charItem )
        {
            var figures = new List<Figure>();

            foreach ( var piece in charItem.PieceFigureItems )
            {
                var figureDoc = piece.Open();
                var figure = figureDoc.Figure;

                foreach ( var shape in figure.Shapes )
                {
                    if ( shape.Enabled )
                    {
                        figures.Add( figure );
                        break;
                    }
                }
            }

            return figures;
        }

        private static FTBBox CalcBbox( Figure figure, bool allShapes )
        {
            int left = int.MaxValue, right = int.MinValue;
            int bottom = int.MaxValue, top = int.MinValue;

            foreach ( var shape in figure.Shapes )
            {
                if ( !allShapes && !shape.Enabled )
                    continue;

                Contour contour = shape.Contours[0];
                Point p = contour.FirstPoint;

                do
                {
                    var edge = p.OutgoingEdge;

                    for ( double t = 0; t < 1; t += 1.0 / 20 )
                    {
                        TestBounds( edge.Calculate( t ) );
                    }

                    TestBounds( edge.Calculate( 1.0 ) );

                    p = edge.P2;
                }
                while ( p != contour.FirstPoint );
            }

            FTBBox bbox = new FTBBox()
            {
                Left = left,
                Right = right,
                Bottom = bottom,
                Top = top
            };

            return bbox;

            void TestBounds( PointD pointD )
            {
                if ( pointD.X < left )
                    left = (int) pointD.X;
                if ( pointD.Y < bottom )
                    bottom = (int) pointD.Y;
                if ( pointD.X > right )
                    right = (int) pointD.X;
                if ( pointD.Y > top )
                    top = (int) pointD.Y;
            }
        }

        private static void WriteBbox( StreamWriter writer, Figure figure, bool allShapes )
        {
            FTBBox bbox = CalcBbox( figure, allShapes );

            int width = bbox.Right - bbox.Left + 1;
            int height = bbox.Top - bbox.Bottom + 1;

            writer.WriteLine(
                "  bbox {0} {1} {2} {3}",
                bbox.Left, bbox.Bottom, width, height );
        }

        private static void WriteMaster( StreamWriter writer, Figure master )
        {
            writer.WriteLine( " master" );
            WriteBbox( writer, master, true );

            foreach ( var shape in master.Shapes )
            {
                WriteShape( writer, shape );
            }

            writer.WriteLine( " end master" );
        }

        private static void WritePiece( StreamWriter writer, Figure figure )
        {
            writer.WriteLine( " piece" );
            WriteBbox( writer, figure, false );

            foreach ( var shape in figure.Shapes )
            {
                if ( shape.Enabled )
                    WriteShape( writer, shape );
            }

            writer.WriteLine( " end piece" );
        }

        private static void WriteShape( StreamWriter writer, Shape shape )
        {
            writer.WriteLine( "  shape" );

            foreach ( var contour in shape.Contours )
            {
                writer.WriteLine( "   M {0} {1}", contour.FirstPoint.X, contour.FirstPoint.Y );

                Point p = contour.FirstPoint;

                do
                {
                    var edge = p.OutgoingEdge;

                    switch ( edge )
                    {
                        case LineEdge line:
                            writer.WriteLine( "   L {0} {1}", line.P2.X, line.P2.Y );
                            break;

                        case ConicEdge conic:
                            writer.WriteLine( "   Q {0} {1} {2} {3}",
                                conic.C1.X, conic.C1.Y, conic.P2.X, conic.P2.Y );
                            break;

                        case CubicEdge cubic:
                            writer.WriteLine( "   C {0} {1} {2} {3} {4} {5}",
                                cubic.C1.X, cubic.C1.Y,
                                cubic.C2.X, cubic.C2.Y,
                                cubic.P2.X, cubic.P2.Y );
                            break;
                    }

                    p = edge.P2;
                }
                while ( p != contour.FirstPoint );
            }

            writer.WriteLine( "  end shape" );
        }
    }
}
