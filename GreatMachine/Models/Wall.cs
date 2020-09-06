using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine.Models
{
    public class Wall : BaseEntity
    {
        private readonly int animationDelay = Main.Instance.Random.Next(100, 200);

        private readonly Vector2 Offset;

        public Wall(int x, int y, int w, int h, SpriteSheet s)
        {
            var position = new Vector2(x, y);
            Invulnerable = true;

            Body = Main.Instance.World.CreateRectangle(w, h, 1f, position, 0, BodyType.Static);
            Body.SetRestitution(0.3f);
            Body.SetFriction(0.5f);
            Body.Tag = this;
            Body.SetCollisionCategories(Category.Cat2);

            SpriteSheet = s;
            Frames = new List<string>();
            Frames.AddRange(SpriteSheet.Index.Keys.ToList());
            if (Main.Instance.Random.Next(0, 2) < 1) Frames.Reverse();
            SpriteName = Frames.First();

            if (w == 320)
            {
                Offset = new Vector2(-160, -32);
            }
            else if (h == 320)
            {
                Offset = new Vector2(-32 , -160);
            }
            else
            {
                Offset = new Vector2(-32, -32);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            FrameCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (FrameCooldown <= 0)
            {
                FrameCooldown = animationDelay;
                CurrentFrame++;
                CurrentFrame %= Frames.Count;
                SpriteName = Frames[CurrentFrame];
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Body != null && SpriteSheet != null)
            {
                spriteBatch.Draw(
                    SpriteSheet.Texture,
                    Body.Position + Offset,
                    SpriteSheet.SourceRectangle(SpriteName),
                    Color.White,
                    Body.Rotation, Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None, 0f);
            }
        }
    }
}
