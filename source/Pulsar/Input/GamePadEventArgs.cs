using System;

namespace Pulsar.Input
{
    /// <summary>
    /// Contains data about all event related to a gamepad
    /// </summary>
    public sealed class GamePadEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Gamepad associated to the event
        /// </summary>
        public readonly GamePad GamePad;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the GamePadEventArgs class
        /// </summary>
        /// <param name="pad">Gamepad associated with the event</param>
        public GamePadEventArgs(GamePad pad)
        {
            this.GamePad = pad;
        }

        #endregion
    }
}
