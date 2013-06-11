using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Pulsar.Extension;

using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Pulsar.Input
{
    /// <summary>
    /// Allows to retrieve state of the keyboard
    /// </summary>
    public static class Keyboard
    {
        #region Fields

        private static Keys[] AllDigital;
        internal static List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private static KeyboardState previousState;
        private static KeyboardState currentState;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of Keyboard class
        /// </summary>
        static Keyboard()
        {
            Keyboard.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the keyboard
        /// </summary>
        internal static void Initialize()
        {
#if !XBOX
            Keyboard.AllDigital = (Keys[])Enum.GetValues(typeof(Keys));
#else
            Keyboard.AllDigital = EnumExtension.GetValues<Keys>();
#endif
        }

        /// <summary>
        /// Update keyboard states
        /// </summary>
        internal static void Update()
        {
            Keyboard.previousState = Keyboard.currentState;
            Keyboard.currentState = XnaKeyboard.GetState();

            Keyboard.ButtonPressed.Clear();
            for (int i = 0; i < Keyboard.AllDigital.Length; i++)
            {
                if(Keyboard.IsPressed(Keyboard.AllDigital[i]))
                {
                    AbstractButton btn = new AbstractButton(Keyboard.AllDigital[i]);
                    Keyboard.ButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed));
                }
            }
        }

        /// <summary>
        /// Check if any key is pressed
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyPressed()
        {
            return Keyboard.ButtonPressed.Count > 0;
        }

        /// <summary>
        /// Check if a key is pressed
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key has been pressed otherwise false</returns>
        public static bool IsPressed(Keys key)
        {
            return Keyboard.previousState.IsKeyUp(key) && Keyboard.currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if a key has been released
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key has been released otherwise false</returns>
        public static bool IsReleased(Keys key)
        {
            return Keyboard.previousState.IsKeyDown(key) && Keyboard.currentState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if a key is down
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key is down otherwise false</returns>
        public static bool IsDown(Keys key)
        {
            return Keyboard.currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if the key is up
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Return true if the key is up otherwise false</returns>
        public static bool IsUp(Keys key)
        {
            return Keyboard.currentState.IsKeyUp(key);
        }

        #endregion
    }
}
