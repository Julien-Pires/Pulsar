using System;

using Pulsar.Assets;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Manages the asset storage used by the graphics engine
    /// </summary>
    internal sealed class GraphicsStorage : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private AssetEngine _assetEngine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GraphicsStorage class
        /// </summary>
        /// <param name="assetEngine">Asset engine</param>
        internal GraphicsStorage(AssetEngine assetEngine)
        {
            ContentReaderHelper.AddReadMethod(c => c.ReadExternalReference<Texture>(), typeof(Texture));

            Storage storage = assetEngine.CreateStorage(GraphicsConstant.Storage);
            TextureFolder = storage.AddFolder(GraphicsConstant.TextureFolder, GraphicsConstant.TextureFolderName);
            ShaderFolder = storage.AddFolder(GraphicsConstant.ShaderFolder, GraphicsConstant.ShaderFolderName);
            MaterialFolder = storage.AddFolder(GraphicsConstant.MaterialFolder, GraphicsConstant.MaterialFolderName);
            MeshFolder = storage.AddFolder(GraphicsConstant.MeshFolder, GraphicsConstant.MeshFolderName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                _assetEngine.DestroyStorage(GraphicsConstant.Storage);
            }
            finally
            {
                TextureFolder = null;
                ShaderFolder = null;
                MaterialFolder = null;
                MeshFolder = null;
                _assetEngine = null;

                _isDisposed = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the texture folder
        /// </summary>
        public AssetFolder TextureFolder { get; private set; }

        /// <summary>
        /// Gets the shader folder
        /// </summary>
        public AssetFolder ShaderFolder { get; private set; }

        /// <summary>
        /// Gets the material folder
        /// </summary>
        public AssetFolder MaterialFolder { get; private set; }

        /// <summary>
        /// Gets the mesh folder
        /// </summary>
        public AssetFolder MeshFolder { get; private set; }

        #endregion
    }
}
