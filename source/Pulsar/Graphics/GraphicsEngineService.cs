using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Service containing the graphic engine
    /// </summary>
    public sealed class GraphicsEngineService : IGraphicsEngineService
    {
        #region Fields

        private GraphicsEngine engine;
        private XnaGame game;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GraphicsEngineService class
        /// </summary>
        /// <param name="game">Instance of Game class</param>
        public GraphicsEngineService(XnaGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game", "game cannot be null");
            }
            this.game = game;
            if (this.game.Services.GetService(typeof(IGraphicsEngineService)) != null)
            {
                throw new ArgumentException("GraphicsEngineService already present");
            }
            this.game.Services.AddService(typeof(IGraphicsEngineService), this);
            this.engine = new GraphicsEngine(game.Services);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the graphic engine
        /// </summary>
        public GraphicsEngine Engine
        {
            get { return this.engine; }
        }

        #endregion
    }
}
