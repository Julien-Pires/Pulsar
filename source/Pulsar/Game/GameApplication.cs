using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Pulsar.Input;
using Pulsar.Graphics;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Game
{
    /// <summary>
    /// Game class containing update and render loop
    /// </summary>
    public class GameApplication : XnaGame
    {
        #region Fields

        private GraphicsEngineService _graphicsEngineService;
        private InputService _inputService;
        private GraphicsDeviceManager _deviceManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GameApplication class
        /// </summary>
        public GameApplication()
        {
            GameServices = Services;
            Content.RootDirectory = "Content";
            Services.AddService(typeof(ContentManager), Content);
            _deviceManager = new GraphicsDeviceManager(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the game
        /// </summary>
        protected override void Initialize()
        {
            _graphicsEngineService = new GraphicsEngineService(this);
            _inputService = new InputService(this);

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
#if WINDOWS
                bool locked = false;
#elif XBOX
                Monitor.Enter(this);
#endif
                try
                {
#if WINDOWS
                    Monitor.Enter(this, ref locked);
#endif
                    _graphicsEngineService.Dispose();
                }
                finally
                {
#if WINDOWS
                    if (locked) Monitor.Exit(this);
#elif XBOX
                    Monitor.Exit(this);
#endif
                }
            }
        }

        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="gameTime">Information about time</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _inputService.Input.Update();
        }

        /// <summary>
        /// Draw the frame
        /// </summary>
        /// <param name="gameTime">Information about time</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _graphicsEngineService.Engine.Render(gameTime);
        }

        #endregion

        #region Properties

        public GraphicsEngine GraphicsEngine
        {
            get { return _graphicsEngineService.Engine; }
        }

        public InputManager InputEngine
        {
            get { return _inputService.Input; }
        }

        /// <summary>
        /// Get the service container
        /// </summary>
        public static GameServiceContainer GameServices { get; internal set; }

        #endregion
    }
}
