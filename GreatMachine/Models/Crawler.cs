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

        public Crawler(Vector2 position)
        {
            Health = 1;
            Lifespan = 5 * 60; // die after 5 minutes, incase get stuck in wall and also limits total number
            Body = Main.Instance.World.CreateCircle(22, 1f, Vector2.One * 16, BodyType.Dynamic);
            Body.Position = position;
            Body.Mass = 2;
            Body.Tag = this;
            Body.OnCollision += OnCollisionEventHandler;

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

            if (Health < 0 || Lifespan < 0) Destroy();
        }

        public override void UpdateDirection()
        {
            DirectionCooldown = 400;
            var target = Main.Instance.Pathfinder.GetVector(Body.Position);
            var vector = target - Body.Position;
            vector.Normalize();

            Body.ResetDynamics();
            Body.ApplyForce(vector * 1500);

            Body.Rotation = MathF.Atan2(vector.Y, vector.X) + MathHelper.PiOver2;
        }
    }
}
