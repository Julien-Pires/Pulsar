using System;

namespace Pulsar.Assets
{
    public abstract class AssetLoader : IAssetLoader
    {
        #region Methods

        public virtual void Initialize(AssetEngine engine)
        {
        }

        public abstract LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder);

        protected void LoadFromFile<T>(string path, AssetFolder assetFolder, LoadedAsset result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (assetFolder == null)
                throw new ArgumentNullException("assetFolder");

            assetFolder.LoadFromFile<T>(path, result);
        }

        #endregion

        #region Properties

        public abstract string Name { get; }

        public abstract Type[] SupportedTypes { get; }

        #endregion
    }
}
