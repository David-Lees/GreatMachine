using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{
    public class MoveableEntity : BaseEntity
    {
        public int LinearVelocity { get; set; }

        public int Direction { get; set; }

        public Vector2 OrthagonalVelocity { get; set; }
    }
}
