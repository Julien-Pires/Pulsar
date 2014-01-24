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

        private readonly GraphicsEngine _engine;

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
            _engine = new GraphicsEngine(game.Services);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _engine.Dispose();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the graphic engine
        /// </summary>
        public GraphicsEngine Engine
        {
            get { return _engine; }
        }

        #endregion
    }
}
