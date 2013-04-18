using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Class describing a scene graph
    /// </summary>
    public sealed class SceneTree
    {
        #region Fields

        private Renderer renderer = null;
        private CameraManager camManager = new CameraManager();
        private RenderQueue queue = new RenderQueue();
        private SceneNode root = null;
        private Dictionary<string, SceneNode> nodesMap = new Dictionary<string, SceneNode>();
        private Dictionary<string, IMovable> movablesMap = new Dictionary<string, IMovable>();
        private EntityFactory entityFactory = new EntityFactory();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the SceneGraph class
        /// </summary>
        /// <param name="renderer">GraphicsRenderer instance</param>
        internal SceneTree(Renderer renderer)
        {
            this.renderer = renderer;
            this.root = this.CreateNode("Root");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call to draw the entire scene
        /// </summary>
        public void RenderScene()
        {
            Camera cam = this.camManager.Current;

            this.UpdateGraph();
            this.Clean();

            if (cam != null)
            {
                this.FindVisibleObjects(cam);
            }

            this.renderer.Render(this.queue, this.camManager.Current);
        }
        
        /// <summary>
        /// Update the scene graph
        /// </summary>
        private void UpdateGraph()
        {
            this.root.Update(true, false);
        }

        /// <summary>
        /// Clean the render queue and batch for the next frame
        /// </summary>
        private void Clean()
        {
            this.queue.Clear();
        }

        /// <summary>
        /// Find all the visible objects in the scene
        /// </summary>
        /// <param name="cam">Current camera</param>
        private void FindVisibleObjects(Camera cam)
        {
            this.root.FindVisibleObjects(cam, this.queue, true);
        }

        /// <summary>
        /// Create an Entity instance
        /// </summary>
        /// <param name="modelName">Name of the model</param>
        /// <param name="name">Name of the Entity</param>
        /// <returns>Returns an Entity instance</returns>
        public Entity CreateEntity(string modelName, string name)
        {
            Entity ent = this.entityFactory.Create(modelName, name);
            
            this.movablesMap.Add(name, ent);

            return ent;
        }

        /// <summary>
        /// Create a node specific to this graph
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Return a new node</returns>
        internal SceneNode CreateNode(string name)
        {
            SceneNode node;
            this.nodesMap.TryGetValue(name, out node);
            if (node != null)
            {
                throw new Exception(string.Format("A node named {0} already exists in this scene graph", name));
            }

            node = new SceneNode(this, name);
            this.nodesMap.Add(name, node);

            return node;
        }

        internal bool RemoveNode(string name)
        {
            return this.nodesMap.Remove(name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the camera manager instance
        /// </summary>
        public CameraManager CameraManager
        {
            get { return this.camManager; }
        }

        /// <summary>
        /// Get the root scene node
        /// </summary>
        public SceneNode Root
        {
            get { return this.root; }
        }

        #endregion
    }
}
