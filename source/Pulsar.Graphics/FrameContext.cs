using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains data for the frame that is currently render
    /// </summary>
    public sealed class FrameContext
    {
        #region Methods

        internal void Reset()
        {
            ElapsedTime = 0.0f;
            Camera = null;
            Renderable = null;
            Material = null;
            RenderState = null;
            DepthStencilState = null;
            RasterizerState = null;
            BlendState = null;
        }

        #endregion

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

        public Material Material { get; internal set; }

        public RenderState RenderState { get; internal set; }

        public DepthStencilState DepthStencilState { get; internal set; }

        public RasterizerState RasterizerState { get; internal set; }

        public BlendState BlendState { get; internal set; }

        #endregion
    }
}
