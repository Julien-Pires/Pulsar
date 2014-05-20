using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Material asset
    /// </summary>
    [AssetLoader(AssetTypes = new[] { typeof(Material) }, LazyInitCategory = GraphicsStorage.LoadersCategory)]
    public sealed class MaterialLoader : AssetLoader
    {
        #region Fields

        private readonly LoadedAsset _result = new LoadedAsset();
        private readonly MaterialParameters _defaultParameter = new MaterialParameters();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MaterialLoader class
        /// </summary>
        internal MaterialLoader()
        {
        }

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

            MaterialParameters materialParameters;
            if (parameters != null)
            {
                materialParameters = parameters as MaterialParameters;
                if (materialParameters == null)
                    throw new Exception("Invalid parameters for this loader");
            }
            else
            {
                materialParameters = _defaultParameter;
                materialParameters.Filename = path;
            }

            switch (materialParameters.Source)
            {
                case AssetSource.FromFile:
                    LoadFromFile<Material>(path, assetFolder, _result);
                    break;

                case AssetSource.NewInstance:
                    _result.Asset = new Material(assetName);
                    break;

                default:
                    throw new Exception("Invalid asset source provided");
            }

            return _result;
        }

        #endregion
    }
}
