using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Pixel Coordinates in World
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Sector Number that the object resides in
        /// </summary>        
        public int Sector 
        { 
            get
            {
                var sectorSize = Main.Instance.SectorSize;
                var w = Main.Instance.SectorCountX;
                return PositionHelper.Convert2Dto1D(
                    (int)Position.X / sectorSize,
                    (int)Position.Y / sectorSize,
                    w);
            }
        }

        public bool Invulnerable { get; set; }

        public int Health { get; set; }

        public int Lifespan { get; set; }

        public bool IsDead => !Invulnerable && Health < 0;

        public SpriteSheet SpriteSheet { get; set; }

        public Rectangle BoundingBox
        {
            get
            {
                var offset = IsCircle ?
                    new Vector2(SpriteSheet.SpriteWidth / 2, SpriteSheet.SpriteHeight / 2) :
                    new Vector2(0, 0);

                var pos = Position - offset;

                return new Rectangle(
                    pos.ToPoint(), 
                    new Point(SpriteSheet.SpriteWidth, SpriteSheet.SpriteHeight));
            }
        }

        public bool IsCircle { get; set; }

        protected BaseEntity()
        {
            IsCircle = false;
        }

        public string SpriteName { get; set; } = "Sprite000";

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {                       
            spriteBatch.Draw(
                SpriteSheet.Texture,
                BoundingBox,
                SpriteSheet.SourceRectangle(SpriteName),
                Color.White);
        }
    }
}
