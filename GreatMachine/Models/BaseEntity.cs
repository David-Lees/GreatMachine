using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine.Models
{
    public abstract class BaseEntity
    {

        public Body Body { get; set; }

        
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

        public float Mass { get; set; }

        public float SimTimeRemaining { get; set; }

        public List<string> Frames {  get; set; }

        public int CurrentFrame { get; set; }

        public double FrameCooldown { get; set; }

        public Rectangle BoundingBox
        {
            get
            {
                var offset = new Vector2(SpriteSheet.SpriteWidth / 2, SpriteSheet.SpriteHeight / 2);
                var pos = Body.Position - offset;
                return new Rectangle(
                    pos.ToPoint(),
                    new Point(SpriteSheet.SpriteWidth, SpriteSheet.SpriteHeight));
            }
        }

        protected BaseEntity()
        {

        }

        public string SpriteName { get; set; } = "Sprite000";

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Body != null && SpriteSheet != null)
            {
                spriteBatch.Draw(
                    SpriteSheet.Texture,
                    BoundingBox,
                    SpriteSheet.SourceRectangle(SpriteName),
                    Color.White, 
                    Body.Rotation,
                    (BoundingBox.Center - BoundingBox.Location).ToVector2() , SpriteEffects.None, 1);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            Sector = PositionHelper.GetSector(Body.Position);
        }

        public void Destroy()
        {
            Main.Instance.EntitiesToRemove.Push(this);
        }
    }
}
