using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        protected GraphicsDeviceManager gDeviceMngr;
        protected GraphicsEngineService gEngine;
        protected InputService inputService;
        protected SpriteBatch spriteBatch;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GameApplication class
        /// </summary>
        public GameApplication()
        {
            GameApplication.GameServices = this.Services;
            this.Content.RootDirectory = "Content";
            this.Services.AddService(typeof(ContentManager), this.Content);
            this.gDeviceMngr = new GraphicsDeviceManager(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the game
        /// </summary>
        protected override void Initialize()
        {
            this.gEngine = new GraphicsEngineService(this);
            this.inputService = new InputService(this);

            base.Initialize();
        }

        /// <summary>
        /// Load the content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Unload the content
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="gameTime">Information about time</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.inputService.Input.Update();
            this.gEngine.Engine.Update(gameTime);
        }

        /// <summary>
        /// Draw the frame
        /// </summary>
        /// <param name="gameTime">Information about time</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the service container
        /// </summary>
        public static GameServiceContainer GameServices { get; internal set; }

        #endregion
    }
}
