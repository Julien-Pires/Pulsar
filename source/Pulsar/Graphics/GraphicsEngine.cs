using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Class used as an entry point for the Graphics system
    /// </summary>
    public sealed class GraphicsEngine : IDisposable
    {
        #region Fields

        private readonly Window _window;
        private readonly Renderer _renderer;
        private readonly BufferManager _bufferManager;
        private readonly Dictionary<string, SceneTree> _scenes = new Dictionary<string, SceneTree>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsEngine class
        /// </summary>
        /// <param name="services">GameServiceContainer used by the engine</param>
        public GraphicsEngine(IServiceProvider services)
        {
            if (services == null) throw new ArgumentNullException("services");

            GraphicsDeviceManager deviceService = services.GetService(typeof(IGraphicsDeviceManager)) 
                as GraphicsDeviceManager;
            if (deviceService == null) 
                throw new NullReferenceException("No Graphics device service found");

            _renderer = new Renderer(deviceService);
            _window = new Window(deviceService, _renderer);
            _bufferManager = new BufferManager(deviceService);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            _window.Dispose();
            _renderer.Dispose();
        }

        /// <summary>
        /// Renders the current frame
        /// </summary>
        /// <param name="time">Time since last update</param>
        public void Render(GameTime time)
        {
            _window.Render(time);
        }

        /// <summary>
        /// Creates a scene graph
        /// </summary>
        /// <param name="name">Name of the scene graph</param>
        /// <returns>Returns an instance of SceneGraph class</returns>
        public SceneTree CreateSceneGraph(string name)
        {
            SceneTree graph = new SceneTree(_renderer);
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
        public FrameStatistics FrameStatistics
        {
            get { return _window.FrameStatistics; }
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
