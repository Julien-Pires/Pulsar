using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using XnaKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Pulsar.Input
{
    public class Keyboard : IPeripheric
    {
        #region Fields

        private KeyboardState previousState;
        private KeyboardState currentState;

        #endregion

        #region Methods

        internal void Update()
        {
            this.previousState = this.currentState;
            this.currentState = XnaKeyboard.GetState();
        }

        public bool IsJustPressed(Keys key)
        {
            return this.previousState.IsKeyUp(key) && this.currentState.IsKeyDown(key);
        }

        public bool IsJustReleased(Keys key)
        {
            return this.previousState.IsKeyDown(key) && this.currentState.IsKeyUp(key);
        }

        public bool IsDown(Keys key)
        {
            return this.currentState.IsKeyDown(key);
        }

        public bool IsUp(Keys key)
        {
            return this.currentState.IsKeyUp(key);
        }

        #endregion
    }
}
