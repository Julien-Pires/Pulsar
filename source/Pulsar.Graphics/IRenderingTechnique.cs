namespace Pulsar.Graphics
{
    /// <summary>
    /// Interfaces for rendering technique used by Renderer class to render a scene graph
    /// </summary>
    public interface IRenderingTechnique
    {
        #region Methods

        /// <summary>
        /// Render a scene graph into a viewport
        /// </summary>
        /// <param name="vp">Viewport in which the rendering is sent</param>
        /// <param name="cam">Camera representing the point of view</param>
        /// <param name="queue">Queue of objects to render</param>
        void Render(Viewport vp, Camera cam, RenderQueue queue);

        #endregion
    }
}
