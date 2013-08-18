using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Rendering.RenderingTechnique;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// GraphicsRenderer is used to do operations on graphic device
    /// </summary>
    public sealed class Renderer : IDisposable
    {
        #region Fields
        
        private readonly GraphicsDevice _graphicDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly InstanceBatchManager _instancingManager;
        private readonly IRenderingTechnique _renderingTechnique;
        private readonly FrameDetail _frameDetail = new FrameDetail();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsRenderer class
        /// </summary>
        /// <param name="gDevice">Graphic device used by this instance</param>
        internal Renderer(GraphicsDevice gDevice)
        {
            _graphicDevice = gDevice;
            _spriteBatch = new SpriteBatch(_graphicDevice);
            _instancingManager = new InstanceBatchManager(this);
            _renderingTechnique = new SimpleRenderingTechnique(this);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _spriteBatch.Dispose();
        }

        /// <summary>
        /// Called before the rendering
        /// </summary>
        /// <param name="vp">Viewport in which the rendering result will be sent</param>
        private void BeginRender(Viewport vp)
        {
            _instancingManager.Reset();
            _frameDetail.Reset();

            if (vp.AlwaysClear) Clear(vp);
            _graphicDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Called after the rendering
        /// </summary>
        /// <param name="vp">Viewport in which the rendering result has been sent</param>
        private void EndRender(Viewport vp)
        {
            vp.FrameDetail.Merge(_frameDetail);
        }

        /// <summary>
        /// Clear graphic device buffers(Vertex, Index, ...)
        /// </summary>
        private void UnsetBuffers()
        {
            _graphicDevice.SetVertexBuffer(null);
            _graphicDevice.Indices = null;
        }

        /// <summary>
        /// Clear a viewport
        /// </summary>
        /// <param name="vp">Viewport to clear</param>
        internal void Clear(Viewport vp)
        {
            _graphicDevice.SetRenderTarget(vp.RenderTarget);
            _graphicDevice.Clear(vp.ClearColor);
            _graphicDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Set a render target on the graphic device
        /// </summary>
        /// <param name="renderTarget">Render target to set</param>
        internal void SetRenderTarget(RenderTarget2D renderTarget)
        {
            _graphicDevice.SetRenderTarget(renderTarget);
        }

        /// <summary>
        /// Remove all render target from the graphic device
        /// </summary>
        internal void UnsetRenderTarget()
        {
            _graphicDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Render a scene graph into a viewport
        /// </summary>
        /// <param name="vp">Viewport in which the rendering is sent</param>
        /// <param name="cam">Camera representing the point of view</param>
        /// <param name="queue">Queue of objects to render</param>
        internal void Render(Viewport vp, Camera cam, RenderQueue queue)
        {
            BeginRender(vp);

            _renderingTechnique.Render(vp, cam, queue);

            EndRender(vp);
        }

        /// <summary>
        /// Draw all viewports of a RenderTarget into its own render target
        /// </summary>
        /// <param name="renderTarget">RenderTarget to render</param>
        internal void RenderToTarget(RenderTarget renderTarget)
        {
            _graphicDevice.SetRenderTarget(renderTarget.Target);
            if(renderTarget.AlwaysClear) _graphicDevice.Clear(renderTarget.ClearColor);

            ViewportCollection viewports = renderTarget.Viewports;
            if (viewports.Count > 0)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise);
                for (int i = 0; i < viewports.Count; i++)
                {
                    Viewport vp = viewports[i];
                    Rectangle rect = new Rectangle(vp.RealLeft, vp.RealTop, vp.RealWidth, vp.RealHeight);
                    _spriteBatch.Draw(vp.RenderTarget, rect, Color.White);
                }
                _spriteBatch.End();
            }

            _graphicDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draw a texture on the entire screen
        /// </summary>
        /// <param name="texture">Texture to render</param>
        internal void DrawFullQuad(Texture2D texture)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            PresentationParameters presentation = _graphicDevice.PresentationParameters;
            Rectangle rect = new Rectangle(0, 0, presentation.BackBufferWidth, presentation.BackBufferHeight);
            _spriteBatch.Draw(texture, rect, Color.White);
            _spriteBatch.End();

            _frameDetail.AddDrawCall(4, 2, 1);
        }

        /// <summary>
        /// Render a geometric shape
        /// </summary>
        /// <param name="geometry">Geometric object</param>
        internal void DrawGeometry(IRenderable geometry)
        {
            if (geometry.RenderInfo.UseIndexes)
            {
                DrawIndexedGeometry(geometry);
            }
            else
            {
                DrawNonIndexedGeometry(geometry);
            }
        }

        /// <summary>
        /// Draw a geometric shape which use index buffer
        /// </summary>
        /// <param name="geometry">Geometric shape</param>
        internal void DrawIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            _graphicDevice.SetVertexBuffers(renderInfo.VertexData.VertexBindings);
            _graphicDevice.Indices = renderInfo.IndexData.Buffer;
            _graphicDevice.DrawIndexedPrimitives(renderInfo.PrimitiveType, 0, 0, renderInfo.VertexCount,
                renderInfo.StartIndex, renderInfo.PrimitiveCount);
            UnsetBuffers();

            _frameDetail.AddDrawCall((uint)renderInfo.VertexCount, (uint)renderInfo.PrimitiveCount, 1);
        }

        /// <summary>
        /// Draw a geometric shape which doesn't use index buffer
        /// </summary>
        /// <param name="geometry">Geometric shape</param>
        internal void DrawNonIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            _graphicDevice.SetVertexBuffers(renderInfo.VertexData.VertexBindings);
            _graphicDevice.DrawPrimitives(renderInfo.PrimitiveType, renderInfo.StartIndex, renderInfo.PrimitiveCount);
            UnsetBuffers();

            _frameDetail.AddDrawCall((uint)renderInfo.VertexCount, (uint)renderInfo.PrimitiveCount, 1);
        }

        /// <summary>
        /// Draw a geometry batch
        /// </summary>
        /// <param name="batch">Bztch of geometric shapes</param>
        internal void DrawInstancedGeometry(InstanceBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo renderInfo = batch.RenderInfo;
            _graphicDevice.SetVertexBuffers(renderInfo.VertexData.VertexBindings);
            _graphicDevice.Indices = renderInfo.IndexData.Buffer;
            _graphicDevice.DrawInstancedPrimitives(renderInfo.PrimitiveType, 0, 0, renderInfo.VertexCount,
                renderInfo.StartIndex, renderInfo.PrimitiveCount, batch.InstanceCount);
            UnsetBuffers();

            int instanceCount = batch.InstanceCount;
            _frameDetail.AddDrawCall((uint)(renderInfo.VertexCount * instanceCount), (uint)(renderInfo.PrimitiveCount * instanceCount), 
                (uint)instanceCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the graphic device used by this renderer
        /// </summary>
        internal GraphicsDevice GraphicsDevice
        {
            get { return _graphicDevice; }
        }

        /// <summary>
        /// Get the InstanceBatch manager
        /// </summary>
        internal InstanceBatchManager InstancingManager
        {
            get { return _instancingManager; }
        }

        #endregion
    }
}
