﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models
{
    public class TextEntity: BaseEntity
    {
        public Vector2 Position { get; set; }

        public string Text { get; set; }

        public TextEntity(int x, int y, string text)
        {
            Position = new Vector2(x, y);
            Invulnerable = true;
            Text = text;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pos = Position * Main.Instance.SectorSize;            
            spriteBatch.DrawString(Main.Instance.Assets.DefaultFont, Text, pos, Color.Black);            
        }
    }    
}
