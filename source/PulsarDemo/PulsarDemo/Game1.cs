using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pulsar.Core;
using Pulsar.Graphics;
using Pulsar.Assets;

using Pulsar.SceneDemo;
using Pulsar.SceneDemo.SolarSystem;

namespace Pulsar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : GameApplication
    {
        private Root graphSystem = null;
        private List<ISceneDemo> demo = new List<ISceneDemo>();
        private ISceneDemo currentScene = null;
        private Input inHandler = null;

        public Game1()
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.graphSystem = new Root(this.gDeviceMngr.GraphicsDevice, Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.inHandler = Input.Instance;

            this.CreateScene();
            for (int i = 0; i < this.demo.Count; i++)
            {
                this.demo[i].Load();
            }
        }

        private void CreateScene()
        {
            this.demo.Add(new SolarSystem(this.graphSystem, this.Content));

            this.currentScene = this.demo[0];
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.inHandler.Update();

            if (this.currentScene != null)
            {
                this.currentScene.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (this.currentScene != null)
            {
                this.currentScene.Render();
            }

            base.Draw(gameTime);
        }
    }
}
