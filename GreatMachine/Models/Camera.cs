using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public float Zoom { get; set; }

        public void Follow(BaseEntity target)
        {
            var position = Matrix.CreateTranslation(
              (-target.Position.X * Main.Instance.SectorSize) - (target.BoundingBox.Width / 2),
              (-target.Position.Y * Main.Instance.SectorSize) - (target.BoundingBox.Height / 2),
              0);

            var offset = Matrix.CreateTranslation(
                Main.Instance.ScreenWidth / 2.0f,
                Main.Instance.ScreenHeight / 2.0f,
                0);

            var scale = Matrix.CreateScale(Main.Instance.Scale);

            if (Main.Instance.Zoomed)
            {
                Transform = scale;
            }
            else
            {
                Transform = position * offset;
            }
        }
    }
}
