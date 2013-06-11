using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Class used as an entry point for the Graphics system
    /// </summary>
    public sealed class GraphicsEngine
    {
        #region Fields

        private GameServiceContainer services;
        private FrameInfo frameInfo = new FrameInfo();
        private Renderer renderer;
        private BufferManager bufferManager;
        private Dictionary<string, SceneTree> scenes = new Dictionary<string, SceneTree>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsEngine class
        /// </summary>
        /// <param name="services">GameServiceContainer used by the engine</param>
        public GraphicsEngine(GameServiceContainer services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("Services cannot be null");
            }
            this.services = services;
            GraphicsDeviceManager deviceService = services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;
            if (deviceService == null)
            {
                throw new ArgumentNullException("No Graphics device service found");
            }
            this.renderer = new Renderer(deviceService.GraphicsDevice, this.frameInfo);
            this.bufferManager = new BufferManager(deviceService);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the graphic engine
        /// </summary>
        /// <param name="time">Time since the last update</param>
        public void Update(GameTime time)
        {
            this.frameInfo.Update(time);
        }

        /// <summary>
        /// Create a scene graph
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph class</returns>
        public SceneTree CreateSceneGraph(string name)
        {
            SceneTree graph = new SceneTree(this.renderer);
            
            scenes.Add(name, graph);

            return graph;
        }

        /// <summary>
        /// Get a scene graph by his name
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph</returns>
        public SceneTree GetScene(string name)
        {
            if (!this.scenes.ContainsKey(name))
                return null;

            return this.scenes[name];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get information about the last frame
        /// </summary>
        public FrameInfo FrameInfo
        {
            get { return this.frameInfo; }
        }

        /// <summary>
        /// Get the buffer manager
        /// </summary>
        public BufferManager BufferManager
        {
            get { return this.bufferManager; }
        }

        /// <summary>
        /// Get the renderer
        /// </summary>
        internal Renderer Renderer
        {
            get { return this.renderer; }
        }

        #endregion
    }
}
