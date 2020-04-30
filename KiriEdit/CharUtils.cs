/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System.Text;

namespace KiriEdit
{
    public static class CharUtils
    {
        public static int GetCodePointCount( string s )
        {
            int byteCount = Encoding.UTF32.GetByteCount( s );
            return byteCount / 4;
        }

        public static uint GetCodePoint( string s )
        {
            byte[] codePointBytes = Encoding.UTF32.GetBytes( s );

            uint codePoint =
                (uint) codePointBytes[0] << 0 |
                (uint) codePointBytes[1] << 8 |
                (uint) codePointBytes[2] << 16 |
                (uint) codePointBytes[3] << 24;

            return codePoint;
        }

        public static string GetString( uint codePoint )
        {
            byte[] codePointBytes = new byte[4];

            codePointBytes[0] = (byte) ((codePoint >> 0) & 0xFF);
            codePointBytes[1] = (byte) ((codePoint >> 8) & 0xFF);
            codePointBytes[2] = (byte) ((codePoint >> 16) & 0xFF);
            codePointBytes[3] = (byte) ((codePoint >> 24) & 0xFF);

            return Encoding.UTF32.GetString( codePointBytes );
        }
    }
}
