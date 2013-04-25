using System;

namespace Pulsar.Components
{
    /// <summary>
    /// Event argument that are used for all game objects event
    /// </summary>
    public sealed class GameObjectEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// GameObject concerned by the event
        /// </summary>
        public readonly GameObject GameObj;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GameObjectEventArgs class
        /// </summary>
        /// <param name="gameObj">Game object</param>
        public GameObjectEventArgs(GameObject gameObj)
        {
            this.GameObj = gameObj;
        }

        #endregion
    }
}
