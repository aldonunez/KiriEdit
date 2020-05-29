/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFig;
using KiriFig.Model;

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
    }
}
