using GreatMachine.Models.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public class Cursor : BaseEntity
    {
        public Vector2 Position { get; set; }

        public Cursor(SpriteSheet spriteSheet)
        {            
            SpriteSheet = spriteSheet;
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            base.HandleInput(input, gameTime);
            Position = Vector2.Transform(
                input.MouseState.Position.ToVector2(),
                Matrix.Invert(Main.Instance.Camera.Transform));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(
                Main.Instance.Assets.DefaultFont,
                $"{Position.X}, {Position.Y}",
                new Vector2(0, 20),
                Color.Black);
        }
    }
}
