/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models.ScreenSystem
{
    public enum EntryType
    {
        Action,
        Separator,        
    }

    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public sealed class MenuEntry
    {
        private Vector2 _baseOrigin;

        private float _height;
        private readonly MenuScreen _menu;

        private float _scale;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        private readonly EntryType _type;
        private float _width;
        private readonly Action _action;
        private readonly Func<bool> _IsVisible;

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(MenuScreen menu, string text, EntryType type, Func<bool> isVisible, Action action)
        {
            Text = text;
            _type = type;
            _menu = menu;
            _scale = 0.9f;
            Alpha = 1.0f;
            _action = action;
            _IsVisible = isVisible;
        }

        public void Act() => _action();

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position { get; set; }

        public float Alpha { get; set; }

        public GameScreen Screen { get; private set; }

        public void Initialize()
        {
            SpriteFont font = _menu.ScreenManager.Fonts.MenuSpriteFont;
            _width = font.MeasureString(Text).X;
            _height = font.MeasureString("M").Y;
            _baseOrigin = new Vector2(_width, _height) * 0.5f;
        }

        public bool IsSelectable() => _type != EntryType.Separator;      

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update(bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            if (_type != EntryType.Separator)
            {
                float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                if (isSelected)
                    _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1f);
                else
                    _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0f);

                _scale = 0.9f + 0.1f * _selectionFade;
            }
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public void Draw()
        {
            if (_IsVisible())
            {
                SpriteFont font = _menu.ScreenManager.Fonts.MenuSpriteFont;
                SpriteBatch batch = _menu.ScreenManager.SpriteBatch;

                // Draw the selected entry   
                var col = new Color(251, 223, 175, 255);
                var colSel = new Color(251, 223, 175, 128);


                Color color = _type == EntryType.Separator ? col : Color.Lerp(col, colSel, _selectionFade);
                color *= Alpha;

                // Draw text, centered on the middle of each line.                
                batch.DrawString(font, Text, Position - _baseOrigin * _scale + Vector2.One, Color.Black, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
                batch.DrawString(font, Text, Position - _baseOrigin * _scale, color, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            }
        }

        /// <summary>Queries how much space this menu entry requires.</summary>
        public int GetHeight() => (int)_height;

        /// <summary>Queries how wide the entry is, used for centering on the screen.</summary>
        public int GetWidth() => (int)_width;
        
    }
}
