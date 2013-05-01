using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Pulsar.Input
{
    public class GamePad : IPeripheric
    {
        #region Fields

        private PlayerIndex gamePadIndex;
        private GamePadState previousState;
        private GamePadState currentState;

        #endregion

        #region Constructors

        internal GamePad(PlayerIndex index)
        {
            this.gamePadIndex = index;
        }

        #endregion

        #region Methods

        internal void Update()
        {
            this.previousState = this.currentState;
            this.currentState = XnaGamePad.GetState(this.gamePadIndex);
        }

        public bool IsJustPressed(Buttons button)
        {
            return (this.previousState.IsButtonUp(button)) && (this.currentState.IsButtonDown(button));
        }

        public bool IsJustReleased(Buttons button)
        {
            return (this.previousState.IsButtonDown(button)) && (this.currentState.IsButtonUp(button));
        }

        public bool IsDown(Buttons button)
        {
            return this.currentState.IsButtonDown(button);
        }

        public bool IsUp(Buttons button)
        {
            return this.currentState.IsButtonUp(button);
        }

        #endregion
    }
}
