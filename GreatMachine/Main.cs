using GreatMachine.Helpers;
using GreatMachine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Controllers;
using tainicom.Aether.Physics2D.Dynamics;

namespace GreatMachine
{
    public class Main : Game
    {
        public static Main Instance { get; private set; }

        public Random Random { get; set; } = new Random();

        public Pathfinder Pathfinder { get; set; }

        private int[] sectors;
        public int[] Sectors() => sectors;
        public int SectorSize { get; private set; } = 64; // in pixels
        public int SectorCountX { get; set; }
        public int SectorCountY { get; set; }

        public Queue<Enemy> EnemiesRequiringPath { get; set; } = new Queue<Enemy>();

        public Vector2 ViewPortOrigin { get; set; } = new Vector2(0, 0);
        public float Scale { get; set; } = 1.0f;
        public ZoomLevel Zoom { get; set; } = ZoomLevel.Normal;

        public double SpawnerCooldown { get; set; }

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public readonly List<BaseEntity> Entities = new List<BaseEntity>();
        private readonly List<MoveableEntity> Movables = new List<MoveableEntity>();
        public Player Player { get; set; }

        public GamePadState GamePadInput { get; set; }
        public KeyboardState KeyboardInput { get; set; }
        public TouchCollection TouchInput { get; set; }
        public MouseState MouseInput { get; set; }

        private GamePadState previousGamePadInput { get; set; }
        private KeyboardState PreviousKeyboardInput { get; set; }
        private TouchCollection previousTouchInput { get; set; }
        private MouseState previousMouseInput { get; set; }

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        private readonly FrameCounter _frameCounter = new FrameCounter();
        private readonly FrameCounter _updateCounter = new FrameCounter();

        private Cursor Cursor { get; set; }

        public Camera Camera { get; set; }


        public World World { get; set; }

        public AssetManager Assets { get; set; }

        public Stack<BaseEntity> EntitiesToRemove { get; private set; } = new Stack<BaseEntity>();

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.Reach,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Main.Instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1680;
            _graphics.PreferredBackBufferHeight = 1050;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

        }

