/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models.ScreenSystem
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class MenuScreen : GameScreen
    {        
        private readonly List<MenuEntry> _menuEntries = new List<MenuEntry>();
        private int _selectedEntry;

        private float _menuTop;

        /// <summary>Constructor</summary>
        public MenuScreen()
        {            
            HasCursor = true;            
        }        

        public void AddMenuItem(string name, EntryType type, Func<bool> isVisible, Action action)
        {
            MenuEntry entry = new MenuEntry(this, name, type, isVisible, action);
            _menuEntries.Add(entry);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            float scrollBarPos = viewport.Width / 2f;
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                _menuEntries[i].Initialize();
                scrollBarPos = Math.Min(scrollBarPos, (viewport.Width - _menuEntries[i].GetWidth()) / 2f);
            }            

            _menuTop = viewport.Height * 0.45f;
        }

        /// <summary>Returns the index of the menu entry at the position of the given mouse state.</summary>
        /// <returns>Index of menu entry if valid, -1 otherwise</returns>
        private int GetMenuEntryAt(Vector2 position)
        {
            int index = 0;
            foreach (MenuEntry entry in _menuEntries)
            {
                float width = entry.GetWidth();
                float height = entry.GetHeight();
                Rectangle rect = new Rectangle((int)(entry.Position.X - width / 2f), (int)(entry.Position.Y - height / 2f), (int)width, (int)height);

                if (rect.Contains((int)position.X, (int)position.Y) && entry.IsSelectable())
                    return index;

                index++;
            }
            return -1;
        }

        /// <summary>Responds to user input, changing the selected entry and accepting or cancelling the menu.</summary>
        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            // Mouse or touch on a menu item
            int hoverIndex = GetMenuEntryAt(input.Cursor);
            if (hoverIndex > -1 && _menuEntries[hoverIndex].IsSelectable())
                _selectedEntry = hoverIndex;
            else
                _selectedEntry = -1;
            
            // Accept or cancel the menu? 
            if (input.IsMenuSelect() && _selectedEntry != -1)
            {
                _menuEntries[_selectedEntry].Act();               
            }
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            Vector2 position = Vector2.Zero;
            position.Y = _menuTop;

            // update each menu entry's location in turn
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2f;

                // set the entry's position
                _menuEntries[i].Position = position;
                _menuEntries[i].Alpha = 1f;

                // move down for the next entry the size of this entry
                position.Y += _menuEntries[i].GetHeight();
            }
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                bool isSelected = IsActive && (i == _selectedEntry);
                _menuEntries[i].Update(isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin();
            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                _menuEntries[i].Draw();
            }

            spriteBatch.End();
        }
    }
}
