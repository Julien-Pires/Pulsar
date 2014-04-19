using System;

using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.RenderingTechnique
{
    /// <summary>
    /// Interfaces for rendering technique used by Renderer class to render a scene graph
    /// </summary>
    internal interface IRenderingTechnique : IDisposable
    {
        #region Methods

        /// <summary>
        /// Render a scene graph into a viewport
        /// </summary>
        /// <param name="viewport">Viewport in which the rendering is sent</param>
        /// <param name="camera">Camera representing the point of view</param>
        /// <param name="queue">Queue of objects to render</param>
        void Render(Viewport viewport, Camera camera, FrameContext context);

        #endregion

        #region Properties

        int PassCount { get; }

        IRenderQueue RenderQueue { get; }

        #endregion
    }
}
