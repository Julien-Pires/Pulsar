using System;

namespace Pulsar.Components
{
    public sealed class GameObjectEventArgs : EventArgs
    {
        #region Fields

        public readonly GameObject GameObj;

        #endregion

        #region Constructors

        public GameObjectEventArgs(GameObject gameObj)
        {
            this.GameObj = gameObj;
        }

        #endregion
    }
}
