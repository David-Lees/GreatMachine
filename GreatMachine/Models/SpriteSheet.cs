using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GreatMachine.Models
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public Dictionary<string, Rectangle> Index { get; set; } = new Dictionary<string, Rectangle>();

        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }

        public float Radius { get; set; }

        /// <summary>
        /// Used for unit testing
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public SpriteSheet(int w, int h)
        {
            SpriteWidth = w;
            SpriteHeight = h;
            Radius = w / 2.0f;
        }

        public SpriteSheet(Texture2D texture)
        {
            Texture = texture;
        }

        public Rectangle? SourceRectangle(string spriteName)
        {
            bool hasValue = Index.TryGetValue(spriteName, out Rectangle r);
            return hasValue ? r : null as Rectangle?;
        }

        public SpriteSheet GenerateIndexes(int width, int height, int xCount, int yCount)
        {
            SpriteWidth = width;
            SpriteHeight = height;
            Radius = width / 2.0f;
            int i = 0;

            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    Index.Add($"Sprite{i:000}", new Rectangle(x * width, y * height, width, height));
                    i++;
                }
            }
            return this;
        }
    }
}
