using System;

namespace Pulsar.Assets
{
    public interface IAssetLoader
    {
        #region Methods

        void Initialize(AssetEngine engine);

        LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder);

        #endregion

        #region Properties

        string Name { get; }

        Type[] SupportedTypes { get; }

        #endregion
    }
}
