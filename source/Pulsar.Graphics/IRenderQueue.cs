namespace Pulsar.Graphics
{
    public interface IRenderQueue
    {
        #region Methods

        void AddRenderable(RenderQueueKey key, IRenderable renderable);

        #endregion
    }
}
