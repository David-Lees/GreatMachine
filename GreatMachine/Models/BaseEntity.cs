using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GreatMachine.Models
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Pixel Coordinates in World
        /// </summary>
        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Sector = PositionHelper.GetSector(Position);
                SurroundingSectors = PositionHelper.SurroundingSectors(Position);
            }
        }
        
        public ICollection<int> SurroundingSectors { get; private set; }

        /// <summary>
        /// Sector Number that the object resides in
        /// </summary>        
        public int Sector { get; private set; }

        public bool Invulnerable { get; set; }

        public int Health { get; set; }

        public double Lifespan { get; set; }

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

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
