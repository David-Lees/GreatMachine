using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using tainicom.Aether.Physics2D.Controllers;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine.Models
{
    public class Player : MoveableEntity
    {

        public int PreviousSector { get; set; }

        public Player(float radius)
        {
            Body = Main.Instance.World.CreateCircle(radius, 1f, new Vector2(-100, -100), BodyType.Dynamic);
            Main.Instance.World.ControllerList.OfType<VelocityLimitController>().Single().AddBody(Body);
            Body.LinearDamping = 1f;
            Body.SetRestitution(1f);
            Body.SetFriction(0.0f);
            Body.Mass = 100f; // kg
            Body.LinearVelocity = Vector2.Zero;
            Body.Tag = this;
            Body.FixedRotation = false;
        }

        public Vector2 Target { get; set; }

        private int Cooldown = 0;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keys = Main.Instance.KeyboardInput;
            var pad = Main.Instance.GamePadInput;
            var mouse = Main.Instance.MouseInput;

            var target = Vector2.Transform(
                Main.Instance.MouseInput.Position.ToVector2(),
                Matrix.Invert(Main.Instance.Camera.Transform));

            var direction = target - Body.Position;
            Body.Rotation = MathF.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;

            if ((keys.IsKeyDown(Keys.Space) || mouse.LeftButton == ButtonState.Pressed) && Cooldown == 0)
            {
                Shoot(target, true);
                Cooldown = 250; // milliseconds
            }
            if ((keys.IsKeyDown(Keys.F) || mouse.RightButton == ButtonState.Pressed) && Cooldown == 0)
            {
                Shoot(target, false);
                Cooldown = 250; // milliseconds
            }

            Cooldown = (int)Math.Max(Cooldown - gameTime.ElapsedGameTime.TotalMilliseconds, 0);

            var force = 5000;
                        
            Body.LinearVelocity *= 0.1f;

            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A) || pad.DPad.Left == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(-force, 0));
            }

            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(force, 0));
            }

            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W) || pad.DPad.Up == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(0, -force));
            }

            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S) || pad.DPad.Down == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(0, force));
            }
        }

        public void Shoot(Vector2 target, bool bouncing)
        {
            var velocity = target - Body.Position;
            velocity.Normalize();

            var bullet = new Bullet(Body.Position + (velocity * 37), velocity)
            {
                Lifespan = 10,
                Health = 1,
                SpriteSheet = Main.Instance.Assets.BulletSheet,
                IsBouncable = bouncing
            };

            Main.Instance.Entities.Add(bullet);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            base.Draw(gameTime, spriteBatch);

            var healthPos = Body.Position + new Vector2(-SpriteSheet.Radius, SpriteSheet.Radius);
            spriteBatch.DrawString(Main.Instance.Assets.DefaultFont, $"{Health}", healthPos, Health > 10 ? Color.Green : Color.Red);
            spriteBatch.DrawString(Main.Instance.Assets.DefaultFont, $"{Health}", healthPos + Vector2.One * 2, Color.Black);            
        }
    }
}
