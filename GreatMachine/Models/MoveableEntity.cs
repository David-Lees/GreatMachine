using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{
    public class MoveableEntity : BaseEntity
    {
        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                if (!float.IsNaN(value.X) && !float.IsNaN(value.Y))
                    // make sure we don't end up with infinite velocities
                    _velocity = new Vector2(
                        MathHelper.Clamp(value.X, -100, 100), 
                        MathHelper.Clamp(value.Y, -100, 100));
            }
        }

        private Vector2 _acceleration;
        public Vector2 Acceleration
        {
            get
            {
                return _acceleration;
            }
            set
            {
                if (!float.IsNaN(value.X) && !float.IsNaN(value.Y))
                    _acceleration = value;
            }
        }

        public Vector2 OldPosition { get; set; }
    }
}
