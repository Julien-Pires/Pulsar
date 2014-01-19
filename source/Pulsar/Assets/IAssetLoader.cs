using System;

namespace Pulsar.Assets
{
    public interface IAssetLoader
    {
        #region Methods

        LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder);

        #endregion

        #region Properties

        string Name { get; }

        Type[] SupportedTypes { get; }

        #endregion
    }
}
