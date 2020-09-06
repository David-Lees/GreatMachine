using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace GreatMachine.Models
{
    public class Crawler : Enemy
    {
        private double DirectionCooldown = 0;
        private double ShootCooldown = 2;

        public Crawler(Vector2 position)
        {
            Health = 1;
            Lifespan = Main.Instance.Random.Next(600, 900); // die after 10-15 minutes, incase get stuck in wall and also limits total number
            Body = Main.Instance.World.CreateCircle(22, 1f, Vector2.One * 16, BodyType.Dynamic);
            Body.Position = position;
            Body.Mass = 2;
            Body.Tag = this;
            Body.OnCollision += OnCollisionEventHandler;
            Body.SetCollisionCategories(Category.Cat3);
            Body.SetCollidesWith(Category.Cat1 | Category.Cat2 | Category.Cat3 | Category.Cat5);

            SpriteSheet = Main.Instance.Assets.BugsSheet;
            Frames = new List<string>();
            Frames.AddRange(SpriteSheet.Index.Keys.ToList());
        }

        private bool OnCollisionEventHandler(Fixture sender, Fixture other, Contact contact)
        {
            var item = other.Body.Tag;
            if (item is Player p)
            {
                p.Health -= 10;
            }

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            FrameCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            DirectionCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            Lifespan -= gameTime.ElapsedGameTime.TotalSeconds;

            if (FrameCooldown <= 0)
            {
                FrameCooldown = 40;
                CurrentFrame++;
                CurrentFrame %= Frames.Count;
                SpriteName = Frames[CurrentFrame];
            }

            if (DirectionCooldown <= 0)
            {
                // disable cooldown until next time
                DirectionCooldown = int.MaxValue;
                Main.Instance.EnemiesRequiringPath.Enqueue(this);
            }

            ShootCooldown -= ShootCooldown > 0 ? gameTime.ElapsedGameTime.TotalSeconds : 0;

            if (Health < 0 || Lifespan < 0) Destroy();
        }

        public override void UpdateDirection()
        {
            DirectionCooldown = 400;
            var target = Main.Instance.Pathfinder.GetVector(Body.Position, out var shootAttempt);
            var vector = target - Body.Position;
            vector.Normalize();

            Body.ResetDynamics();
            Body.ApplyForce(vector * 1500);

            Body.Rotation = MathF.Atan2(vector.Y, vector.X) + MathHelper.PiOver2;

            if (shootAttempt && !HitWall() && ShootCooldown <= 0)
            {
                Shoot(Main.Instance.Player.Body.Position);                
            }
        }

        public void Shoot(Vector2 target)
        {
            var velocity = target - Body.Position;
            velocity.Normalize();

            var bullet = new Goo(Body.Position + (velocity * 31), velocity)
            {                
                Health = 1,
                SpriteSheet = Main.Instance.Assets.BulletSheet,        
                SpriteName = "Sprite000"
            };

            Main.Instance.Entities.Add(bullet);

            ShootCooldown = 2;
        }

        private bool HitWall()
        {
            var hitWall = false;

            float callback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            {
                if (fixture.Body.Tag.GetType() == typeof(Wall))
                {
                    hitWall = true;
                    return 0;
                }
                return -1;
            }

            // Summary:
            //     Ray-cast the world for all fixtures in the path of the ray. Your callback
            //     controls whether you get the closest point, any point, or n-points.  The
            //     ray-cast ignores shapes that contain the starting point.  Inside the callback:
            //     return -1: ignore this fixture and continue 
            //     return  0: terminate the ray cast
            //     return fraction: clip the ray to this point
            //     return 1:        don't clip the ray and continue
            Main.Instance.World.RayCast(callback, Body.Position, Main.Instance.Player.Body.Position);

            return hitWall;
        }
    }
}
