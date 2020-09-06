using GreatMachine.Helpers;
using GreatMachine.Models;
using GreatMachine.Models.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using tainicom.Aether.Physics2D.Controllers;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine
{
    public enum GameState
    {
        New,
        Running,
        Won,
        Lost,
    }

    public class Main : GameScreen
    {
        private bool isDebug = false;
        private readonly ContentManager Content;
        private readonly FrameCounter _frameCounter = new FrameCounter();
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int[] sectors;

        public GraphicsDevice GraphicsDevice { get; private set; }
        public static Main Instance { get; private set; }
        public Random Random { get; set; } = new Random();
        public Stack<BaseEntity> EntitiesToRemove { get; private set; } = new Stack<BaseEntity>();
        public AssetManager Assets { get; set; }
        public Camera Camera { get; set; }
        public World World { get; set; }
        public Player Player { get; set; }
        public readonly List<BaseEntity> Entities = new List<BaseEntity>();
        public double SpawnerCooldown { get; set; }
        public Vector2 ViewPortOrigin { get; set; } = new Vector2(0, 0);
        public float Scale { get; set; } = 1.0f;
        public ZoomLevel Zoom { get; set; } = ZoomLevel.Normal;
        public int[] Sectors() => sectors;
        public int SectorSize { get; private set; } = 64; // in pixels
        public int SectorCountX { get; set; }
        public int SectorCountY { get; set; }
        public Queue<Enemy> EnemiesRequiringPath { get; set; } = new Queue<Enemy>();
        public Pathfinder Pathfinder { get; set; }
        public GameState State { get; set; } = GameState.New;

        public GameStatusScreen GameOver { get; private set; } = new GameStatusScreen("Overlays/game-over");
        public GameStatusScreen GameWon { get; private set; } = new GameStatusScreen("Overlays/game-won");
        public MenuScreen Menu { get; set; }
        public Vector2 Crosshairs { get; set; }

        public SoundEffectInstance Music { get; set; }

        public Main(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, ContentManager content)
        {
            GraphicsDevice = graphicsDevice;
            Content = content;
            _graphics = graphics;
            Instance = this;

            ScreenState = ScreenState.Hidden;            

            // Create World
            World = new World()
            {
                Gravity = Vector2.Zero
            };

            var velocityController = new VelocityLimitController(1, 2);
            World.ControllerList.Add(velocityController);
        }

        public void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
            {
                _graphics.IsFullScreen = false;
                _graphics.PreferredBackBufferWidth = 1280;
                _graphics.PreferredBackBufferHeight = 720;
            }
            else
            {
                _graphics.IsFullScreen = true;
                _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            }
            _graphics.ApplyChanges();
        }

        public override void LoadContent()
        {
            // Load Game Assets
            Assets = new AssetManager(Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Remove any existing players
            foreach (var e in Entities.OfType<Player>().ToList())
            {
                Entities.Remove(e);
            }

            // Create Player
            Player = new Player()
            {
                SpriteSheet = Assets.PlayerSheet,
                Health = 100
            };
            Entities.Add(Player);

            Camera = new Camera();
            Camera.Follow(Player);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (Music != null && Music.State != SoundState.Playing)
            {
                Music.Play();
            }
            else if (Music == null && Assets.SoundEffects.ContainsKey("Music"))
            {
                Music = Assets.SoundEffects["Music"].CreateInstance();
            }

            if (IsActive && !otherScreenHasFocus && State == GameState.Running)
            {
                if (Player.Health < 0)
                {
                    State = GameState.Lost;
                    GameOver.ScreenState = ScreenState.Active;
                    Assets.SoundEffects["Wilhelm"].Play();
                }

                SpawnerCooldown -= SpawnerCooldown > 0 ? gameTime.ElapsedGameTime.TotalSeconds : 0;

                if (SpawnerCooldown <= 0)
                {
                    var spawners = Entities.OfType<Spawner>().ToList();

                    if (spawners.Count > 0)
                    {
                        SpawnerCooldown = spawners.Count / 10.0f + 0.3f;
                        SpawnCrawler(spawners);
                    }
                    else if (spawners.Count == 0 && !Entities.OfType<Crawler>().Any())
                    {
                        State = GameState.Won;
                        GameWon.ScreenState = ScreenState.Active;
                    }
                }

                if (Pathfinder != null && Player != null && Player.PreviousSector != Player.Sector)
                {
                    Player.PreviousSector = Player.Sector;
                    var pos = PositionHelper.GetSectorAsVector(Player.Body.Position);
                    Pathfinder.CalculateWavePropogation((int)pos.X, (int)pos.Y);
                }

                // becuase it doesn't run fast enough if called only once.
                for (int i = 0; i < 8; i++)
                    World.Step(gameTime.ElapsedGameTime);

                // Update AI
                int updatesLeft = 50;
                while (EnemiesRequiringPath.Count > 0 && updatesLeft > 0)
                {
                    updatesLeft--;
                    var enemy = EnemiesRequiringPath.Dequeue();
                    enemy.UpdateDirection();
                }

                while (EntitiesToRemove.Count > 0)
                {
                    var item = EntitiesToRemove.Pop();
                    if (World.BodyList.Contains(item.Body)) World.Remove(item.Body);
                    if (Entities.Contains(item)) Entities.Remove(item);
                }

                Camera.Follow(Player);

                foreach (var e in Entities.Where(e => e != Player).ToList())
                {
                    e.Update(gameTime);
                }

                Player.Update(gameTime);

                var origin = ViewPortOrigin;
                Scale = Zoom == ZoomLevel.Far ? (float)_graphics.PreferredBackBufferWidth / (SectorCountX * SectorSize) : 1.05f;
                Camera.Zoom = Zoom;
                ViewPortOrigin = origin;

                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            Crosshairs = input.MouseState.Position.ToVector2();            

            if (input.IsNewKeyPress(Keys.F11))
            {
                ToggleFullscreen();
            }

            if (input.IsNewButtonPress(Buttons.Back) || input.IsNewKeyPress(Keys.Escape))
            {
                ScreenState = ScreenState.Hidden;
                Menu.ScreenState = ScreenState.Active;
            }
            if (input.IsNewKeyPress(Keys.F3)) isDebug = !isDebug;

            Player.HandleInput(input, gameTime);

            if (isDebug)
            {
                if (input.IsNewKeyPress(Keys.NumPad4))
                    Player.Body.Position = new Vector2(Player.Body.Position.X - SectorSize * 6, Player.Body.Position.Y);
                if (input.IsNewKeyPress(Keys.NumPad6))
                    Player.Body.Position = new Vector2(Player.Body.Position.X + SectorSize * 6, Player.Body.Position.Y);
                if (input.IsNewKeyPress(Keys.NumPad8))
                    Player.Body.Position = new Vector2(Player.Body.Position.X, Player.Body.Position.Y - SectorSize * 6);
                if (input.IsNewKeyPress(Keys.NumPad2))
                    Player.Body.Position = new Vector2(Player.Body.Position.X, Player.Body.Position.Y + SectorSize * 6);

                var target = Vector2.Transform(
                    input.MouseState.Position.ToVector2(),
                    Matrix.Invert(Camera.Transform));

                if (input.IsNewKeyPress(Keys.F4))
                {
                    Entities.Add(new Crawler(target));
                }
            }

            if (input.IsNewKeyPress(Keys.Z))
            {
                Zoom = Zoom switch
                {
                    ZoomLevel.Close => ZoomLevel.Far,
                    ZoomLevel.Normal => ZoomLevel.Close,
                    ZoomLevel.Far => ZoomLevel.Normal,
                    _ => Zoom
                };
            }

        }

        public override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = $"FPS: {_frameCounter.AverageFramesPerSecond:.00}";
            var playerPos = PositionHelper.GetSectorAsVector(Player.Body.Position);

            DrawBackground();
            DrawWorldObjects(gameTime);
            DrawOverlays(fps, playerPos);

            base.Draw(gameTime);
        }

        private void DrawOverlays(string fps, Vector2 playerPos)
        {
            // overlays
            _spriteBatch.Begin();

            var w = 0;
            while (w < GraphicsDevice.Viewport.Width)
            {
                _spriteBatch.Draw(Assets.CogsTop, new Rectangle(w, 0, Assets.CogsTop.Width, Assets.CogsTop.Height), Color.White);
                _spriteBatch.Draw(Assets.CogsBottom, new Rectangle(w, GraphicsDevice.Viewport.Height - Assets.CogsBottom.Height, Assets.CogsBottom.Width, Assets.CogsBottom.Height), Color.White);
                w += Assets.CogsTop.Width;
            }
            var h = 0;
            while (h < GraphicsDevice.Viewport.Height)
            {
                _spriteBatch.Draw(Assets.CogsLeft, new Rectangle(0, h, Assets.CogsLeft.Width, Assets.CogsLeft.Height), Color.White);
                _spriteBatch.Draw(Assets.CogsRight, new Rectangle(GraphicsDevice.Viewport.Width - Assets.CogsRight.Width, h, Assets.CogsRight.Width, Assets.CogsRight.Height), Color.White);
                h += Assets.CogsTop.Width;
            }
            _spriteBatch.Draw(Assets.CogsTopLeft, new Rectangle(0, 0, Assets.CogsTopLeft.Width, Assets.CogsTopLeft.Height), Color.White);
            _spriteBatch.Draw(Assets.CogsTopRight, new Rectangle(GraphicsDevice.Viewport.Width - Assets.CogsTopRight.Width, 0, Assets.CogsTopRight.Width, Assets.CogsTopRight.Height), Color.White);
            _spriteBatch.Draw(Assets.CogsBottomLeft, new Rectangle(0, GraphicsDevice.Viewport.Height - Assets.CogsBottomLeft.Height, Assets.CogsBottomLeft.Width, Assets.CogsBottomLeft.Height), Color.White);
            _spriteBatch.Draw(Assets.CogsBottomRight, new Rectangle(GraphicsDevice.Viewport.Width - Assets.CogsBottomRight.Width, GraphicsDevice.Viewport.Height - Assets.CogsBottomRight.Height, Assets.CogsBottomRight.Width, Assets.CogsBottomRight.Height), Color.White);

            if (Assets.MiniMap != null)
            {
                Texture2D minimap = GenerateMiniMap(playerPos);
                _spriteBatch.Draw(
                    minimap,
                    new Rectangle(GraphicsDevice.Viewport.Width - (int)(Assets.MiniMap.Width * 1.5) - 16, 16, (int)(Assets.MiniMap.Width * 1.5), (int)(Assets.MiniMap.Height * 1.5)),
                    Color.White);
            }
            if (isDebug)
            {
                _spriteBatch.DrawString(Assets.DefaultFont, fps, new Vector2(1, 1), Color.Black);
            }

            _spriteBatch.Draw(Assets.CursorSheet.Texture, Crosshairs - (Vector2.One * 16), Color.White);

            _spriteBatch.Draw(
                Assets.HealthBar,
                new Rectangle(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 256, 256),
                null,
                Color.Lerp(Color.Red, Color.Green, Player.Health / 100f),
                MathHelper.Lerp(-MathHelper.PiOver2, 0, Player.Health / 100f),
                Vector2.One * 256,
                SpriteEffects.None,
                0);


            _spriteBatch.End();
        }

        private Texture2D GenerateMiniMap(Vector2 playerPos)
        {
            var minimap = new Texture2D(GraphicsDevice, Assets.MiniMap.Width, Assets.MiniMap.Height);
            var data = new Color[Assets.MiniMap.Width * Assets.MiniMap.Height];
            Assets.MiniMap.GetData(data);


            foreach (var e in Entities.OfType<Enemy>())
            {
                var epos = PositionHelper.GetSectorAsVector(e.Body.Position);
                if (epos.X > 0 && epos.Y > 0 && epos.X < Assets.MiniMap.Width - 2 && epos.Y < Assets.MiniMap.Height - 2)
                {
                    data[PositionHelper.Convert2Dto1D((int)epos.X, (int)epos.Y, Assets.MiniMap.Width)] = Color.IndianRed;
                }
            }

            if (playerPos.X > 0 && playerPos.Y > 0 && playerPos.X < Assets.MiniMap.Width - 2 && playerPos.Y < Assets.MiniMap.Height - 2)
            {
                data[P((int)playerPos.X, (int)playerPos.Y)] = Color.White;
                data[P((int)playerPos.X - 1, (int)playerPos.Y)] = Color.White;
                data[P((int)playerPos.X, (int)playerPos.Y - 1)] = Color.White;
                data[P((int)playerPos.X + 1, (int)playerPos.Y)] = Color.White;
                data[P((int)playerPos.X, (int)playerPos.Y + 1)] = Color.White;
            }
            minimap.SetData(data);
            return minimap;
        }

        private int P(int x, int y) => PositionHelper.Convert2Dto1D(MathHelper.Clamp(x, 0, Assets.MiniMap.Width), MathHelper.Clamp(y, 0, Assets.MiniMap.Height), Assets.MiniMap.Width);

        private void DrawWorldObjects(GameTime gameTime)
        {
            // World objects
            _spriteBatch.Begin(transformMatrix: Camera.Transform);

            foreach (var e in Entities)
            {
                e.Draw(gameTime, _spriteBatch);
            }

            if (isDebug)
            {
                for (int x = 0; x < SectorCountX; x++)
                {
                    for (int y = 0; y < SectorCountY; y++)
                    {
                        var s = Pathfinder.Stength(x, y);
                        _spriteBatch.DrawString(
                            Assets.DefaultFont,
                            s.ToString(),
                            new Vector2(x * SectorSize + 10, y * SectorSize + 10),
                            Color.Black);
                    }
                }
            }

            //Cursor.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }

        private void DrawBackground()
        {
            // Background
            _spriteBatch.Begin();

            float scaleHeight = GraphicsDevice.Viewport.Height / (float)Assets.BackgroundTexture.Height;
            float scaleWidth = GraphicsDevice.Viewport.Width / (float)Assets.BackgroundTexture.Width;
            float scale = MathF.Max(scaleWidth, scaleHeight);

            _spriteBatch.Draw(
                Assets.BackgroundTexture,
                new Rectangle(
                    0, 0,
                    (int)(Assets.BackgroundTexture.Width * scale), (int)(Assets.BackgroundTexture.Height * scale)),
                Color.White);

            _spriteBatch.End();
        }

        public void CreateLevel()
        {
            State = GameState.New;

            foreach (var e in Entities.ToList())
            {
                if (e.GetType() != typeof(Player)) e.Destroy();
            }

            var width = 31;
            var height = 21; //width * 9 / 16;
            SectorCountX = width * 6;
            SectorCountY = height * 6;
            sectors = new int[SectorCountX * SectorCountY];

            Pathfinder = new Pathfinder(SectorCountX, SectorCountY);

            Player.Body.Position = new Vector2(
                SectorCountX * SectorSize / 2 + (SectorSize / 2),
                SectorCountY * SectorSize / 2 + (SectorSize / 2));

            Player.Health = 100;

            var maze = new MazeGenerator(width, height);
            var walls = maze.GetWalls();
            foreach (var wall in walls.OfType<Wall>().ToList())
            {
                for (int i = 0; i < wall.BoundingBox.Width / 64; i++)
                {
                    for (int j = 0; j < wall.BoundingBox.Height / 64; j++)
                    {
                        var sector = PositionHelper.GetSectorAsVector(
                            new Vector2(wall.BoundingBox.X + i * 64 + 32, wall.BoundingBox.Y + j * 64 + 32));
                        Pathfinder.SetObstacle((int)Math.Floor(sector.X), (int)Math.Floor(sector.Y), true);
                    }
                }
            }
            Entities.AddRange(walls);

            var positions = new List<Vector2>();
            var spanwerCount = 0;
            while(spanwerCount < 30)            
            {
                var vector = new Vector2(
                    Random.Next(width) * SectorSize * 6 + (SectorSize / 2) + (SectorSize * 2) + 64,
                    Random.Next(height) * SectorSize * 6 + (SectorSize / 2) + (SectorSize * 2) + 64
                    );

                if (!positions.Contains(vector) && Vector2.Distance(vector, Player.Body.Position) > 640)
                {
                    spanwerCount++;
                    var spawner = new Spawner(vector);
                    Entities.Add(spawner);

                    for (int j = 0; j < 10; j++)
                    {
                        SpawnCrawlerAt(spawner);
                    }
                }
            }

            var healthCount = 0;
            while(healthCount < 30)
            {
                var x = Random.Next(0, SectorCountX);
                var y = Random.Next(0, SectorCountY);
                if (!Pathfinder.IsObstacle(x, y))
                {
                    var v = new Vector2(x * SectorSize - (SectorSize / 2), y * SectorSize - (SectorSize / 2));
                    if (!positions.Contains(v))
                    {
                        healthCount++;
                        var health = new Health(v);
                        Entities.Add(health);
                    }
                }
            }

            Assets.MiniMap = maze.CreateMazeTexture();

            State = GameState.Running;
        }

        private void SpawnCrawler(List<Spawner> spawners)
        {
            var spawner = spawners.OrderBy(x => Guid.NewGuid()).First();
            SpawnCrawlerAt(spawner);
        }

        private void SpawnCrawlerAt(Spawner spawner)
        {
            Entities.Add(new Crawler(new Vector2(
                spawner.Body.Position.X + Random.Next(256) - 128,
                spawner.Body.Position.Y + Random.Next(256) - 128)));
        }
    }
}
