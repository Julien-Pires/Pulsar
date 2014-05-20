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

        public const string Storage = "GraphicsEngine_Storage";
        public const string LoadersCategory = "GraphicsLoaders";
        public const string ShaderFolderName = "Shaders";
        public const string TextureFolderName = "Textures";
        public const string MaterialFolderName = "Materials";
        public const string MeshFolderName = "Mesh";

        private const string ShaderFolderPath = "Content/Graphics/Shaders";
        private const string TextureFolderPath = "Content/Graphics/Texture";
        private const string MaterialFolderPath = "Content/Graphics/Material";
        private const string MeshFolderPath = "Content/Graphics/Mesh";

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
            _assetEngine = assetEngine;

            Storage storage = assetEngine.CreateStorage(Storage);
            TextureFolder = storage.AddFolder(TextureFolderPath, TextureFolderName);
            ShaderFolder = storage.AddFolder(ShaderFolderPath, ShaderFolderName);
            MaterialFolder = storage.AddFolder(MaterialFolderPath, MaterialFolderName);
            MeshFolder = storage.AddFolder(MeshFolderPath, MeshFolderName);
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
                _assetEngine.DestroyStorage(Storage);
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
