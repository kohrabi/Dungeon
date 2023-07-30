using Microsoft.Xna.Framework;
using Nez;
using System;

namespace NezTopDown
{
    public class Utils
    {
        public static float AngleDifference(float angle1, float angle2)
        {
            float diff = angle1 - angle2;
            if (Math.Abs(diff) <= 180)
                return diff;
            else
                return diff + (360 * -diff / Math.Abs(diff));
        }

        public static float PointDirection(Vector2 from, Vector2 to)
        {
            return (Mathf.Atan2(to.Y - from.Y, to.X - from.X) * Mathf.Rad2Deg) + 180;
        }

        public static float LengthDir_X(float len, float dir)
        {
            return (float)Math.Cos(dir * Mathf.Deg2Rad) * len;
        }

        public static float LengthDir_Y(float len, float dir)
        {
            return (float)Math.Sin(dir * Mathf.Deg2Rad) * len;
        }
    }
}
