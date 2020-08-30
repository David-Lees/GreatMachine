using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace GreatMachine.Models
{
    public class Player : MoveableEntity
    {
        const double Speed = 600;

        public Player()
        {
            IsCircle = true;
        }

        public Vector2 Target { get; set; }

        public void UpdatePosition(GameTime gameTime)
        {
            var keys = Main.Instance.KeyboardInput;
            var pad = Main.Instance.GamePadInput;

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

            var walls = Main.Instance.Entities.Where(x => x is Wall).ToList();

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
    }
}
