namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents an element in a render queue
    /// </summary>
    internal sealed class RenderQueueElement
    {
        #region Fields

        /// <summary>
        /// Key of the element
        /// </summary>
        public ulong Key;

        /// <summary>
        /// Renderable object
        /// </summary>
        public IRenderable Renderable;

        #endregion
    }
}
