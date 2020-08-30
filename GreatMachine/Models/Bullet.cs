using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using System.Linq;

namespace GreatMachine.Models
{
    public class Bullet : MoveableEntity
    {
        const int Speed = 8;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Lifespan -= gameTime.ElapsedGameTime.TotalSeconds;
            if (Lifespan < 0) Destroy();

            var pos = new Vector2(Position.X, Position.Y);

            pos += Velocity * Speed;
            Position = pos;
            
            var walls = Main.Instance.Entities
                .Where(e => e is Wall && SurroundingSectors.Contains(e.Sector))
                .ToList();

            var overlaps = walls.Where(x => CollisionHelper.Overlaps(this, x));

            // bounce
            foreach (var overlap in overlaps)
            {
                var edge = CollisionHelper.CollidingEdge(this, overlap);
                var offset = (SpriteSheet.SpriteWidth / 2) + 1;
                if (edge.HasFlag(CollisionEdge.Top))
                {
                    var ammount = pos.Y - overlap.BoundingBox.Y;
                    pos.Y = overlap.BoundingBox.Y - MathHelper.Max(ammount, offset);
                }
                if (edge.HasFlag(CollisionEdge.Bottom))
                {
                    var ammount = (overlap.BoundingBox.Y + overlap.BoundingBox.Height) - pos.Y;
                    pos.Y = (overlap.BoundingBox.Y + overlap.BoundingBox.Height) + MathHelper.Max(ammount, offset);
                }
                if (edge.HasFlag(CollisionEdge.Left))
                {
                    var ammount = pos.X - overlap.BoundingBox.X;
                    pos.X = overlap.BoundingBox.X - MathHelper.Max(ammount, offset);
                }
                if (edge.HasFlag(CollisionEdge.Right))
                {
                    var ammount = (overlap.BoundingBox.X + overlap.BoundingBox.Height) - pos.X;
                    pos.X = (overlap.BoundingBox.X + overlap.BoundingBox.Height) + MathHelper.Max(ammount, offset);
                }
            }
            Position = pos;

            var player = Main.Instance.Entities.SingleOrDefault(e => e is Player);
            if (player != null && CollisionHelper.Overlaps(this, player))
            {
                player.Health -= 10;
                Destroy();
            }

            
            var enemies = Main.Instance.Entities
                .Where(e => e is Enemy && SurroundingSectors.Contains(e.Sector));
            foreach (var enemy in enemies)
            {
                if (CollisionHelper.Overlaps(enemy, this))
                {
                    enemy.Health -= 10;
                    Destroy();
                    break;
                }
            }
        }

        public void Destroy()
        {
            Main.Instance.Entities.Remove(this);
        }
    }
}
