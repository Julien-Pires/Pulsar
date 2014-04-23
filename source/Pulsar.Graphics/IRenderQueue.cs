namespace Pulsar.Graphics
{
    /// <summary>
    /// Describes a render queue
    /// </summary>
    public interface IRenderQueue
    {
        #region Methods

        /// <summary>
        /// Adds a renderable object to the queue
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="renderable">Renderable object</param>
        void AddRenderable(RenderQueueKey key, IRenderable renderable);

        #endregion
    }
}
