using System;

namespace Pulsar.Input
{
    public sealed class GamePadEventArgs : EventArgs
    {
        #region Fields

        public readonly GamePad GamePad;

        #endregion

        #region Constructor

        public GamePadEventArgs(GamePad pad)
        {
            this.GamePad = pad;
        }

        #endregion
    }
}
