using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public class TextEntity: BaseEntity
    {
        public string Text { get; set; }

        public TextEntity(int x, int y, string text)
        {
            Position = new Vector2(x, y);
            Invulnerable = true;
            Text = text;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pos = (Position - Main.Instance.ViewPortOrigin) * Main.Instance.Scale * Main.Instance.SectorSize;
            spriteBatch.DrawString(Main.Instance.DefaultFont, Text, pos, Color.Black);            
        }
    }    
}