        protected override void LoadContent()
        {
            // Load Game Assets
            Assets = new AssetManager(Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create World
            World = new World()
            {
                Gravity = Vector2.Zero
            };

            var velocityController = new VelocityLimitController(1, 2);
            World.ControllerList.Add(velocityController);

            // Create Player
            Player = new Player(Assets.PlayerSheet.Radius)
            {
                SpriteSheet = Assets.PlayerSheet,
                Health = 100
            };
            Camera = new Camera();
            Camera.Follow(Player);

            // Create Cursor
            Cursor = new Cursor(Assets.CursorSheet);

            CreateLevel();
        }


        private void CreateLevel()
        {
            foreach (var e in Entities.ToList())
            {
                e.Destroy();
            }

            var width = 45;
            var height = width * 9 / 16;
            SectorCountX = width * 6;
            SectorCountY = height * 6;
            sectors = new int[SectorCountX * SectorCountY];

            Pathfinder = new Pathfinder(SectorCountX, SectorCountY);

            Player.Body.Position = new Vector2(
                SectorCountX * SectorSize / 2 + (SectorSize / 2),
                SectorCountY * SectorSize / 2 + (SectorSize / 2));

            var maze = new MazeGenerator(width, height);
            var walls = maze.GetWalls(5);
            foreach (var wall in walls.OfType<Wall>().ToList())
            {
                wall.SpriteSheet = Assets.WallSheet;
                var sector = PositionHelper.GetSectorAsVector(wall.BoundingBox.Center.ToVector2());
                Pathfinder.SetObstacle((int)Math.Floor(sector.X) - 1, (int)Math.Floor(sector.Y) - 1, true);
            }
            Entities.AddRange(walls);
            Entities.Add(Player);

            for (int i = 0; i < 50; i++)
            {
                var spawner = new Spawner(new Vector2(
                    Random.Next(width) * SectorSize * 6 + (SectorSize / 2) + (SectorSize * 2),
                    Random.Next(height) * SectorSize * 6 + (SectorSize / 2) + (SectorSize * 2)
                    ));
                Entities.Add(spawner);

                for (int j = 0; j < 10; j++)
                {
                    SpawnCrawlerAt(spawner);
                }
            }

            Assets.MiniMap = maze.CreateMazeTexture();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _updateCounter.Update(deltaTime);

            SpawnerCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
            if (SpawnerCooldown < 0)
            {
                var spawners = Entities.OfType<Spawner>().ToList();

                if (spawners.Count > 0)
                {
                    SpawnerCooldown = spawners.Count / 10.0f + 0.3f;
                    SpawnCrawler(spawners);
                }
                else
                {
                    // TODO: Game over?
                }
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleInput(gameTime);

            if (Pathfinder != null && Player != null && Player.PreviousSector != Player.Sector)
            {
                Player.PreviousSector = Player.Sector;
                var pos = PositionHelper.GetSectorAsVector(Player.Body.Position);
                Pathfinder.CalculateWavePropogation((int)pos.X, (int)pos.Y);
            }

            // becuase it doesn't run fast enough if called only once.
            for (int i = 1; i < 10; i++)
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

            Cursor.Update(gameTime);

            base.Update(gameTime);
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

        public void HandleInput(GameTime gameTime)
        {
            KeyboardInput = Keyboard.GetState();
            MouseInput = Mouse.GetState();
            GamePadInput = GamePad.GetState(PlayerIndex.One);
            TouchInput = TouchPanel.GetState();

            if (GamePadInput.Buttons.Back == ButtonState.Pressed || KeyboardInput.IsKeyDown(Keys.Escape))
                Exit();

            if (KeyboardInput.IsKeyDown(Keys.F3) && PreviousKeyboardInput.IsKeyUp(Keys.F3)) isDebug = !isDebug;

            var origin = ViewPortOrigin;

            if (isDebug)
            {
                if (KeyboardInput.IsKeyDown(Keys.NumPad4) && PreviousKeyboardInput.IsKeyUp(Keys.NumPad4))
                    Player.Body.Position = new Vector2(Player.Body.Position.X - SectorSize * 6, Player.Body.Position.Y);
                if (KeyboardInput.IsKeyDown(Keys.NumPad6) && PreviousKeyboardInput.IsKeyUp(Keys.NumPad6))
                    Player.Body.Position = new Vector2(Player.Body.Position.X + SectorSize * 6, Player.Body.Position.Y);
                if (KeyboardInput.IsKeyDown(Keys.NumPad8) && PreviousKeyboardInput.IsKeyUp(Keys.NumPad8))
                    Player.Body.Position = new Vector2(Player.Body.Position.X, Player.Body.Position.Y - SectorSize * 6);
                if (KeyboardInput.IsKeyDown(Keys.NumPad2) && PreviousKeyboardInput.IsKeyUp(Keys.NumPad2))
                    Player.Body.Position = new Vector2(Player.Body.Position.X, Player.Body.Position.Y + SectorSize * 6);

                var target = Vector2.Transform(
                    Main.Instance.MouseInput.Position.ToVector2(),
                    Matrix.Invert(Main.Instance.Camera.Transform));

                if (KeyboardInput.IsKeyDown(Keys.F4) && PreviousKeyboardInput.IsKeyUp(Keys.F4))
                {
                    Entities.Add(new Crawler(target));
                }
            }

            if (KeyboardInput.IsKeyDown(Keys.R) && PreviousKeyboardInput.IsKeyUp(Keys.R)) CreateLevel();

            if (KeyboardInput.IsKeyDown(Keys.Z) && PreviousKeyboardInput.IsKeyUp(Keys.Z))
            {
                Zoom = Zoom switch
                {
                    ZoomLevel.Close => ZoomLevel.Far,
                    ZoomLevel.Normal => ZoomLevel.Close,
                    ZoomLevel.Far => ZoomLevel.Normal,
                    _ => Zoom
                };
            }
            Scale = Zoom == ZoomLevel.Far ? (float)_graphics.PreferredBackBufferWidth / (SectorCountX * SectorSize) : 1.05f;
            Camera.Zoom = Zoom;

            ViewPortOrigin = origin;

            Player.Update(gameTime);

            previousGamePadInput = GamePadInput;
            PreviousKeyboardInput = KeyboardInput;
            previousMouseInput = MouseInput;
            previousTouchInput = TouchInput;
        }

        private bool isDebug = false;
        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = $"FPS: {_frameCounter.AverageFramesPerSecond:.00}; " +
                $"TPS: {_updateCounter.AverageFramesPerSecond:.00}; " +
                $"Scale: {Scale:.00}";

            GraphicsDevice.Clear(Color.SaddleBrown);

            var playerPos = PositionHelper.GetSectorAsVector(Player.Body.Position);

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


            Cursor.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // overlays
            _spriteBatch.Begin();

            if (Assets.MiniMap != null)
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
                    data[PositionHelper.Convert2Dto1D((int)playerPos.X, (int)playerPos.Y, Assets.MiniMap.Width)] = Color.White;
                    data[PositionHelper.Convert2Dto1D((int)playerPos.X - 1, (int)playerPos.Y, Assets.MiniMap.Width)] = Color.White;
                    data[PositionHelper.Convert2Dto1D((int)playerPos.X, (int)playerPos.Y - 1, Assets.MiniMap.Width)] = Color.White;
                    data[PositionHelper.Convert2Dto1D((int)playerPos.X + 1, (int)playerPos.Y, Assets.MiniMap.Width)] = Color.White;
                    data[PositionHelper.Convert2Dto1D((int)playerPos.X, (int)playerPos.Y + 1, Assets.MiniMap.Width)] = Color.White;
                }
                minimap.SetData(data);

                _spriteBatch.Draw(
                    minimap,
                    new Rectangle(ScreenWidth - Assets.MiniMap.Width, 0, Assets.MiniMap.Width, Assets.MiniMap.Height),
                    Color.White);

            }
            _spriteBatch.DrawString(Assets.DefaultFont, fps, new Vector2(1, 1), Color.Red);
            _spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
