using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Graph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Class used as an entry point for the Graphics system
    /// </summary>
    public sealed class Root
    {
        #region Fields

        private Renderer renderer = null;
        private ContentManager content = null;
        private Dictionary<string, SceneGraph> scenes = new Dictionary<string, SceneGraph>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the Root class
        /// </summary>
        /// <param name="gDevice">Graphic device used by the rendering system</param>
        /// <param name="content">Content manager used by the entire system</param>
        public Root(GraphicsDevice gDevice, ContentManager content)
        {
            this.content = content;
            this.renderer = new Renderer(gDevice);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a scene graph
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph class</returns>
        public SceneGraph CreateSceneGraph(string name)
        {
            SceneGraph graph = new SceneGraph(this.renderer);
            
            scenes.Add(name, graph);

            return graph;
        }

        /// <summary>
        /// Get a scene graph by his name
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph</returns>
        public SceneGraph GetScene(string name)
        {
            if (!this.scenes.ContainsKey(name))
                return null;

            return this.scenes[name];
        }

        #endregion
    }
}
