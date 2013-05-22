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
        private Renderer renderer;
        private Dictionary<string, SceneTree> scenes = new Dictionary<string, SceneTree>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the Root class
        /// </summary>
        /// <param name="gDevice">Graphic device used by the rendering system</param>
        public GraphicsEngine(GameServiceContainer services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("Services cannot be null");
            }
            this.services = services;
            GraphicsDeviceManager deviceService = services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            if (deviceService == null)
            {
                throw new ArgumentNullException("No Graphics device service found");
            }
            this.renderer = new Renderer(deviceService.GraphicsDevice);
        }

        #endregion

        #region Methods

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

        internal Renderer Renderer
        {
            get { return this.renderer; }
        }

        #endregion
    }
}
