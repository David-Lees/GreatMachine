using Microsoft.Xna.Framework;

namespace GreatMachine.Models
{
    public class LineSegment
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Radius { get; set; } = 0;

        public int Sector { get; set; }

        public LineSegment(float sx, float sy, float ex, float ey, int sector)
        {
            Sector = sector;
            Start = new Vector2(sx, sy);
            End = new Vector2(ex, ey);
        }
    }
}
