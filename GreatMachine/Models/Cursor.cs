using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public class Cursor : BaseEntity
    {
        public Cursor(SpriteSheet spriteSheet)
        {
            IsCircle = true;
            SpriteSheet = spriteSheet;
        }

        public override void Update(GameTime gameTime)
        {            
            Position = Vector2.Transform(
                Main.Instance.MouseInput.Position.ToVector2(), 
                Matrix.Invert(Main.Instance.Camera.Transform));            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(
                Main.Instance.DefaultFont,
                $"{Position.X}, {Position.Y}",
                new Vector2(0, 20),
                Color.Black);
        }
    }
}
