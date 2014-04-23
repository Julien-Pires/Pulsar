using System;

using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics.RenderingTechnique
{
    /// <summary>
    /// Describes a rendering technique
    /// A rendering technique must provide its own render queue
    /// </summary>
    internal interface IRenderingTechnique : IDisposable
    {
        #region Methods

        /// <summary>
        /// Render a scene for a specified viewport and camera
        /// </summary>
        /// <param name="viewport">Viewport in which the rendering is sent</param>
        /// <param name="camera">Camera representing the point of view</param>
        /// <param name="context">Frame context</param>
        void Render(Viewport viewport, Camera camera, FrameContext context);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of pass for this rendering technique
        /// </summary>
        int PassCount { get; }

        /// <summary>
        /// Gets the render queue
        /// </summary>
        IRenderQueue RenderQueue { get; }

        #endregion
    }
}
