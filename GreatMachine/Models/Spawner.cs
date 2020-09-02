using Microsoft.Xna.Framework;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine.Models
{
    public class Spawner : Enemy
    {
        public Spawner(Vector2 position)
        {
            Health = 1;
            Body = Main.Instance.World.CreateCircle(22, 1f, Vector2.One * 32, BodyType.Static);
            Body.Position = position;
            Body.Mass = 200;
            Body.Tag = this;

            SpriteSheet = Main.Instance.Assets.SpawnerSheet;
            Frames = new List<string>();
            Frames.AddRange(SpriteSheet.Index.Keys);
        }

        public override void Update(GameTime gameTime)
        {
            if(Health <= 0)
            {
                Destroy();
            }
        }
    }
}
