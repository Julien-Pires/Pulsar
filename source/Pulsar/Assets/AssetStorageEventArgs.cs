using System;

namespace Pulsar.Assets
{
    public sealed class AssetStorageEventArgs : EventArgs
    {
        #region Fields

        public readonly AssetStorage Storage;

        #endregion

        #region Constructors

        public AssetStorageEventArgs(AssetStorage storage)
        {
            this.Storage = storage;
        }

        #endregion
    }
}
