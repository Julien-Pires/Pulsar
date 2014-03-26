using System;

using Pulsar.Assets;
using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Shader asset
    /// </summary>
    [AssetLoader(AssetTypes = new[] { typeof(Shader) })]
    public sealed class ShaderLoader : AssetLoader
    {
        #region Fields

        private readonly LoadedAsset _result = new LoadedAsset();
        private GraphicsEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderLoader class
        /// </summary>
        internal ShaderLoader()
        {
        }

        #endregion

        #region Methods

        public override void Initialize(AssetEngine engine, IServiceProvider serviceProvider)
        {
            base.Initialize(engine, serviceProvider);

            IGraphicsEngineService engineService =
                serviceProvider.GetService(typeof (IGraphicsEngineService)) as IGraphicsEngineService;
            if(engineService == null)
                throw new Exception("");

            _engine = engineService.Engine;
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
        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();

            Shader shader = _engine.ShaderManager.GetShader(assetName);
            if (shader == null)
                LoadFromFile<Shader>(path, assetFolder, _result);

            return _result;
        }

        #endregion
    }
}
