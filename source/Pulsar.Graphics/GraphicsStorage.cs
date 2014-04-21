using System;

using Pulsar.Assets;

namespace Pulsar.Graphics
{
    internal sealed class GraphicsStorage : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private AssetEngine _assetEngine;

        #endregion

        #region Constructors

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

        public void Dispose()
        {
            if(_isDisposed) return;

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

        public AssetFolder TextureFolder { get; private set; }

        public AssetFolder ShaderFolder { get; private set; }

        public AssetFolder MaterialFolder { get; private set; }

        public AssetFolder MeshFolder { get; private set; }

        #endregion
    }
}
