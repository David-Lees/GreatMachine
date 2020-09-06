using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace GreatMachine.Models
{
    public class Goo : MoveableEntity
    {
        public Goo(Vector2 position, Vector2 velocity)
        {
            Body = Main.Instance.World.CreateCircle(4, 1f, position, BodyType.Dynamic);
            Body.SetRestitution(1.0f);
            Body.SetFriction(0.0f);
            Body.Tag = this;
            Body.IsBullet = true;
            Body.Mass = 0.01f;
            Body.ApplyForce(velocity * 100);
            Body.OnCollision += OnCollisionEventHandler;
            Body.SetCollisionCategories(Category.Cat6);
            Body.SetCollidesWith(Category.Cat1 | Category.Cat2);
        }

        private bool OnCollisionEventHandler(Fixture sender, Fixture other, Contact contact)
        {
            var item = other.Body.Tag;
            if (item is Player p)
            {
                p.Health -= 5;
                Destroy();
            }
            if (item is Wall)
            {
                Destroy();                
            }

            return true;
        }

    }
}
