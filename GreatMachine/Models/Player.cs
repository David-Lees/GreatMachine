using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace GreatMachine.Models
{
    public class Player : MoveableEntity
    {
        const double Speed = 10;

        public Vector2 Target { get; set; }

        public void UpdatePosition(GameTime gameTime)
        {
            var keys = Main.Instance.KeyboardInput;
            var pad = Main.Instance.GamePadInput;

            var pos = new Vector2(Position.X, Position.Y);
           
            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A) || pad.DPad.Left == ButtonState.Pressed)
            {                                
                pos.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
                pos.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W) || pad.DPad.Up == ButtonState.Pressed)
                pos.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S) || pad.DPad.Down == ButtonState.Pressed)
                pos.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            var walls = Main.Instance.Entities.Where(x => x is Wall).ToList();
            var canMove = !walls.Any(x => CollisionHelper.Overlaps(this, x));

            if (canMove) Position = pos;
        }
    }
}
