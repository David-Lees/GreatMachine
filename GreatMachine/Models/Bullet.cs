using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace GreatMachine.Models
{
    public class Bullet : MoveableEntity
    {        
        public bool IsBouncable { get; set; }

        public Bullet(Vector2 position, Vector2 velocity)
        {            
            Body = Main.Instance.World.CreateCircle(4, 1f, position, BodyType.Dynamic);
            Body.SetRestitution(1.0f);
            Body.SetFriction(0.0f);
            Body.Tag = this;
            Body.IsBullet = true;
            Body.Mass = 0.01f;
            Body.ApplyForce(velocity * 100);
            Body.OnCollision += OnCollisionEventHandler;
            Body.SetCollisionCategories(Category.Cat5);
            Body.SetCollidesWith(Category.Cat1 | Category.Cat2 | Category.Cat3 | Category.Cat4);
        }

        private bool OnCollisionEventHandler(Fixture sender, Fixture other, Contact contact)
        {
            var item = other.Body.Tag;
            if (item is Player p) { 
                if (IsBouncable) p.Health -= 1;
                Destroy();
            }
            if (item is Crawler c)
            {
                c.Health -= 10;
                Destroy();
            }
            if (item is Spawner s)
            {
                s.Health -= 10;
                Destroy();
            }
            if (item is Wall)
            {
                if (IsBouncable)
                {
                    var effect = Main.Instance.Random.Next(0, 2) == 0 ? "Ricochet1" : "Ricochet2";
                    float pitch = (float)Main.Instance.Random.NextDouble() - 1.0f;
                    var playerPos = Main.Instance.Player.Body.Position;
                    var distance = Vector2.DistanceSquared(playerPos, Body.Position);
                    var maxSound = Main.Instance.Assets.MaxSoundDistance;
                    var volume = MathHelper.Clamp((maxSound - distance) / (maxSound * 4), 0, 0.25f);
                    var pan = MathHelper.Clamp((Body.Position.X - playerPos.X) / 500, -1.0f, 1.0f);
                    Main.Instance.Assets.SoundEffects[effect].Play(volume, pitch, pan);
                }
                else
                {
                    Destroy();
                }
            }
           
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Lifespan -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Lifespan < 0 || Body.LinearVelocity.LengthSquared() < 100) Destroy();
        }
    }
}
