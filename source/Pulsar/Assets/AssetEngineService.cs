using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Assets
{
    public sealed class AssetEngineService : IAssetEngineService, IDisposable
    {
        #region Fields

        private AssetEngine _engine;
        private bool _isDisposed;

        #endregion

        #region Constructors

        public AssetEngineService(XnaGame game)
        {
            if(game == null)
                throw new ArgumentNullException("game");

            if(game.Services.GetService(typeof(IAssetEngineService)) != null)
                throw new Exception("");

            game.Services.AddService(typeof(IAssetEngineService), this);
            _engine = new AssetEngine(game.Services);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) return;

            _engine.Dispose();
            _engine = null;

            _isDisposed = true;
        }

        #endregion

        #region Properties

        public AssetEngine AssetEngine
        {
            get { return _engine; }
        }

        #endregion
    }
}
