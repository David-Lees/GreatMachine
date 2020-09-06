using GreatMachine.Models.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine
{
    public class GreatMachine : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        public ScreenManager ScreenManager { get; set; }

        public Main Main { get; set; }        

        public GreatMachine()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.Reach,               
                PreferMultiSampling = true,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            _graphics.PreparingDeviceSettings += GraphicsPreparingDeviceSettings;

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        void GraphicsPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // unlock the 30 fps limit. 60fps (if possible)
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;

            // set HiDef Profile if supported
            if (e.GraphicsDeviceInformation.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Main = new Main(_graphics, GraphicsDevice, Content);

            MenuScreen menuScreen = new MenuScreen();

            menuScreen.AddMenuItem("Resume", EntryType.Action, () => Main.State == GameState.Running, () => { 
                Main.ScreenState = ScreenState.Active;
                menuScreen.ScreenState = ScreenState.Hidden;
            });
            menuScreen.AddMenuItem("New Game", EntryType.Action, () => true, () => {
                Main.CreateLevel();
                Main.ScreenState = ScreenState.Active;
                menuScreen.ScreenState = ScreenState.Hidden;
                Main.GameOver.ScreenState = ScreenState.Hidden;
                Main.GameWon.ScreenState = ScreenState.Hidden;
            });            
            menuScreen.AddMenuItem("Exit", EntryType.Action, () => true, () => Exit());
            menuScreen.AddMenuItem("", EntryType.Separator, () => true, () => { });

            ScreenManager.AddScreen(new BackgroundScreen() { ScreenState = ScreenState.Active });
            ScreenManager.AddScreen(Main);
            ScreenManager.AddScreen(menuScreen);
            ScreenManager.AddScreen(Main.GameOver);
            ScreenManager.AddScreen(Main.GameWon);

            Main.Menu = menuScreen;

            menuScreen.ScreenState = ScreenState.Active;
        }
    }
}
