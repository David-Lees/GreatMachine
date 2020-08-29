using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GreatMachine.Models
{
    public class Player : MoveableEntity
    {
        const double Speed = 1;

        public Vector2 Target { get; set; }

        public void UpdatePosition(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            var pos = Position;

            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A))
                pos.X -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D))
                pos.X += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W))
                pos.Y -= (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                pos.Y += (float)(Speed * gameTime.ElapsedGameTime.TotalSeconds);

            Position = pos;
        }
    }
}
