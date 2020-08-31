using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{
    public class Wall : BaseEntity
    {
        public Wall(int x, int y)
        {
            Position = new Vector2(x, y);
            Invulnerable = true;            
        }
    }
}
