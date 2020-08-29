using GreatMachine.Models;
using Microsoft.Xna.Framework;

namespace GreatMachine.Helpers
{
    public static class CollisionHelper
    {       
        public static bool Overlaps(BaseEntity e1, BaseEntity e2)
        {
            if (e1.IsCircle && e2.IsCircle)
                return CircleOnCircle(e1, e2);
            if (e1.IsCircle && !e2.IsCircle)
                return CircleOnRectangle(e1, e2);
            if (!e1.IsCircle && e2.IsCircle)
                return CircleOnRectangle(e2, e1);
            return RectangleOnRectangle(e1, e2);
        }

        private static bool RectangleOnRectangle(BaseEntity e1, BaseEntity e2)
        {
            var L1 = e1.Position.X;
            var R1 = L1 + e1.BoundingBox.Width;
            var L2 = e2.Position.X;
            var R2 = L2 + e2.BoundingBox.Width;           
            var T1 = e1.Position.Y;
            var B1 = T1 + e1.BoundingBox.Height;
            var T2 = e2.Position.Y;
            var B2 = T2 + e2.BoundingBox.Height;
            return (L2 < R1) && (L1 < R2) && (B2 < T1) && (B1 < T2);
        }

        private static bool CircleOnRectangle(BaseEntity circle, BaseEntity rectangle)
        {
            var L = rectangle.Position.X;
            var R = L + rectangle.BoundingBox.Width;
            var T = rectangle.Position.Y;
            var B = T + rectangle.BoundingBox.Height;

            var x = circle.Position.X;
            var y = circle.Position.Y;
            var cx = MathHelper.Clamp(x, L, R);
            var cy = MathHelper.Clamp(y, T, B);

            var r = circle.BoundingBox.Width / 2;
            var rs = r * r;
            var distance = cx * cx + cy * cy;             
            return (distance < rs) || ((x < R) && (L < x) && (y < T) && (B < y));
        }

        private static bool CircleOnCircle(BaseEntity e1, BaseEntity e2)
        {
            var r1 = e1.BoundingBox.Width / 2;
            var r1s = r1 * r1;
            var r2 = e2.BoundingBox.Width / 2;
            var r2s = r2 * r2;
            var dx = e1.Position.X - e2.Position.X;
            var dy = e1.Position.Y - e2.Position.Y;
            var distance = dx * dx + dy * dy;
            return distance < r1s + r2s;
        }
    }
}
