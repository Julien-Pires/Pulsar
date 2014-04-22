using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Input;
using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

using Pulsar.Extension;

namespace Pulsar.Input
{
    /// <summary>
    /// Allows to retrieve state of the keyboard
    /// </summary>
    public static class Keyboard
    {
        #region Fields

        internal static readonly List<ButtonEvent> InternalButtonPressed = new List<ButtonEvent>(8);

        private static readonly ReadOnlyCollection<ButtonEvent> ReadOnlyButtonPressed;
        private static readonly Keys[] AllDigital;
        private static KeyboardState _previousState;
        private static KeyboardState _currentState;

        #endregion

        #region Static constructors

        /// <summary>
        /// Static constructor of Keyboard class
        /// </summary>
        static Keyboard()
        {
            AllDigital = EnumExtension.GetValues<Keys>();
            ReadOnlyButtonPressed = new ReadOnlyCollection<ButtonEvent>(InternalButtonPressed);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Updates keyboard states
        /// </summary>
        internal static void Update()
        {
            _previousState = _currentState;
            _currentState = XnaKeyboard.GetState();

            InternalButtonPressed.Clear();
            for (int i = 0; i < AllDigital.Length; i++)
            {
                if (_currentState.IsKeyUp(AllDigital[i]))
                    continue;

                AbstractButton btn = new AbstractButton(AllDigital[i]);
                InternalButtonPressed.Add(new ButtonEvent(btn));
            }
        }

        /// <summary>
        /// Checks if any key is pressed
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyPressed()
        {
            return InternalButtonPressed.Count > 0;
        }

        /// <summary>
        /// Checks if a key is pressed
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key has been pressed otherwise false</returns>
        public static bool IsPressed(Keys key)
        {
            return _previousState.IsKeyUp(key) && _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if a key has been released
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key has been released otherwise false</returns>
        public static bool IsReleased(Keys key)
        {
            return _previousState.IsKeyDown(key) && _currentState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if a key is down
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key is down otherwise false</returns>
        public static bool IsDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the key is up
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key is up otherwise false</returns>
        public static bool IsUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        #endregion

        #region Static properties

        /// <summary>
        /// Gets the keyboard keys that are currently being pressed
        /// </summary>
        public static ReadOnlyCollection<ButtonEvent> ButtonPressed
        {
            get { return ReadOnlyButtonPressed; }
        }

        #endregion
    }
}
