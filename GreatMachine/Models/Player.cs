using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

            var pos = Position;

            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A) || pad.DPad.Left == ButtonState.Pressed)
                pos.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
                pos.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W) || pad.DPad.Up == ButtonState.Pressed)
                pos.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S) || pad.DPad.Down == ButtonState.Pressed)
                pos.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            Position = pos;
        }
    }
}
