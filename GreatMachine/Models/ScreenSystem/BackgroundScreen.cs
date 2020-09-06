/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace GreatMachine.Models.ScreenSystem
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    public class BackgroundScreen : GameScreen
    {
        private const float LogoScreenHeightRatio = 0.25f;
        private const float LogoScreenBorderRatio = 0.0375f;
        private const float LogoWidthHeightRatio = 1.4f;

        private Texture2D _backgroundTexture;
        private Texture2D _logoTexture;
        private Texture2D _titleTexture;
        private Texture2D _girl;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
        }

        public override void LoadContent()
        {
            _logoTexture = ScreenManager.Content.Load<Texture2D>("Common/logo");
            _backgroundTexture = ScreenManager.Content.Load<Texture2D>("background2");
            _titleTexture = ScreenManager.Content.Load<Texture2D>("Overlays/logo");
            _girl = ScreenManager.Content.Load<Texture2D>("Overlays/girl");                            
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 logoSize = new Vector2
            {
                Y = viewport.Height * LogoScreenHeightRatio
            };
            logoSize.X = logoSize.Y * LogoWidthHeightRatio;

            var ratio = (viewport.Width / 2.0f) / _titleTexture.Width;
            var _titleDestination = new Rectangle(
                (int)((viewport.Width - (_titleTexture.Width * ratio)) / 2.0f),
                (int)(viewport.Height / 10.0f),
                (int)(_titleTexture.Width * ratio), (int)(_titleTexture.Height * ratio));


            float border = viewport.Height * LogoScreenBorderRatio;
            Vector2 logoPosition = new Vector2(border, viewport.Height - border - logoSize.Y);
            var _logoDestination = new Rectangle((int)logoPosition.X, (int)logoPosition.Y, (int)logoSize.X, (int)logoSize.Y);
            var _viewport = viewport.Bounds;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(_backgroundTexture, _viewport, Color.White);
            ScreenManager.SpriteBatch.Draw(_logoTexture, _logoDestination, Color.White * 0.25f);
            ScreenManager.SpriteBatch.Draw(_titleTexture, _titleDestination, Color.White);
            ScreenManager.SpriteBatch.Draw(_girl, new Vector2(_viewport.Width - _girl.Width, _viewport.Height - _girl.Height), Color.White);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Shadowlands 3 - Machine Kevin MacLeod (incompetech.com)");
            sb.AppendLine("Licensed under Creative Commons: By Attribution 3.0 License http://creativecommons.org/licenses/by/3.0/");

            ScreenManager.SpriteBatch.DrawString(
                Main.Instance.Assets.DefaultFont, sb, 
                new Vector2(3, _viewport.Height - Main.Instance.Assets.DefaultFont.MeasureString("Mp").Y),
                Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

            ScreenManager.SpriteBatch.End();
        }
    }
}
