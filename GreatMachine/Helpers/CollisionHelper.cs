using GreatMachine.Models;
using Microsoft.Xna.Framework;
using System;

namespace GreatMachine.Helpers
{
    [Flags]
    public enum CollisionEdge {  
        None = 0,
        Top = 1, 
        Left = 2, 
        Bottom = 4, 
        Right = 8 
    }

    public static class CollisionHelper
    {       
        public static bool Overlaps(BaseEntity e1, BaseEntity e2)
        {
            if (e1.IsCircle && e2.IsCircle)
                return CircleOnCircle(e1, e2);
            if (e1.IsCircle && !e2.IsCircle)
                return CircleOnRectangle(e1, e2, out _);
            if (!e1.IsCircle && e2.IsCircle)
                return CircleOnRectangle(e2, e1, out _);
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

        private static bool CircleOnRectangle(BaseEntity circle, BaseEntity rectangle, out CollisionEdge edge)
        {
            var L = rectangle.Position.X;
            var R = L + rectangle.BoundingBox.Width;
            var T = rectangle.Position.Y;
            var B = T + rectangle.BoundingBox.Height;

            var x = circle.Position.X;
            var y = circle.Position.Y;
            var cx = MathHelper.Clamp(x, L, R);
            var cy = MathHelper.Clamp(y, T, B);

            var dx = cx - x;
            var dy = cy - y;

            var r = circle.BoundingBox.Width / 2;
            var rs = r * r;
            var distance = dx * dx + dy * dy;             

            var colliding = (distance < rs) || ((x < R) && (L < x) && (y < T) && (B < y));

            edge = CollisionEdge.None;
            if (colliding)
            {                
                if (cy == T) edge |= CollisionEdge.Top;
                if (cy == B) edge |= CollisionEdge.Bottom;
                if (cx == L) edge |= CollisionEdge.Left;
                if (cx == R) edge |= CollisionEdge.Right;
            }

            return colliding;
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

        public static CollisionEdge CollidingEdge(BaseEntity circle, BaseEntity rectangle)
        {
            _ = CircleOnRectangle(circle, rectangle, out var edge);
            return edge;
        }
    }
}
