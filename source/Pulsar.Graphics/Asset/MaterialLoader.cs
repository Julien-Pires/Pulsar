using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Material asset
    /// </summary>
    [AssetLoader(AssetTypes = new[]{ typeof(Material) })]
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
            MaterialParameters matParameters;
            if (parameters != null)
            {
                matParameters = parameters as MaterialParameters;
                if(matParameters == null)
                    throw new Exception("Invalid parameters for this loader");
            }
            else
                matParameters = _defaultParameter;

            Material material;
            switch (matParameters.Source)
            {
                case AssetSource.FromFile:
                    throw new NotSupportedException("Cannot load material from file");

                case AssetSource.NewInstance:
                    material = new Material(assetName);
                    break;

                default:
                    throw new Exception("Invalid asset source provided");
            }
            _result.Reset();
            _result.Asset = material;

            return _result;
        }

        #endregion
    }
}
