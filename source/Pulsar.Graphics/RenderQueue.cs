using System.Collections.Generic;

namespace Pulsar.Graphics
{
    public sealed class RenderQueue
    {
        #region Fields

        private static readonly RenderQueueElementComparer Comparer = new RenderQueueElementComparer();

        private Pool<RenderQueueElement> _pool = new Pool<RenderQueueElement>(CreatePair, 64, ResetPair);
        private List<RenderQueueElement> _renderables = new List<RenderQueueElement>();
        
        #endregion

        #region Constructors

        internal RenderQueue()
        {
        }

        #endregion

        #region Static methods

        private static RenderQueueElement CreatePair()
        {
            return new RenderQueueElement();
        }

        private static void ResetPair(RenderQueueElement element)
        {
            element.Key = null;
            element.Renderable = null;
        }

        #endregion

        #region Methods

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
            _renderables.Sort(Comparer);
        }

        #endregion
    }
}
