using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace GreatMachine.Models
{
    public class Player : MoveableEntity
    {
        const double Speed = 500;

        public Player()
        {
            IsCircle = true;
        }

        public Vector2 Target { get; set; }

        private int Cooldown = 0;

        public override void Update(GameTime gameTime)
        {
            var keys = Main.Instance.KeyboardInput;
            var pad = Main.Instance.GamePadInput;
            var mouse = Main.Instance.MouseInput;

            if ((keys.IsKeyDown(Keys.Space) || mouse.LeftButton == ButtonState.Pressed) && Cooldown == 0)
            {
                Shoot();
                Cooldown = 250; // milliseconds
            }
            Cooldown = (int)Math.Max(Cooldown - gameTime.ElapsedGameTime.TotalMilliseconds, 0);

            var pos = new Vector2(Position.X, Position.Y);
            var originalPos = new Vector2(Position.X, Position.Y);

            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A) || pad.DPad.Left == ButtonState.Pressed)
                pos.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
                pos.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W) || pad.DPad.Up == ButtonState.Pressed)
                pos.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S) || pad.DPad.Down == ButtonState.Pressed)
                pos.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            Position = pos;
            
            var walls = Main.Instance.Entities
                .Where(x => x is Wall && SurroundingSectors.Contains(x.Sector))
                .ToList();

            var overlaps = walls.Where(x => CollisionHelper.Overlaps(this, x));
            var overlapping = overlaps.Any();

            if (overlapping)
            {
                Position = originalPos;
                pos = originalPos;

                // force player away from obstruction, if we are still overlapping
                foreach (var overlap in overlaps)
                {
                    var edge = CollisionHelper.CollidingEdge(this, overlap);
                    var offset = (SpriteSheet.SpriteWidth / 2) + 1;
                    if (edge.HasFlag(CollisionEdge.Top))
                        pos.Y = overlap.BoundingBox.Y - offset;
                    if (edge.HasFlag(CollisionEdge.Bottom))
                        pos.Y = overlap.BoundingBox.Y + overlap.BoundingBox.Height + offset;
                    if (edge.HasFlag(CollisionEdge.Left))
                        pos.X = overlap.BoundingBox.X - offset;
                    if (edge.HasFlag(CollisionEdge.Right))
                        pos.X = overlap.BoundingBox.X + overlap.BoundingBox.Width + offset;
                }
            }
            Position = pos;
        }

        public void Shoot()
        {
            var target = Vector2.Transform(
                Main.Instance.MouseInput.Position.ToVector2(),
                Matrix.Invert(Main.Instance.Camera.Transform));

            var velocity = target - Position;
            velocity.Normalize();

            var bullet = new Bullet
            {
                Velocity = velocity,
                Position = Position + (velocity * 37), // give enough distance so no overlap
                Lifespan = 5,
                Health = 1,
                IsCircle = true,
                SpriteSheet = Main.Instance.BulletSheet
            };

            Main.Instance.Entities.Add(bullet);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(Main.Instance.DefaultFont, $"{Health} {Position.X}, {Position.Y}", Position, Color.Black);
        }


    }
}
