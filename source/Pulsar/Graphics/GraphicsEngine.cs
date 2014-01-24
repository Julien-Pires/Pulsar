using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Assets;
using Pulsar.Graphics.Asset;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents an entry point for the Graphics system
    /// </summary>
    public sealed class GraphicsEngine : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly AssetEngine _assetEngine;
        private Window _window;
        private Renderer _renderer;
        private readonly BufferManager _bufferManager;
        private PrefabFactory _prefabFactory;
        private Dictionary<string, SceneTree> _scenes = new Dictionary<string, SceneTree>();
        private readonly FrameStatistics _frameStats = new FrameStatistics();
        private readonly Stopwatch _watch = new Stopwatch();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GraphicsEngine class
        /// </summary>
        /// <param name="services">Services provider</param>
        public GraphicsEngine(IServiceProvider services)
        {
            if (services == null) 
                throw new ArgumentNullException("services");

            GraphicsDeviceManager deviceManager = services.GetService(typeof(IGraphicsDeviceManager)) 
                as GraphicsDeviceManager;
            if (deviceManager == null) 
                throw new NullReferenceException("Failed to find GraphicsDeviceManager");

            IAssetEngineService assetService = services.GetService(typeof(IAssetEngineService))
                as IAssetEngineService;
            if (assetService == null)
                throw new NullReferenceException("Failed to find AssetEngine service");

            _deviceManager = deviceManager;
            _bufferManager = new BufferManager(_deviceManager);
            _assetEngine = assetService.AssetEngine;

            InitializeStorage();
            InitializeResources();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes resources
        /// </summary>
        private void InitializeResources()
        {
            _renderer = new Renderer(_deviceManager, _assetEngine);
            _window = new Window(_deviceManager, _renderer);
            _prefabFactory = new PrefabFactory(_assetEngine);
        }

        /// <summary>
        /// Initializes storage and loaders
        /// </summary>
        private void InitializeStorage()
        {
            Storage storage = _assetEngine.CreateStorage(GraphicsConstant.Storage);
            storage.AddFolder(GraphicsConstant.TextureFolder, GraphicsConstant.TextureFolderName);
            storage.AddFolder(GraphicsConstant.MaterialFolder, GraphicsConstant.MaterialFolderName);
            storage.AddFolder(GraphicsConstant.ShaderFolder, GraphicsConstant.ShaderFolderName);
            storage.AddFolder(GraphicsConstant.MeshFolder, GraphicsConstant.MeshFolderName);

            _assetEngine.AddLoader(new TextureLoader(_deviceManager));
            _assetEngine.AddLoader(new ShaderLoader());
            _assetEngine.AddLoader(new MaterialLoader());
            _assetEngine.AddLoader(new MeshLoader(_deviceManager, _bufferManager));
        }

        /// <summary>
        /// Releases storage and loaders
        /// </summary>
        private void ReleaseStorage()
        {
            _assetEngine.RemoveLoader(ShaderLoader.LoaderName);
            _assetEngine.RemoveLoader(MeshLoader.LoaderName);
            _assetEngine.RemoveLoader(MaterialLoader.LoaderName);
            _assetEngine.RemoveLoader(TextureLoader.LoaderName);
            
            _assetEngine[GraphicsConstant.Storage].Dispose();
        }

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                foreach (SceneTree graph in _scenes.Values)
                    graph.Dispose();

                _window.Dispose();
                _renderer.Dispose();
                ReleaseStorage();
            }
            finally
            {
                _scenes.Clear();
                _scenes = null;
                _window = null;
                _renderer = null;

                _assetEngine.DestroyStorage(GraphicsConstant.Storage);
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Renders the current frame
        /// </summary>
        /// <param name="time">Time since last update</param>
        public void Render(GameTime time)
        {
            _watch.Reset();
            _watch.Start();
            _window.Render(time);
            _watch.Stop();

            FrameDetail currentFrame = _frameStats.CurrentFrame;
            currentFrame.Elapsed = _watch.Elapsed.TotalMilliseconds;
            currentFrame.Merge(_window.FrameDetail);

            _frameStats.SaveCurrentFrame();
            _frameStats.Framecount++;
            _frameStats.ComputeFramerate(time);
        }

        /// <summary>
        /// Creates a scene graph
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph class</returns>
        public SceneTree CreateSceneGraph(string name)
        {
            SceneTree graph = new SceneTree(name, _renderer, _assetEngine);
            _scenes.Add(name, graph);

            return graph;
        }

        /// <summary>
        /// Removes a scene graph
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns true if the scene graph is removed otherwise false</returns>
        public bool RemoveSceneGraph(string name)
        {
            SceneTree graph;
            if (!_scenes.TryGetValue(name, out graph))
                return false;

            graph.Dispose();

            return _scenes.Remove(name);
        }

        /// <summary>
        /// Gets a scene graph by its name
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph</returns>
        public SceneTree GetScene(string name)
        {
            return !_scenes.ContainsKey(name) ? null : _scenes[name];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets statistics about drawn frames
        /// </summary>
        public FrameStatistics Statistics
        {
            get { return _frameStats; }
        }

        /// <summary>
        /// Gets a prefab factory
        /// </summary>
        public PrefabFactory PrefabFactory
        {
            get { return _prefabFactory; }
        }

        /// <summary>
        /// Gets the window
        /// </summary>
        public Window Window
        {
            get { return _window; }
        }

        /// <summary>
        /// Gets the buffer manager
        /// </summary>
        public BufferManager BufferManager
        {
            get { return _bufferManager; }
        }

        /// <summary>
        /// Gets the renderer
        /// </summary>
        internal Renderer Renderer
        {
            get { return _renderer; }
        }

        #endregion
    }
}