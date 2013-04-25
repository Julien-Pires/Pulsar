using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Event argument for all event related to asset storage
    /// </summary>
    public sealed class AssetStorageEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Storage concerned by the event
        /// </summary>
        public readonly AssetStorage Storage;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetStorageEventArgs class
        /// </summary>
        /// <param name="storage">Storage instance</param>
        public AssetStorageEventArgs(AssetStorage storage)
        {
            this.Storage = storage;
        }

        #endregion
    }
}
