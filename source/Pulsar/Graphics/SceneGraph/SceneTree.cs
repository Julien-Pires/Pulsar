using System;
using System.Collections.Generic;

using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Class describing a scene graph
    /// </summary>
    public sealed class SceneTree
    {
        #region Fields

        private readonly Renderer _renderer;
        private readonly CameraManager _camManager;
        private readonly RenderQueue _queue = new RenderQueue();
        private readonly SceneNode _root;
        private readonly Dictionary<string, SceneNode> _nodesMap = new Dictionary<string, SceneNode>();
        private readonly Dictionary<string, IMovable> _movablesMap = new Dictionary<string, IMovable>();
        private readonly EntityFactory _entityFactory = new EntityFactory();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the SceneGraph class
        /// </summary>
        /// <param name="renderer">GraphicsRenderer instance</param>
        internal SceneTree(Renderer renderer)
        {
            _renderer = renderer;
            _camManager = new CameraManager(this);
            _root = CreateNode("Root");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call to draw the entire scene
        /// </summary>
        public void RenderScene(Viewport vp, Camera cam)
        {
            UpdateGraph();
            Clean();
            
            FindVisibleObjects(cam);
            _renderer.Render(vp, cam, _queue);
        }
        
        /// <summary>
        /// Update the scene graph
        /// </summary>
        private void UpdateGraph()
        {
            _root.Update(true, false);
        }

        /// <summary>
        /// Clean the render queue and batch for the next frame
        /// </summary>
        private void Clean()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Find all the visible objects in the scene
        /// </summary>
        /// <param name="cam">Current camera</param>
        private void FindVisibleObjects(Camera cam)
        {
            _root.FindVisibleObjects(cam, _queue, true);
        }

        /// <summary>
        /// Create an Entity instance
        /// </summary>
        /// <param name="modelName">Name of the model</param>
        /// <param name="name">Name of the Entity</param>
        /// <returns>Returns an Entity instance</returns>
        public Entity CreateEntity(string modelName, string name)
        {
            Entity ent = _entityFactory.Create(modelName, name);
            _movablesMap.Add(name, ent);

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
            _nodesMap.TryGetValue(name, out node);
            if (node != null) throw new Exception(string.Format("A node named {0} already exists in this scene graph", name));

            node = new SceneNode(this, name);
            _nodesMap.Add(name, node);

            return node;
        }

        /// <summary>
        /// Remove a node
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Returns true if the node is removed successfully otherwise false</returns>
        internal bool RemoveNode(string name)
        {
            return _nodesMap.Remove(name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the camera manager instance
        /// </summary>
        public CameraManager CameraManager
        {
            get { return _camManager; }
        }

        /// <summary>
        /// Get the root scene node
        /// </summary>
        public SceneNode Root
        {
            get { return _root; }
        }

        #endregion
    }
}
