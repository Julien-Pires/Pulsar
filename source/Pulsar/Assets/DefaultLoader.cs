using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Implements default loader behaviour
    /// </summary>
    internal sealed class DefaultLoader : AssetLoader
    {
        #region Fields

        private const string LoaderName = "DefaultLoader";

        private readonly Type[] _supportedTypes = {};
        private readonly LoadedAsset _result = new LoadedAsset();

        #endregion

        #region Methods

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <param name="assetFolder">Folder where the asset will be stored</param>
        /// <returns>Returns a loaded asset</returns>
        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();
            _result.Name = assetName;
            LoadFromFile<T>(path, assetFolder, _result);

            return _result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the loader
        /// </summary>
        public override string Name
        {
            get { return LoaderName; }
        }

        /// <summary>
        /// Gets the list of assets supported by this loader
        /// </summary>
        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}
