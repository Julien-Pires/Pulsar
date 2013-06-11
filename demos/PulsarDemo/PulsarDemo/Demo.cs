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

using Pulsar.Input;
using Pulsar.Game;
using Pulsar.Core;
using Pulsar.Graphics;
using Pulsar.Assets;

using PulsarDemo.SceneDemo;
using PulsarDemo.SceneDemo.SolarSystem;

namespace PulsarDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : GameApplication
    {
        private List<ISceneDemo> scenes = new List<ISceneDemo>();
        private ISceneDemo currentScene = null;
        private CameraController camCtrl;

        public Demo()
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.CreateCamera();
            this.CreateScene();
            this.SwitchScene(0);
        }

        private void CreateCamera()
        {
            InputService inService = (InputService)GameApplication.GameServices.GetService(typeof(IInputService));
            if (inService == null)
            {
                throw new ArgumentException("Failed to find InputService");
            }
            this.camCtrl = new CameraController(inService.Input);
        }

        private void CreateScene()
        {
            this.scenes.Add(new SolarSystem(this.gEngine.Engine, this.camCtrl));
        }

        private void SwitchScene(int sceneIndex)
        {
            ISceneDemo scene = this.scenes[sceneIndex];
            this.currentScene = scene;
            scene.Activate();
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
            base.Update(gameTime);

            if (this.currentScene != null)
            {
                this.currentScene.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.currentScene != null)
            {
                this.currentScene.Render();
            }
        }
    }
}
