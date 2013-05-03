using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Pulsar.Input
{
    public static class Keyboard
    {
        #region Fields

        private static KeyboardState previousState;
        private static KeyboardState currentState;

        #endregion

        #region Methods

        internal static void Update()
        {
            Keyboard.previousState = Keyboard.currentState;
            Keyboard.currentState = XnaKeyboard.GetState();
        }

        public static bool IsPressed(Keys key)
        {
            return Keyboard.previousState.IsKeyUp(key) && Keyboard.currentState.IsKeyDown(key);
        }

        public static bool IsReleased(Keys key)
        {
            return Keyboard.previousState.IsKeyDown(key) && Keyboard.currentState.IsKeyUp(key);
        }

        public static bool IsDown(Keys key)
        {
            return Keyboard.currentState.IsKeyDown(key);
        }

        public static bool IsUp(Keys key)
        {
            return Keyboard.currentState.IsKeyUp(key);
        }

        #endregion
    }
}
