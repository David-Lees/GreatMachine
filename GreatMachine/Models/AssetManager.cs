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
        public SpriteSheet WallSheet { get; set; }
        public SpriteSheet CursorSheet { get; set; }
        public SpriteSheet SpawnerSheet { get; set; }

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


        public AssetManager(ContentManager content)
        {
            DefaultFont = content.Load<SpriteFont>("DefaultFont");

            var BugsTexture = content.Load<Texture2D>("Bugs/Bugs");
            var PlayerTexture = content.Load<Texture2D>("Player/Player");
            var WallTexture = content.Load<Texture2D>("Walls/wall1");
            var BulletTexture = content.Load<Texture2D>("Bullets/bullet");
            var CursorTexture = content.Load<Texture2D>("Crosshairs");
            var SpawnerTexture = content.Load<Texture2D>("Bugs/Spawner");
                        
            BackgroundTexture = content.Load<Texture2D>("background1");
            CogsTopRight = content.Load<Texture2D>("Overlays/cogs-top-right");
            CogsTopLeft = content.Load<Texture2D>("Overlays/cogs-top-left");
            CogsBottomRight = content.Load<Texture2D>("Overlays/cogs-bottom-right");
            CogsBottomLeft = content.Load<Texture2D>("Overlays/cogs-bottom-left");
            CogsLeft = content.Load<Texture2D>("Overlays/cogs-left");
            CogsRight = content.Load<Texture2D>("Overlays/cogs-right");
            CogsTop = content.Load<Texture2D>("Overlays/cogs-top");
            CogsBottom = content.Load<Texture2D>("Overlays/cogs-bottom");

            Floor = content.Load<Texture2D>("Floor/oldmetal13a");

            BugsSheet = new SpriteSheet(BugsTexture).GenerateIndexes(64, 64, 13, 1);
            PlayerSheet = new SpriteSheet(PlayerTexture).GenerateIndexes(64, 64, 1, 1);
            WallSheet = new SpriteSheet(WallTexture).GenerateIndexes(64, 64, 1, 1);
            BulletSheet = new SpriteSheet(BulletTexture).GenerateIndexes(8, 8, 1, 1);
            CursorSheet = new SpriteSheet(CursorTexture).GenerateIndexes(32, 32, 1, 1);
            SpawnerSheet = new SpriteSheet(SpawnerTexture).GenerateIndexes(128, 128, 1, 1);
            

            SoundEffects = new Dictionary<string, SoundEffect>();
            try
            {
                SoundEffects.Add("Ricochet1", content.Load<SoundEffect>("Bullets/ric1"));
                SoundEffects.Add("Ricochet2", content.Load<SoundEffect>("Bullets/ric6"));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
