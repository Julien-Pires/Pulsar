using System;

namespace Pulsar.Assets
{
    public interface IAssetLoader
    {
        #region Methods

        LoadResult Load<T>(string assetName, object parameters, Storage storage);

        #endregion

        #region Properties

        Type[] SupportedTypes { get; }

        #endregion
    }
}
