using System;
using System.Collections.Generic;

using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Represents a basic scene graph
    /// </summary>
    public class SceneTree
    {
        #region Fields

        private readonly Renderer _renderer;
        private readonly CameraManager _camManager;
        private readonly RenderQueue _queue = new RenderQueue();
        private readonly SceneNode _root;
        private readonly Dictionary<string, IMovable> _movablesMap = new Dictionary<string, IMovable>();
        private readonly List<SceneNode>  _nodes = new List<SceneNode>();
        private readonly EntityFactory _entityFactory = new EntityFactory();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SceneTree class
        /// </summary>
        /// <param name="renderer">Renderer instance</param>
        internal SceneTree(Renderer renderer)
        {
            _renderer = renderer;
            _camManager = new CameraManager(this);
            _root = CreateNode();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws the entire scene
        /// </summary>
        public void RenderScene(Viewport vp, Camera cam)
        {
            UpdateGraph();
            Clean();
            
            FindVisibleObjects(cam);
            _renderer.Render(vp, cam, _queue);
        }
        
        /// <summary>
        /// Updates the scene graph
        /// </summary>
        private void UpdateGraph()
        {
            _root.Update(true);
        }

        /// <summary>
        /// Cleans the render queue for the next frame
        /// </summary>
        private void Clean()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Finds all the visible objects in the scene
        /// </summary>
        /// <param name="cam">Current camera</param>
        private void FindVisibleObjects(Camera cam)
        {
            _root.FindVisibleObjects(cam, _queue, true);
        }

        /// <summary>
        /// Creates an Entity from a mesh
        /// </summary>
        /// <param name="modelName">Name of the model</param>
        /// <param name="name">Name of the Entity</param>
        /// <returns>Returns an Entity</returns>
        public Entity CreateEntity(string modelName, string name)
        {
            Entity ent = _entityFactory.Create(modelName, name);
            _movablesMap.Add(name, ent);

            return ent;
        }

        /// <summary>
        /// Creates a new node in this graph
        /// </summary>
        /// <returns>Returns a new node</returns>
        public SceneNode CreateNode()
        {
            SceneNode node = new SceneNode(this);
            _nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Destroys a node in this graph
        /// </summary>
        /// <param name="node">Node to destroy</param>
        public void DestroyNode(SceneNode node)
        {
            if(node.Scene != this) throw new ArgumentException("", "node");
            if(node == _root) throw new ArgumentException("", "node");

            Node parent = node.ParentNode;
            if (parent != null) parent.RemoveChild(node);

            node.DestroyAllChild();
            _nodes.Remove(node);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the camera manager instance
        /// </summary>
        public CameraManager CameraManager
        {
            get { return _camManager; }
        }

        /// <summary>
        /// Gets the root scene node
        /// </summary>
        public SceneNode Root
        {
            get { return _root; }
        }

        #endregion
    }
}
