using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GreatMachine.Models
{
    public class AssetManager
    {
        public SpriteSheet BulletSheet { get; set; }
        public SpriteFont DefaultFont { get; set; }
        public SpriteSheet BugsSheet { get; set; }
        public SpriteSheet PlayerSheet { get; set; }
        public List<SpriteSheet> HorizontalWallSheets { get; set; }
        public List<SpriteSheet> VerticalWallSheets { get; set; }
        public List<SpriteSheet> CornerWallSheets { get; set; }
        public SpriteSheet CursorSheet { get; set; }
        public SpriteSheet SpawnerSheet { get; set; }
        public SpriteSheet HealthSheet { get; set; }

        public Dictionary<string, SoundEffect> SoundEffects { get; set; }

        public Texture2D MiniMap { get; set; }

        public float MaxSoundDistance { get; private set; } = 600 * 600;

        public Texture2D BackgroundTexture { get; private set; }
        public Texture2D CogsTopRight { get; private set; }
        public Texture2D CogsTopLeft { get; private set; }
        public Texture2D CogsBottomRight { get; private set; }
        public Texture2D CogsBottomLeft { get; private set; }

        public Texture2D CogsLeft { get; private set; }
        public Texture2D CogsRight { get; private set; }
        public Texture2D CogsTop { get; private set; }
        public Texture2D CogsBottom { get; private set; }
        public Texture2D Floor { get; private set; }

        public Texture2D HealthBar { get; private set; }

        public AssetManager(ContentManager content)
        {
            DefaultFont = content.Load<SpriteFont>("DefaultFont");

            var BugsTexture = content.Load<Texture2D>("Bugs/Bugs");
            var PlayerTexture = content.Load<Texture2D>("Player/Player");
            var BulletTexture = content.Load<Texture2D>("Bullets/bullet");
            var CursorTexture = content.Load<Texture2D>("Crosshairs");
            var SpawnerTexture = content.Load<Texture2D>("Bugs/Spawner");
            var HealthTexture = content.Load<Texture2D>("Player/health");

            BackgroundTexture = content.Load<Texture2D>("background1");
            CogsTopRight = content.Load<Texture2D>("Overlays/cogs-top-right");
            CogsTopLeft = content.Load<Texture2D>("Overlays/cogs-top-left");
            CogsBottomRight = content.Load<Texture2D>("Overlays/cogs-bottom-right");
            CogsBottomLeft = content.Load<Texture2D>("Overlays/cogs-bottom-left");
            CogsLeft = content.Load<Texture2D>("Overlays/cogs-left");
            CogsRight = content.Load<Texture2D>("Overlays/cogs-right");
            CogsTop = content.Load<Texture2D>("Overlays/cogs-top");
            CogsBottom = content.Load<Texture2D>("Overlays/cogs-bottom");
            HealthBar = content.Load<Texture2D>("Overlays/HealthBar");
            Floor = content.Load<Texture2D>("Floor/oldmetal13a");

            BugsSheet = new SpriteSheet(BugsTexture).GenerateIndexes(64, 64, 13, 1);
            PlayerSheet = new SpriteSheet(PlayerTexture).GenerateIndexes(128, 128, 1, 1);
            BulletSheet = new SpriteSheet(BulletTexture).GenerateIndexes(8, 8, 2, 1);
            CursorSheet = new SpriteSheet(CursorTexture).GenerateIndexes(32, 32, 1, 1);
            SpawnerSheet = new SpriteSheet(SpawnerTexture).GenerateIndexes(128, 128, 1, 1);
            HealthSheet = new SpriteSheet(HealthTexture).GenerateIndexes(32, 32, 1, 1);

            HorizontalWallSheets = new List<SpriteSheet>
            {
                new SpriteSheet(content.Load<Texture2D>("Walls/combined-cogs1")).GenerateIndexes(320, 64, 1, 21),
                new SpriteSheet(content.Load<Texture2D>("Walls/Combined-Cogs2")).GenerateIndexes(320, 64, 1, 21)
            };

            VerticalWallSheets = new List<SpriteSheet>
            {
                new SpriteSheet(content.Load<Texture2D>("Walls/combined-cogs1r")).GenerateIndexes(64, 320, 21, 1),
                new SpriteSheet(content.Load<Texture2D>("Walls/Combined-Cogs2r")).GenerateIndexes(64, 320, 21, 1)
            };

            CornerWallSheets = new List<SpriteSheet>
            {
                new SpriteSheet(content.Load<Texture2D>("Walls/wall2")).GenerateIndexes(64, 64, 1, 1)
            };

            SoundEffects = new Dictionary<string, SoundEffect>
            {
                { "Bag", content.Load<SoundEffect>("Bullets/bag") },
                { "Bounce", content.Load<SoundEffect>("Bullets/bounce") },
                { "Die1", content.Load<SoundEffect>("Bugs/die1") },
                { "Die2", content.Load<SoundEffect>("Bugs/die2") },
                { "Wilhelm", content.Load<SoundEffect>("Player/Wilhelm_Scream") },
                { "Eek", content.Load<SoundEffect>("Player/eek") },
                { "Oh", content.Load<SoundEffect>("Player/oh") },
                { "Music", content.Load<SoundEffect>("Shadowlands3Machine") }
            };

        }
    }
}
