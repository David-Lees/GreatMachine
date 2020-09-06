using GreatMachine.Models.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GreatMachine.Models
{
    public class GameStatusScreen : GameScreen
    {
        private Texture2D Texture;
        private readonly string TextureName;

        public GameStatusScreen(string tex)
        {
            ScreenState = ScreenState.Hidden;
            TextureName = tex;
            IsPopup = true;
        }

        public override void LoadContent()
        {
            Texture = ScreenManager.Content.Load<Texture2D>(TextureName);
        }

        public override void Draw(GameTime gameTime)
        {
            if (ScreenState == ScreenState.Active)
            {
                var spriteBatch = ScreenManager.SpriteBatch;
                var viewport = ScreenManager.GraphicsDevice.Viewport;

                spriteBatch.Begin();
                spriteBatch.Draw(Texture, new Vector2((viewport.Width - Texture.Width) / 2, (viewport.Height - Texture.Height) / 2), Color.White);
                spriteBatch.End();
            }
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            if (input.IsNewButtonPress(Buttons.Back) || input.IsNewKeyPress(Keys.Escape))
            {
                ScreenState = ScreenState.Hidden;
                Main.Instance.ScreenState = ScreenState.Hidden;
                Main.Instance.Menu.ScreenState = ScreenState.Active;
            }
        }
    }
}
