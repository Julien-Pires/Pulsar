using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains data for the frame that is currently render
    /// </summary>
    public sealed class FrameContext
    {
        #region Methods

        /// <summary>
        /// Resets the context
        /// </summary>
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

        /// <summary>
        /// Gets the current material
        /// </summary>
        public Material Material { get; internal set; }

        /// <summary>
        /// Gets the current render state
        /// </summary>
        public RenderState RenderState { get; internal set; }

        /// <summary>
        /// Gets the current depthstencil state
        /// </summary>
        public DepthStencilState DepthStencilState { get; internal set; }

        /// <summary>
        /// Gets the current rasterizer state
        /// </summary>
        public RasterizerState RasterizerState { get; internal set; }

        /// <summary>
        /// Gets the blend state
        /// </summary>
        public BlendState BlendState { get; internal set; }

        #endregion
    }
}
