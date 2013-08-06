using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Rendering.RenderingTechnique;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// GraphicsRenderer is used to do operations on graphic device
    /// </summary>
    public sealed class Renderer
    {
        #region Fields
        
        private GraphicsDevice _graphicDevice;
        private SpriteBatch _spriteBatch;
        private InstanceBatchManager instancingManager;
        private IRenderingTechnique _renderingTechnique;
        private readonly FrameDetail _frameDetail = new FrameDetail();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsRenderer class
        /// </summary>
        /// <param name="gDevice">Graphic device used by this instance</param>
        internal Renderer(GraphicsDevice gDevice)
        {
            this._graphicDevice = gDevice;
            this._spriteBatch = new SpriteBatch(_graphicDevice);
            this.instancingManager = new InstanceBatchManager(this);
            this._renderingTechnique = new SimpleRenderingTechnique(this);
        }

        #endregion

        #region Methods

        private void BeginRender(Viewport vp)
        {
            this.instancingManager.Reset();
            _frameDetail.Reset();

            if (vp.AlwaysClear) Clear(vp);
            _graphicDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void EndRender(Viewport vp)
        {
            vp.FrameDetail.Merge(_frameDetail);
        }

        /// <summary>
        /// Clear graphic device buffers(Vertex, Index, ...)
        /// </summary>
        private void UnsetBuffers()
        {
            this._graphicDevice.SetVertexBuffer(null);
            this._graphicDevice.Indices = null;
        }

        internal void Clear(Viewport vp)
        {
            this._graphicDevice.SetRenderTarget(vp.RenderTarget);
            this._graphicDevice.Clear(vp.ClearColor);
            this._graphicDevice.SetRenderTarget(null);
        }

        internal void SetRenderTarget(RenderTarget2D renderTarget)
        {
            this._graphicDevice.SetRenderTarget(renderTarget);
        }

        internal void UnsetRenderTarget()
        {
            _graphicDevice.SetRenderTarget(null);
        }

        internal void Render(Viewport vp, Camera cam, RenderQueue queue)
        {
            BeginRender(vp);

            _renderingTechnique.Render(vp, cam, queue);

            EndRender(vp);
        }

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

        internal void DrawGeometry(IRenderable geometry)
        {
            if (geometry.RenderInfo.useIndexes)
            {
                this.DrawIndexedGeometry(geometry);
            }
            else
            {
                this.DrawNonIndexedGeometry(geometry);
            }
        }

        /// <summary>
        /// Draw a IRenderable instance
        /// </summary>
        /// <param name="geometry">IRenderable instance</param>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        internal void DrawIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this._graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this._graphicDevice.Indices = renderInfo.indexData.Buffer;
            this._graphicDevice.DrawIndexedPrimitives(renderInfo.primitive, 0, 0, renderInfo.vertexCount,
                renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();

            _frameDetail.AddDrawCall((uint)renderInfo.VertexCount, (uint)renderInfo.PrimitiveCount, 1);
        }

        internal void DrawNonIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this._graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this._graphicDevice.DrawPrimitives(renderInfo.primitive, renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();

            _frameDetail.AddDrawCall((uint)renderInfo.VertexCount, (uint)renderInfo.PrimitiveCount, 1);
        }

        /// <summary>
        /// Draw a geometry batch
        /// </summary>
        /// <param name="batch">Geometry batch to draw</param>
        internal void DrawInstancedGeometry(InstanceBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo renderInfo = batch.RenderInfo;
            this._graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this._graphicDevice.Indices = renderInfo.indexData.Buffer;
            this._graphicDevice.DrawInstancedPrimitives(renderInfo.primitive, 0, 0, renderInfo.vertexCount, 
                renderInfo.startIndex, renderInfo.triangleCount, batch.InstanceCount);
            this.UnsetBuffers();

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
            get { return this._graphicDevice; }
        }

        /// <summary>
        /// Get the InstanceBatch manager
        /// </summary>
        internal InstanceBatchManager InstancingManager
        {
            get { return this.instancingManager; }
        }

        #endregion
    }
}
