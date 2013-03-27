using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Game
{
    /// <summary>
    /// Game class containing update and render loop
    /// </summary>
    public class GameApplication : Microsoft.Xna.Framework.Game
    {
        #region Fields

        protected GraphicsDeviceManager gDeviceMngr = null;
        protected SpriteBatch spriteBatch = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GameApplication class
        /// </summary>
        public GameApplication()
        {
            this.gDeviceMngr = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the game
        /// </summary>
        protected override void Initialize()
        {
            this.Services.AddService(typeof(ContentManager), this.Content);
            GameApplication.GameGraphicsDevice = this.gDeviceMngr.GraphicsDevice;
            GameApplication.GameServices = this.Services;

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
        /// Get the graphic device
        /// </summary>
        public static GraphicsDevice GameGraphicsDevice { get; internal set; }

        /// <summary>
        /// Get the service container
        /// </summary>
        public static GameServiceContainer GameServices { get; internal set; }

        #endregion
    }
}
