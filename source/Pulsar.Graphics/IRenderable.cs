using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Defines a renderable object
    /// </summary>
    public interface IRenderable
    {
        #region Properties

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the render queue key
        /// </summary>
        RenderQueueKey Key { get; }

        /// <summary>
        /// Gets the render queue group
        /// </summary>
        byte RenderQueueGroup { get; }

        /// <summary>
        /// Gets the full transform matrix
        /// </summary>
        Matrix Transform { get; }

        /// <summary>
        /// Gets the rendering info
        /// </summary>
        RenderingInfo RenderInfo { get; }

        /// <summary>
        /// Gets the material associated to this renderable object
        /// </summary>
        Material Material { get; }

        #endregion
    }
}
