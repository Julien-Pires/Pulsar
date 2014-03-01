using System;
using System.Collections.Generic;
using Pulsar.Assets;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Represents a basic scene graph
    /// </summary>
    public class BaseScene : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly Renderer _renderer;
        private readonly CameraManager _camManager;
        private readonly RenderQueue _queue = new RenderQueue();
        private readonly SceneNode _root;
        private readonly Dictionary<string, IMovable> _movablesMap = new Dictionary<string, IMovable>();
        private readonly List<SceneNode>  _nodes = new List<SceneNode>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BaseScene class
        /// </summary>
        /// <param name="name">Name of the scene tree</param>
        /// <param name="renderer">Renderer</param>
        /// <param name="assetEngine">AssetEngine</param>
        internal BaseScene(string name, Renderer renderer, AssetEngine assetEngine)
        {
            Name = name;
            _renderer = renderer;
            AssetEngine = assetEngine;
            _camManager = new CameraManager(this);
            _root = CreateNode();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            foreach (IMovable movable in _movablesMap.Values)
                movable.Dispose();

            _movablesMap.Clear();
            _isDisposed = true;
        }

        /// <summary>
        /// Draws the entire scene
        /// </summary>
        public void RenderScene(Viewport vp, Camera cam)
        {
            Clean();

            UpdateGraph();
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
        /// Creates an entity
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <param name="meshName">Name of the mesh</param>
        /// <param name="storage">Storage to load the mesh from</param>
        /// <param name="folder">Asset folder</param>
        /// <returns>Returns an entity instance</returns>
        public Entity CreateEntity(string name, string meshName, string storage, string folder)
        {
            AssetFolder assetFolder = AssetEngine[storage][folder];

            return CreateEntity(name, meshName, assetFolder);
        }

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <param name="meshName">Name of the mesh</param>
        /// <param name="assetFolder">Asset folder</param>
        /// <returns>Returns an entity instance</returns>
        public Entity CreateEntity(string name, string meshName, AssetFolder assetFolder)
        {
            Mesh mesh = assetFolder.Load<Mesh>(meshName);

            return CreateEntity(name, mesh);
        }

        /// <summary>
        /// Creates an entity from specified mesh
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <param name="mesh">Mesh</param>
        /// <returns>Returns an entity instance</returns>
        public Entity CreateEntity(string name, Mesh mesh)
        {
            Entity entity = new Entity(name, mesh, this);
            _movablesMap.Add(name, entity);

            return entity;
        }

        /// <summary>
        /// Destroys an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Returns true if the entity is destroyed otherwise false</returns>
        public bool DestroyEntity(Entity entity)
        {
            return DestroyEntity(entity.Name);
        }

        /// <summary>
        /// Destroys an entity with a specified name
        /// </summary>
        /// <param name="name">Name of the entity</param>
        /// <returns>Returns true if the entity is destroyed otherwise false</returns>
        public bool DestroyEntity(string name)
        {
            IMovable entity;
            if (!_movablesMap.TryGetValue(name, out entity))
                return false;

            _movablesMap.Remove(name);
            entity.Dispose();

            return true;
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
            if(node.Scene != this) 
                throw new ArgumentException("Already attached to another tree", "node");

            if(node == _root) 
                throw new ArgumentException("Cannot destroy root node", "node");

            Node parent = node.ParentNode;
            if (parent != null) 
                parent.RemoveChild(node);

            node.DestroyAllChild();
            _nodes.Remove(node);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the scene tree
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets an AssetEngine instance
        /// </summary>
        internal AssetEngine AssetEngine { get; private set; }

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
