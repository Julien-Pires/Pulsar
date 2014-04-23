using Microsoft.Xna.Framework;

using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Defines a renderable object
    /// </summary>
    public interface IRenderable
    {
        #region Methods

        /// <summary>
        /// Computes the squared distance between the renderable and a camera
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <returns>Returns the squared distance</returns>
        float GetViewDepth(Camera camera);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of this renderable
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets a boolean indicating if this instance use instancing
        /// </summary>
        bool UseInstancing { get; set; }

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
