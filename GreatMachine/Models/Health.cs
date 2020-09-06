using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace GreatMachine.Models
{
    public class Health : BaseEntity
    {
        public Health(Vector2 position)
        {
            Body = Main.Instance.World.CreateCircle(31, 1f, Vector2.One * 16, BodyType.Static);
            Body.Position = position;
            Body.Mass = 1;
            Body.Tag = this;
            Body.SetCollisionCategories(Category.Cat7);
            Body.OnCollision += OnCollisionEventHandler;
            SpriteSheet = Main.Instance.Assets.HealthSheet;
        }

        private bool OnCollisionEventHandler(Fixture sender, Fixture other, Contact contact)
        {
            var item = other.Body.Tag;
            if (item is Player p)
            {
                p.Health = 100;
                Destroy();
            }
            return true;
        }

    }
}
