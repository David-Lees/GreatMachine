using GreatMachine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreatMachine
{
    public class Main : Game
    {
        public static Main Instance { get; private set; }

        public Random Random { get; set; } = new Random();

        private int[] sectors;
        public int[] Sectors() => sectors;
        public int SectorSize { get; private set; } = 64; // in pixels
        public int SectorCountX { get; set; }
        public int SectorCountY { get; set; }

        public Vector2 ViewPortOrigin { get; set; } = new Vector2(0, 0);
        public float Scale { get; set; } = 1.0f;
        public bool Zoomed { get; set; }


        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public readonly List<BaseEntity> Entities = new List<BaseEntity>();
        private readonly List<MoveableEntity> Movables = new List<MoveableEntity>();
        private readonly Player player = new Player();

        private Texture2D BugsTexture;
        private Texture2D PlayerTexture;
        private Texture2D WallTexture;
        private Texture2D BulletTexture;
        private Texture2D CursorTexture;

        private SpriteSheet BugsSheet;
        private SpriteSheet PlayerSheet;
        private SpriteSheet WallSheet;
        private SpriteSheet CursorSheet;

        public SpriteSheet BulletSheet { get; set; }

        public SpriteFont DefaultFont { get; set; }

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

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Main.Instance = this;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultFont = Content.Load<SpriteFont>("DefaultFont");

            BugsTexture = Content.Load<Texture2D>("Bugs/Bugs");
            PlayerTexture = Content.Load<Texture2D>("Player/Player");
            WallTexture = Content.Load<Texture2D>("Walls/wall1");
            BulletTexture = Content.Load<Texture2D>("Bullets/bullet");
            CursorTexture = Content.Load<Texture2D>("Crosshairs");

            BugsSheet = new SpriteSheet(BugsTexture).GenerateIndexes(32, 32, 9, 4);
            PlayerSheet = new SpriteSheet(PlayerTexture).GenerateIndexes(64, 64, 1, 1);
            WallSheet = new SpriteSheet(WallTexture).GenerateIndexes(64, 64, 1, 1);
            BulletSheet = new SpriteSheet(BulletTexture).GenerateIndexes(8, 8, 1, 1);
            CursorSheet = new SpriteSheet(CursorTexture).GenerateIndexes(32, 32, 1, 1);

            player.SpriteSheet = PlayerSheet;
            player.Health = 100;
            Camera = new Camera();
            Camera.Follow(player);

            Cursor = new Cursor(CursorSheet);

            CreateLevel();
        }

        private void CreateLevel()
        {
            var width = 45;
            var height = width * 9  / 16;
            SectorCountX = width * 6 + 1;
            SectorCountY = height * 6 + 1;            
            sectors = new int[SectorCountX * SectorCountY];

            player.Position = new Vector2(
                SectorCountX * SectorSize / 2 + (SectorSize / 2), 
                SectorCountY * SectorSize / 2 + (SectorSize / 2));

            Entities.Clear();
            var maze = new MazeGenerator(width, height);
            var walls = maze.GetWalls(5);
            foreach (var wall in walls)
            {
                wall.SpriteSheet = WallSheet;
            }
            Entities.AddRange(walls);
            Entities.Add(player);
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _updateCounter.Update(deltaTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();            

            HandleInput(gameTime);

            Camera.Follow(player);

            foreach(var e in Entities.Where(e => e != player).ToList())
            {
                e.Update(gameTime);
            }

            Cursor.Update(gameTime);

            base.Update(gameTime);
        }


        public void HandleInput(GameTime gameTime)
        {
            KeyboardInput = Keyboard.GetState();
            MouseInput = Mouse.GetState();
            GamePadInput = GamePad.GetState(PlayerIndex.One);
            TouchInput = TouchPanel.GetState();

            if (GamePadInput.Buttons.Back == ButtonState.Pressed || KeyboardInput.IsKeyDown(Keys.Escape))
                Exit();

            var origin = ViewPortOrigin;
            var speed = 100;
            if (KeyboardInput.IsKeyDown(Keys.NumPad4)) origin.X -= (float)(gameTime.ElapsedGameTime.TotalSeconds * speed);
            if (KeyboardInput.IsKeyDown(Keys.NumPad6)) origin.X += (float)(gameTime.ElapsedGameTime.TotalSeconds * speed);
            if (KeyboardInput.IsKeyDown(Keys.NumPad8)) origin.Y -= (float)(gameTime.ElapsedGameTime.TotalSeconds * speed);
            if (KeyboardInput.IsKeyDown(Keys.NumPad2)) origin.Y += (float)(gameTime.ElapsedGameTime.TotalSeconds * speed);
            if (KeyboardInput.IsKeyDown(Keys.OemPlus)) Scale *= 1.1f;
            if (KeyboardInput.IsKeyDown(Keys.OemMinus)) Scale *= 0.9f;

            if (KeyboardInput.IsKeyDown(Keys.R) && PreviousKeyboardInput.IsKeyUp(Keys.R)) CreateLevel();

            if (KeyboardInput.IsKeyDown(Keys.Z) && PreviousKeyboardInput.IsKeyUp(Keys.Z)) Zoomed = !Zoomed;
            Scale = Zoomed ? (float)_graphics.PreferredBackBufferWidth / (SectorCountX * SectorSize) : 1.0f;
            Camera.Zoom = Zoomed ? 0.5f : 0;

            ViewPortOrigin = origin;

            player.Update(gameTime);

            previousGamePadInput = GamePadInput;
            PreviousKeyboardInput = KeyboardInput;
            previousMouseInput = MouseInput;
            previousTouchInput = TouchInput;
        }


        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = $"FPS: {_frameCounter.AverageFramesPerSecond:.00}; " +
                $"TPS: {_updateCounter.AverageFramesPerSecond:.00}; " +
                $"Scale: {Scale:.00}";

            GraphicsDevice.Clear(Color.CornflowerBlue);            

            // World objects
            _spriteBatch.Begin(transformMatrix: Camera.Transform);

            foreach (var e in Entities)
            {
                e.Draw(gameTime, _spriteBatch);
            }

            Cursor.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // overlays
            _spriteBatch.Begin();
            _spriteBatch.DrawString(DefaultFont, fps, new Vector2(1, 1), Color.Red);            
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
