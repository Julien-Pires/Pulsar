using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Base implementation of an asset loader
    /// </summary>
    public abstract class AssetLoader : IAssetLoader
    {
        #region Methods

        /// <summary>
        /// Initializes the loader
        /// </summary>
        /// <param name="engine">Asset engine using this loader</param>
        /// <param name="serviceProvider">Service provider</param>
        public virtual void Initialize(AssetEngine engine, IServiceProvider serviceProvider)
        {
        }

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <param name="assetFolder">Folder where the asset will be stored</param>
        /// <returns>Returns a loaded asset</returns>
        public abstract LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder);

        /// <summary>
        /// Loads an asset from a file from a specified folder
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="path">Path of the asset</param>
        /// <param name="assetFolder">Folder used to load the file</param>
        /// <param name="result">Result containing everything about the loaded asset</param>
        protected void LoadFromFile<T>(string path, AssetFolder assetFolder, LoadedAsset result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (assetFolder == null)
                throw new ArgumentNullException("assetFolder");

            assetFolder.LoadFromFile<T>(path, result);
        }

        #endregion
    }
}
