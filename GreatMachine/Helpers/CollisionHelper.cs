using GreatMachine.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace GreatMachine.Helpers
{
    [Flags]
    public enum CollisionEdge
    {
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


        private static bool DoCirclesOverlap(float x1, float y1, float r1, float x2, float y2, float r2)
        {
            return MathF.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) <= ((r1 + r2) * (r1 + r2));
        }

        private static bool IsPointInCircle(float x1, float y1, float r1, float px, float py)
        {
            return MathF.Abs((x1 - px) * (x1 - px) + (y1 - py) * (y1 - py)) < (r1 * r1);
        }


        private static bool CircleOnCircle(BaseEntity e1, BaseEntity e2)
        {
            var r1 = e1.SpriteSheet.Radius;
            var r2 = e2.SpriteSheet.Radius;
            var dx = e1.Position.X - e2.Position.X;
            var dy = e1.Position.Y - e2.Position.Y;
            var distance = dx * dx + dy * dy;
            return distance <= (r1 + r2) * (r1 + r2);
        }



        public static Tuple<Vector2, Vector2> NearestEdge(Vector2 vector, Rectangle rectangle)
        {
            var L = rectangle.X;
            var R = L + rectangle.Width;
            var T = rectangle.Y;
            var B = T + rectangle.Height;

            var cx = MathHelper.Clamp(vector.X, L, R);
            var cy = MathHelper.Clamp(vector.Y, T, B);

            if (cy == T) return new Tuple<Vector2, Vector2>(new Vector2(L, T), new Vector2(R, T));
            if (cy == B) return new Tuple<Vector2, Vector2>(new Vector2(L, B), new Vector2(R, B));
            if (cx == L) return new Tuple<Vector2, Vector2>(new Vector2(L, T), new Vector2(L, B));
            if (cx == R) return new Tuple<Vector2, Vector2>(new Vector2(R, T), new Vector2(R, B));
            return null;
        }

        public static Vector2? LinesIntersectAt(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            var E = B - A;
            var F = D - C;
            var P = new Vector2(-E.X, E.Y);
            var h = Vector2.Dot(A - C, P) / Vector2.Dot(F, P);

            if (h >= 0 && h <= 1)
            {
                // the lines intersect
                return C + F * h;
            }
            return null;
        }

        public static Vector2? PathCollides(Vector2 start, Vector2 end, Rectangle r)
        {
            var nearestLine = NearestEdge(start, r);
            return LinesIntersectAt(start, end, nearestLine.Item1, nearestLine.Item2);
        }

        public static Vector2 CalculateRebounds(MoveableEntity e, ICollection<BaseEntity> walls)
        {
            var destination = e.Position + e.Velocity;
            var radius = (int)Math.Ceiling(e.SpriteSheet.Radius);
            var source = e.Position;
            destination = CalculateRebounds(source, destination, radius, walls);
            return destination;
        }

        private static Vector2 CalculateRebounds(Vector2 start, Vector2 destination, int radius, ICollection<BaseEntity> walls, int rebounds = 0)
        {
            //var travel = destination - start;
            //travel.Normalize();
            //var offset = travel * radius;
            //var B = destination + offset;
            //foreach (var wall in walls)
            //{
            //    var nearestLine = NearestEdge(start, wall.BoundingBox);
            //    if (nearestLine != null)
            //    {
            //        var collision = LinesIntersectAt(start, B, nearestLine.Item1, nearestLine.Item2);
            //        if (collision.HasValue)
            //        {
            //            var bouncePoint = collision - offset;
            //            Vector2 Rebound =
            //            if (nearestLine.Item1.X == nearestLine.Item2.X)
            //            {
            //                rebound.X = -rebound.X;
            //            }
            //            else
            //            {
            //                rebound.Y = -rebound.Y;
            //            }
            //            destination = collision.Value + rebound;
            //            destination = CalculateRebounds(collision.Value, destination, radius, walls.Where(e => e != wall).ToList(), rebounds++);
            //        }
            //    }
            //}
            return destination;
        }

        public static CollisionEdge CollidingEdge(BaseEntity circle, BaseEntity rectangle)
        {
            _ = CircleOnRectangle(circle, rectangle, out var edge);
            return edge;
        }



        public static void CalculateCollisions(ICollection<BaseEntity> entities, GameTime gameTime)
        {
            var CollidingPairs = new List<Tuple<MoveableEntity, MoveableEntity>>();
            var fakeBalls = new List<MoveableEntity>();

            // Threshold indicating stability of object
            float stable = 0.05f;

            // Multiple simulation updates with small time steps permit more accurate physics
            // and realistic results at the expense of CPU time of course
            int SimulationUpdates = 1;

            // Multiple collision trees require more steps to resolve. Normally we would
            // continue simulation until the object has no simulation time left for this
            // epoch, however this is risky as the system may never find stability, so we
            // can clamp it here
            int MaxSimulationSteps = 15;

            // Break up the frame elapsed time into smaller deltas for each simulation update
            float SimElapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / SimulationUpdates;

            var lines = Main.Instance.Lines;

            var moveables = entities
                .OfType<MoveableEntity>()
                .ToList();

            // Main simulation loop
            for (int i = 0; i < SimulationUpdates; i++)
            {
                // Set all moveables time to maximum for this epoch
                foreach (var m in moveables)
                {
                    m.SimTimeRemaining = SimElapsedTime;
                }

                // Erode simulation time on a per object basis, depending upon what happens
                // to it during its journey through this epoch
                for (int j = 0; j < MaxSimulationSteps; j++)
                {
                    // Update Ball Positions
                    foreach (var item in moveables)
                    {
                        if (item.SimTimeRemaining > 0.0f)
                        {
                            item.OldPosition = new Vector2(item.Position.X, item.Position.Y);
                            item.Acceleration = new Vector2(-item.Velocity.X * 0.99f, -item.Velocity.Y * 0.99f);
                            item.Velocity = new Vector2(
                                item.Velocity.X + item.Acceleration.X * item.SimTimeRemaining,
                                item.Velocity.Y + item.Acceleration.Y * item.SimTimeRemaining);

                            item.Position = new Vector2(
                                item.Position.X + item.Velocity.X * item.SimTimeRemaining,
                                item.Position.Y + item.Velocity.Y * item.SimTimeRemaining);

                            // Stop when velocity is neglible
                            if (MathF.Abs(item.Velocity.X * item.Velocity.X + item.Velocity.Y * item.Velocity.Y) < stable)
                            {
                                item.Velocity = Vector2.Zero;
                            }
                        }
                    }

                    // Work out static collisions with walls and displace balls so no overlaps
                    foreach (var item in moveables)
                    {
                        // Against Edges
                        foreach (var edge in lines.Where(x => item.SurroundingSectors.Contains(x.Sector)))
                        {
                            float lineX1 = edge.End.X - edge.Start.X;
                            float lineY1 = edge.End.Y - edge.Start.Y;

                            float lineX2 = item.Position.X - edge.Start.X;
                            float lineY2 = item.Position.Y - edge.Start.Y;

                            float edgeLength = lineX1 * lineX1 + lineY1 * lineY1;

                            // This is nifty - It uses the DP of the line segment vs the line to the object, to work out
                            // how much of the segment is in the "shadow" of the object vector. The min and max clamp
                            // this to lie between 0 and the line segment length, which is then normalised. We can
                            // use this to calculate the closest point on the line segment
                            float t = MathF.Max(0, MathF.Min(edgeLength, lineX1 * lineX2 + lineY1 * lineY2)) / edgeLength;

                            // Which we do here
                            float closestPointX = edge.Start.X + t * lineX1;
                            float closestPointY = edge.Start.Y + t * lineY1;

                            // And once we know the closest point, we can check if the ball has collided with the segment in the
                            // same way we check if two balls have collided
                            float distance = MathF.Sqrt((item.Position.X - closestPointX) * (item.Position.X - closestPointX) + (item.Position.Y - closestPointY) * (item.Position.Y - closestPointY));

                            if (distance <= (item.SpriteSheet.Radius + edge.Radius))
                            {
                                // Collision has occurred - treat collision point as a ball that cannot move. To make this
                                // compatible with the dynamic resolution code below, we add a fake ball with an infinite mass
                                // so it behaves like a solid object when the momentum calculations are performed
                                MoveableEntity fakeball = new MoveableEntity()
                                {
                                    SpriteSheet = new SpriteSheet((int)edge.Radius, (int)edge.Radius),
                                    Mass = 1000000, //item.Mass * 0.8f,
                                    Position = new Vector2(closestPointX, closestPointY),
                                    // We will use these later to allow the lines to impart energy into ball
                                    // if the lines are moving, i.e. like pinball flippers
                                    Velocity = new Vector2(-item.Velocity.X, -item.Velocity.Y)
                                };

                                // Store Fake Ball
                                fakeBalls.Add(fakeball);

                                // Add collision to vector of collisions for dynamic resolution
                                CollidingPairs.Add(new Tuple<MoveableEntity, MoveableEntity>(item, fakeball));

                                // Calculate displacement required
                                float overlap = 1.0f * (distance - item.SpriteSheet.Radius - fakeball.SpriteSheet.Radius);

                                // Displace Current Ball away from collision                          
                                item.Position = new Vector2(
                                    item.Position.X - overlap * (item.Position.X - fakeball.Position.X) / distance,
                                    item.Position.Y - overlap * (item.Position.Y - fakeball.Position.Y) / distance);
                            }
                        }

                        // Against other moveables
                        foreach (var target in moveables.Where(x => item.SurroundingSectors.Contains(x.Sector)))
                        {
                            // Do not check against self
                            if (item != target && DoCirclesOverlap(item.Position.X, item.Position.Y, item.SpriteSheet.Radius, target.Position.X, target.Position.Y, target.SpriteSheet.Radius))
                            {
                                // Collision has occured
                                CollidingPairs.Add(new Tuple<MoveableEntity, MoveableEntity>(item, target));

                                // Distance between ball centers
                                float distance = Vector2.Distance(item.Position, target.Position);
                                
                                // Calculate displacement required
                                float overlap = 0.5f * (distance - item.SpriteSheet.Radius - target.SpriteSheet.Radius);

                                // Displace Current Ball away from collision
                                item.Position = new Vector2(
                                    item.Position.X - overlap * (item.Position.X - target.Position.X) / distance,
                                    item.Position.Y - overlap * (item.Position.Y - target.Position.Y) / distance);

                                // Displace Target Ball away from collision - Note, this should affect the timing of the target ball
                                // and it does, but this is absorbed by the target ball calculating its own time delta later on
                                target.Position = new Vector2(
                                    target.Position.X + overlap * (item.Position.X - target.Position.X) / distance,
                                    target.Position.Y + overlap * (item.Position.Y - target.Position.Y) / distance);
                            }
                        }

                        // Time displacement - we knew the velocity of the ball, so we can estimate the distance it should have covered
                        // however due to collisions it could not do the full distance, so we look at the actual distance to the collision
                        // point and calculate how much time that journey would have taken using the speed of the object. Therefore
                        // we can now work out how much time remains in that timestep.
                        float intendedSpeed = MathF.Sqrt(item.Velocity.X * item.Velocity.X + item.Velocity.Y * item.Velocity.Y);                        
                        float actualDistance = Vector2.Distance(item.Position, item.OldPosition);
                        float actualTime = actualDistance / intendedSpeed;

                        // After static resolution, there may be some time still left for this epoch, so allow simulation to continue
                        item.SimTimeRemaining -= actualTime;
                    }

                    // Now work out dynamic collisions
                    float efficiency = 1.00f;
                    foreach (var c in CollidingPairs)
                    {
                        var b1 = c.Item1;
                        var b2 = c.Item2;

                        // Distance between balls
                        float distance = Vector2.Distance(b1.Position, b2.Position);

                        // Normal
                        float nx = (b2.Position.X - b1.Position.X) / distance;
                        float ny = (b2.Position.Y - b1.Position.Y) / distance;

                        // Tangent
                        float tx = -ny;
                        float ty = nx;

                        // Dot Product Tangent
                        float dpTan1 = b1.Velocity.X * tx + b1.Velocity.Y * ty;
                        float dpTan2 = b2.Velocity.X * tx + b2.Velocity.Y * ty;

                        // Dot Product Normal
                        float dpNorm1 = b1.Velocity.X * nx + b1.Velocity.Y * ny;
                        float dpNorm2 = b2.Velocity.X * nx + b2.Velocity.Y * ny;

                        // Conservation of momentum in 1D
                        float m1 = efficiency * (dpNorm1 * (b1.Mass - b2.Mass) + 2.0f * b2.Mass * dpNorm2) / (b1.Mass + b2.Mass);
                        float m2 = efficiency * (dpNorm2 * (b2.Mass - b1.Mass) + 2.0f * b1.Mass * dpNorm1) / (b1.Mass + b2.Mass);

                        // Update ball velocities
                        b1.Velocity = new Vector2(tx * dpTan1 + nx * m1, ty * dpTan1 + ny * m1);
                        b2.Velocity = new Vector2(tx * dpTan2 + nx * m2, ty * dpTan2 + ny * m2);
                    }

                    // Remove collisions
                    CollidingPairs.Clear();

                    // Remove fake balls                
                    fakeBalls.Clear();
                }

            }
        }

        public static void CalculateCollisions(GameTime gameTime, BaseEntity entities, List<LineSegment> lines)
        {

        }


    }
}
