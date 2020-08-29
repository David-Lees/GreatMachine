using Microsoft.Xna.Framework;
using System;

namespace GreatMachine.Helpers
{
    public static class PositionHelper
    {
        public static int Convert2Dto1D(int x, int y, int width)
        {
            return y * width + x;
        }

        public static int Convert2Dto1D(Vector2 vector, Vector2 size)
        {
            return (int)Math.Floor(vector.Y * size.X + vector.X);
        }

        public static Vector2 Convert1Dto2D(int offset, int width)
        {            
            int y = offset / width;
            int x = offset - y;
            return new Vector2(x, y);
        }
    }
}
