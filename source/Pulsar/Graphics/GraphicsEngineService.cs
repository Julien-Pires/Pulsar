using System;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Service containing the graphic engine
    /// </summary>
    public sealed class GraphicsEngineService : IGraphicsEngineService
    {
        #region Fields

        private readonly GraphicsEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GraphicsEngineService class
        /// </summary>
        /// <param name="game">Instance of Game class</param>
        public GraphicsEngineService(Microsoft.Xna.Framework.Game game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.Services.GetService(typeof(IGraphicsEngineService)) != null) 
                throw new ArgumentException("GraphicsEngineService already present");

            game.Services.AddService(typeof(IGraphicsEngineService), this);
            _engine = new GraphicsEngine(game.Services);
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
