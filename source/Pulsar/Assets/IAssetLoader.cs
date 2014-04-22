using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Defines mechanisms to load custom asset
    /// </summary>
    public interface IAssetLoader
    {
        #region Methods

        /// <summary>
        /// Initializes the loader
        /// </summary>
        /// <param name="engine">Asset engine that use this loader</param>
        /// <param name="serviceProvider">Service provider</param>
        void Initialize(AssetEngine engine, IServiceProvider serviceProvider);

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <param name="assetFolder">Folder where the asset will be stored</param>
        /// <returns>Returns a loaded asset</returns>
        LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder);

        #endregion
    }
}
