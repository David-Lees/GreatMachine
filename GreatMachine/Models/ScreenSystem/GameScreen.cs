/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GreatMachine.Models.ScreenSystem
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        Active,
        Hidden,
    }

    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
    {
        private GestureType _enabledGestures = GestureType.None;

        protected GameScreen()
        {
            ScreenState = ScreenState.Hidden;
            HasCursor = false;
            HasVirtualStick = false;
        }

        public bool HasCursor { get; set; }
        public bool HasVirtualStick { get; set; }


        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup { get; protected set; }

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState { get; set; }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive => ScreenState == ScreenState.Active;


        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return _enabledGestures; }
            protected set
            {
                _enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourself.
                if (ScreenState == ScreenState.Active)
                    TouchPanel.EnabledGestures = value;
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputHelper input, GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the screen should draw rendertargets.
        /// </summary>
        public virtual void PreDraw(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            ScreenManager.RemoveScreen(this);
        }

    }
}
