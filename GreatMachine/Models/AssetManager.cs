using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        public AssetManager(ContentManager content)
        {
            DefaultFont = content.Load<SpriteFont>("DefaultFont");

            var BugsTexture = content.Load<Texture2D>("Bugs/Bugs");
            var PlayerTexture = content.Load<Texture2D>("Player/Player");
            var WallTexture = content.Load<Texture2D>("Walls/wall1");
            var BulletTexture = content.Load<Texture2D>("Bullets/bullet");
            var CursorTexture = content.Load<Texture2D>("Crosshairs");
            var SpawnerTexture = content.Load<Texture2D>("Bugs/Spawner");

            BugsSheet = new SpriteSheet(BugsTexture).GenerateIndexes(64, 64, 13, 1);
            PlayerSheet = new SpriteSheet(PlayerTexture).GenerateIndexes(64, 64, 1, 1);
            WallSheet = new SpriteSheet(WallTexture).GenerateIndexes(64, 64, 1, 1);
            BulletSheet = new SpriteSheet(BulletTexture).GenerateIndexes(8, 8, 1, 1);
            CursorSheet = new SpriteSheet(CursorTexture).GenerateIndexes(32, 32, 1, 1);
            SpawnerSheet = new SpriteSheet(SpawnerTexture).GenerateIndexes(64, 64, 1, 1);

            SoundEffects = new Dictionary<string, SoundEffect>
            {
                { "Ricochet1", content.Load<SoundEffect>("Bullets/ric1") },
                { "Ricochet2", content.Load<SoundEffect>("Bullets/ric6") }
            };
        }
    }
}
