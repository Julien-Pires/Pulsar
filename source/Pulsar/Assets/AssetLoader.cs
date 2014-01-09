using System;

namespace Pulsar.Assets
{
    public abstract class AssetLoader : IAssetLoader
    {
        #region Methods

        public abstract LoadResult Load<T>(string assetName, object parameters, Storage storage);

        protected void LoadFromFile<T>(string assetName, Storage storage, LoadResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (storage == null)
                throw new ArgumentNullException("storage");

            storage.LoadFromFile<T>(assetName, result);
        }

        #endregion

        #region Properties

        public abstract Type[] SupportedTypes { get; }

        #endregion
    }
}
