using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Assets
{
    /// <summary>
    /// Handles the asset engine
    /// </summary>
    public sealed class AssetEngineService : IAssetEngineService, IDisposable
    {
        #region Fields

        private AssetEngine _engine;
        private bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructors of AssetEngineService class
        /// </summary>
        /// <param name="game">Game instance</param>
        public AssetEngineService(XnaGame game)
        {
            if(game == null)
                throw new ArgumentNullException("game");

            if(game.Services.GetService(typeof(IAssetEngineService)) != null)
                throw new Exception("IAssetEngineService already presents");

            game.Services.AddService(typeof(IAssetEngineService), this);
            _engine = new AssetEngine(game.Services);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                _engine.Dispose();
            }
            finally
            {
                _engine = null;
            }

            _isDisposed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the asset engine
        /// </summary>
        public AssetEngine AssetEngine
        {
            get { return _engine; }
        }

        #endregion
    }
}
