using GreatMachine.Models.ScreenSystem;
using Microsoft.Xna.Framework;
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

        public Player()
        {
            Body = Main.Instance.World.CreateCircle(48, 1f, Vector2.One * 64, BodyType.Dynamic);
            Main.Instance.World.ControllerList.OfType<VelocityLimitController>().Single().AddBody(Body);
            Body.LinearDamping = 1f;
            Body.SetRestitution(1f);
            Body.SetFriction(0.0f);
            Body.SetCollisionCategories(Category.Cat1);
            Body.SetCollidesWith(Category.Cat2 | Category.Cat3 | Category.Cat5 | Category.Cat6 | Category.Cat7);
            Body.Mass = 100f; // kg
            Body.LinearVelocity = Vector2.Zero;
            Body.Tag = this;
            Body.FixedRotation = false;
        }

        public Vector2 Target { get; set; }

        private double Cooldown = 0;

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);

            var target = Vector2.Transform(
                input.MouseState.Position.ToVector2(),
                Matrix.Invert(Main.Instance.Camera.Transform));

            var direction = target - Body.Position;
            Body.Rotation = MathF.Atan2(direction.Y, direction.X) + MathHelper.PiOver2 + 0.05f;

            if ((input.KeyboardState.IsKeyDown(Keys.Space) || input.MouseState.LeftButton == ButtonState.Pressed) && Cooldown <= 0)
            {
                Shoot(target, true);
                Cooldown = 250; // milliseconds
            }
            if ((input.KeyboardState.IsKeyDown(Keys.F) || input.MouseState.RightButton == ButtonState.Pressed) && Cooldown <= 0)
            {
                Shoot(target, false);
                Cooldown = 250; // milliseconds
            }

            var force = 5000;

            if (input.KeyboardState.IsKeyDown(Keys.Left) ||
                input.KeyboardState.IsKeyDown(Keys.A) ||
                input.GamePadState.DPad.Left == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(-force, 0));
            }

            if (input.KeyboardState.IsKeyDown(Keys.Right) ||
             input.KeyboardState.IsKeyDown(Keys.D) ||
             input.GamePadState.DPad.Right == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(force, 0));
            }

            if (input.KeyboardState.IsKeyDown(Keys.Up) ||
                input.KeyboardState.IsKeyDown(Keys.W) ||
                input.GamePadState.DPad.Up == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(0, -force));
            }

            if (input.KeyboardState.IsKeyDown(Keys.Down) ||
                input.KeyboardState.IsKeyDown(Keys.S) ||
                input.GamePadState.DPad.Down == ButtonState.Pressed)
            {
                Body.ApplyLinearImpulse(new Vector2(0, force));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            Body.LinearVelocity *= 0.1f;
        }

        public void Shoot(Vector2 target, bool bouncing)
        {
            var velocity = target - Body.Position;
            velocity.Normalize();

            var bullet = new Bullet(Body.Position + (velocity * 63), velocity)
            {
                Lifespan = 10,
                Health = 1,
                SpriteSheet = Main.Instance.Assets.BulletSheet,
                SpriteName = "Sprite001",
                IsBouncable = bouncing
            };

            Main.Instance.Entities.Add(bullet);
        }
    }
}
