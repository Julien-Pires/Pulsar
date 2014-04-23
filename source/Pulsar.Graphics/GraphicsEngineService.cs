using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Service containing the graphic engine
    /// </summary>
    public sealed class GraphicsEngineService : IGraphicsEngineService, IDisposable
    {
        #region Fields

        private bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GraphicsEngineService class
        /// </summary>
        /// <param name="game">Instance of Game class</param>
        public GraphicsEngineService(XnaGame game)
        {
            if (game == null) 
                throw new ArgumentNullException("game");

            if (game.Services.GetService(typeof(IGraphicsEngineService)) != null) 
                throw new ArgumentException("GraphicsEngineService already present");

            game.Services.AddService(typeof(IGraphicsEngineService), this);
            Engine = new GraphicsEngine(game.Services);
            Engine.Initialize();
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
                Engine.Dispose();
            }
            finally
            {
                Engine = null;

                _isDisposed = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graphic engine
        /// </summary>
        public GraphicsEngine Engine { get; private set; }

        #endregion
    }
}
