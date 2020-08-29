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

        public Rectangle BoundingBox { get; set; }

        public bool IsCircle { get; set; } = false;

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pos = Position * Main.Instance.SectorSize;

            BoundingBox = new Rectangle(pos.ToPoint(), new Point(SpriteSheet.SpriteWidth, SpriteSheet.SpriteHeight));

            spriteBatch.Draw(
                SpriteSheet.Texture, 
                BoundingBox, 
                SpriteSheet.SourceRectangle("Sprite000"), 
                Color.White);         
        }
    }
}
