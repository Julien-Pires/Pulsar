using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains data for the frame that is currently render
    /// </summary>
    public sealed class FrameContext
    {
        #region Properties

        /// <summary>
        /// Gets the elapsed time
        /// </summary>
        public float ElapsedTime { get; internal set; }

        /// <summary>
        /// Gets the current camera
        /// </summary>
        public Camera Camera { get; internal set; }

        /// <summary>
        /// Gets the current object that is drawn
        /// </summary>
        public IRenderable Renderable { get; internal set; }

        #endregion
    }
}
