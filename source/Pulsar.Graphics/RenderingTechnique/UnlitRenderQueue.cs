using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.RenderingTechnique
{
    internal sealed partial class UnlitRendering
    {
        #region Nested

        public sealed class UnlitRenderQueue : IRenderQueue, IDisposable
        {
            #region Fields

            private bool _isDisposed;
            private readonly RenderQueueElementComparer _comparer = new RenderQueueElementComparer();
            private Pool<RenderQueueElement> _pool = new Pool<RenderQueueElement>(CreateElement, 64, ResetElement);
            private List<RenderQueueElement> _renderables = new List<RenderQueueElement>();

            #endregion

            #region Constructors

            internal UnlitRenderQueue()
            {
            }

            #endregion

            #region Static methods

            private static RenderQueueElement CreateElement()
            {
                return new RenderQueueElement();
            }

            private static void ResetElement(RenderQueueElement element)
            {
                element.Key = 0;
                element.Renderable = null;
            }

            #endregion

            #region Methods

            public void Dispose()
            {
                if(_isDisposed)
                    return;

                try
                {
                    _renderables.Clear();
                    _pool.Dispose();
                }
                finally
                {
                    _renderables = null;
                    _pool = null;

                    _isDisposed = true;
                }
            }

            internal void Reset()
            {
                _pool.Release(_renderables);
                _renderables.Clear();
            }

            public void AddRenderable(RenderQueueKey key, IRenderable renderable)
            {
                RenderQueueElement element = _pool.Get();
                element.Key = key;
                element.Renderable = renderable;
                _renderables.Add(element);
            }

            internal void Sort()
            {
                _renderables.Sort(_comparer);
            }

            #endregion

            #region Properties

            internal List<RenderQueueElement> Renderables
            {
                get { return _renderables; }
            }

            #endregion
        }

        #endregion
    }
}
