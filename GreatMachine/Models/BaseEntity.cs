using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public abstract class BaseEntity
    {
        public Vector2 Position { get; set; }

        public int Sector { get; set; }

        public bool Invulnerable { get; set; }

        public int Health { get; set; }

        public int Lifespan { get; set; }

        public bool IsDead => !Invulnerable && Health < 0;

        public SpriteSheet SpriteSheet { get; set; }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pos = (Position - Main.Instance.ViewPortOrigin) * Main.Instance.Scale * Main.Instance.SectorSize;            

            spriteBatch.Draw(
                SpriteSheet.Texture, 
                new Rectangle(pos.ToPoint(), new Point((int)(SpriteSheet.SpriteWidth * Main.Instance.Scale), (int)(SpriteSheet.SpriteHeight * Main.Instance.Scale))), 
                SpriteSheet.SourceRectangle("Sprite000"), 
                Color.White);         
        }
    }
}
