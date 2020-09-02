using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine.Models
{
    public class Wall : BaseEntity
    {
        public Wall(int x, int y)
        {
            var position = new Vector2(x, y);
            Invulnerable = true;

            Body = Main.Instance.World.CreateRectangle(64, 64, 1f, position, 0, BodyType.Static);
            Body.SetRestitution(0.3f);
            Body.SetFriction(0.5f);
            Body.Tag = this;
        }
    }
}
