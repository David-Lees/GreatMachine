﻿using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{

    public enum ZoomLevel
    {
        Close = 0,
        Normal = 1,
        Far = 2
    }

    public class Camera
    {
        public Matrix Transform { get; private set; }

        public ZoomLevel Zoom { get; set; }

        public void Follow(BaseEntity target)
        {
            var position = Matrix.CreateTranslation(
              -target.Body.Position.X - (target.BoundingBox.Width / 2),
              -target.Body.Position.Y - (target.BoundingBox.Height / 2),
              0);

            var offset = Matrix.CreateTranslation(
                Main.Instance.GraphicsDevice.Viewport.Width / 2.0f,
                Main.Instance.GraphicsDevice.Viewport.Height / 2.0f,
                0);

            // var scale = Matrix.CreateScale(Main.Instance.Scale)
            var half = Matrix.CreateScale(0.5f);
            var quater = Matrix.CreateScale(0.25f);

            Transform = Zoom switch
            {
                ZoomLevel.Close => position * offset,
                ZoomLevel.Normal => position * half * offset,
                ZoomLevel.Far => position * quater * offset,
                _=> position * offset,
            };
        }
    }
}
