using System;
using System.Collections.Generic;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.RenderingTechnique
{
    /// <summary>
    /// Represents a basic rendering technique with no light no shadows
    /// </summary>
    internal sealed partial class UnlitRendering
    {
        #region Nested

        /// <summary>
        /// Describes a render queue
        /// </summary>
        public sealed class UnlitRenderQueue : IRenderQueue, IDisposable
        {
            #region Fields

            private bool _isDisposed;
            private readonly RenderQueueElementComparer _comparer = new RenderQueueElementComparer();
            private Dictionary<Shader, Shader> _usedShaders = new Dictionary<Shader, Shader>(16);
            private Pool<RenderQueueElement> _pool = new Pool<RenderQueueElement>(CreateElement, 64, ResetElement);
            private List<RenderQueueElement> _renderables = new List<RenderQueueElement>(64);

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of UnlitRenderQueue
            /// </summary>
            internal UnlitRenderQueue()
            {
            }

            #endregion

            #region Static methods

            /// <summary>
            /// Used to create RenderQueueElement instance
            /// </summary>
            /// <returns>Returns a new instance of RenderQueueElement</returns>
            private static RenderQueueElement CreateElement()
            {
                return new RenderQueueElement();
            }

            /// <summary>
            /// Used to reset a RenderQueueElement
            /// </summary>
            /// <param name="element">RenderQueueElement instance</param>
            private static void ResetElement(RenderQueueElement element)
            {
                element.Key = 0;
                element.Renderable = null;
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
                    _renderables.Clear();
                    _usedShaders.Clear();
                    _pool.Dispose();
                }
                finally
                {
                    _renderables = null;
                    _usedShaders = null;
                    _pool = null;

                    _isDisposed = true;
                }
            }

            /// <summary>
            /// Resets the render queue
            /// </summary>
            internal void Reset()
            {
                _pool.Release(_renderables);
                _renderables.Clear();
                _usedShaders.Clear();
            }

            /// <summary>
            /// Adds a renderable object to the render queue
            /// </summary>
            /// <param name="key">Key</param>
            /// <param name="renderable">Renderable object</param>
            public void AddRenderable(RenderQueueKey key, IRenderable renderable)
            {
                RenderQueueElement element = _pool.Get();
                element.Key = key;
                element.Renderable = renderable;
                _renderables.Add(element);

                Shader shader = renderable.Material.Technique.Shader;
                _usedShaders[shader] = shader;
            }

            /// <summary>
            /// Sorts the render queue
            /// </summary>
            internal void Sort()
            {
                _renderables.Sort(_comparer);
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the list of renderables object
            /// </summary>
            internal List<RenderQueueElement> Renderables
            {
                get { return _renderables; }
            }

            internal Dictionary<Shader, Shader> UsedShaders
            {
                get { return _usedShaders; }
            }

            #endregion
        }

        #endregion
    }
}
