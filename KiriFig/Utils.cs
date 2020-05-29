/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using KiriFig.Model;

namespace KiriFig
{
    public static class Utils
    {
        internal static float GetBisectorAngle( Point p1, Point p2, Point p3 )
        {
            // Calculate angles.

            double angle3 = Math.Atan2( p3.Y - p2.Y, p3.X - p2.X );
            double angle1 = Math.Atan2( p1.Y - p2.Y, p1.X - p2.X );

            if ( angle3 < 0 )
                angle3 += 2 * Math.PI;

            if ( angle1 < 0 )
                angle1 += 2 * Math.PI;

            if ( angle3 < angle1 )
                angle3 += 2 * Math.PI;

            double angleDiff = angle3 - angle1;

            // Calculate the bisector.

            double bisectorAngle = angle3 - (angleDiff / 2);

            if ( bisectorAngle < 0 )
                bisectorAngle += 2 * Math.PI;

            return (float) bisectorAngle;
        }

        // Calculate the dot product of the unit line segment from refPoint at angle projected onto
        // the line segment from refPoint to targetPoint.

        internal static float GetAngleDotProduct( float angle, Point refPoint, Point targetPoint )
        {
            float unitBisectorX = (float) Math.Cos( angle );
            float unitBisectorY = (float) Math.Sin( angle );

            int targetVX = targetPoint.X - refPoint.X;
            int targetVY = targetPoint.Y - refPoint.Y;

            float dotProduct = unitBisectorX * targetVX + unitBisectorY * targetVY;

            return dotProduct;
        }
    }
}
