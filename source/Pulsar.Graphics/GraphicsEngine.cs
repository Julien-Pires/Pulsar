using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Assets;
using Pulsar.Graphics.Fx;
using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents an entry point for the Graphics engine
    /// </summary>
    public sealed class GraphicsEngine : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly GraphicsDeviceManager _deviceManager;
        private AssetEngine _assetEngine;
        private GraphicsStorage _graphicsStorage;
        private Window _window;
        private Renderer _renderer;
        private readonly BufferManager _bufferManager;
        private readonly BuiltInShaderManager _shaderManager;
        private readonly PrefabFactory _prefabFactory;
        private Dictionary<string, SceneGraph> _scenes = new Dictionary<string, SceneGraph>();
        private readonly FrameStatistics _frameStats = new FrameStatistics();
        private FrameContext _context = new FrameContext();
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
            _assetEngine = assetService.AssetEngine;
            _graphicsStorage = new GraphicsStorage(_assetEngine);

            _bufferManager = new BufferManager(_deviceManager);
            _shaderManager = new BuiltInShaderManager(_graphicsStorage);
            _renderer = new Renderer(_deviceManager);
            _window = new Window(_deviceManager, _renderer);
            _prefabFactory = new PrefabFactory(_assetEngine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                foreach (SceneGraph graph in _scenes.Values)
                    graph.Dispose();

                _scenes.Clear();
                _window.Dispose();
                _renderer.Dispose();
                _graphicsStorage.Dispose();
            }
            finally
            {
                _graphicsStorage = null;
                _assetEngine = null;
                _scenes = null;
                _window = null;
                _renderer = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal void Initialize()
        {
            _assetEngine.InitializeLoaders(GraphicsConstant.LoadersCategory);
        }

        /// <summary>
        /// Renders the current frame
        /// </summary>
        /// <param name="time">Time since last update</param>
        public void Render(GameTime time)
        {
            _watch.Reset();
            _context.Reset();

            _watch.Start();
            _window.Render(time, _context);
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
        public SceneGraph CreateSceneGraph(string name)
        {
            SceneGraph graph = new SceneGraph(name, _renderer, _assetEngine);
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
            SceneGraph graph;
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
        public SceneGraph GetScene(string name)
        {
            return !_scenes.ContainsKey(name) ? null : _scenes[name];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graphics device manager used by the engine
        /// </summary>
        public GraphicsDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

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

        /// <summary>
        /// Gets the manager for built-in shaders
        /// </summary>
        internal BuiltInShaderManager ShaderManager
        {
            get { return _shaderManager; }
        }

        #endregion
    }
}