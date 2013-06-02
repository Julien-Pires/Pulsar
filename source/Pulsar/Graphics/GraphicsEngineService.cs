using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Graphics
{
    public sealed class GraphicsEngineService : IGraphicsEngineService
    {
        #region Fields

        private GraphicsEngine engine;
        private XnaGame game;

        #endregion

        #region Constructors

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

        public GraphicsEngine Engine
        {
            get { return this.engine; }
        }

        #endregion
    }
}
