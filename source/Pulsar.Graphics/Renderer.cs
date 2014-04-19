using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Performs all rendering operations on a graphics device
    /// </summary>
    public sealed class Renderer : IDisposable
    {
        #region Fields

        internal readonly FrameDetail FrameDetail = new FrameDetail();

        private bool _disposed;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private GraphicsDevice _graphicDevice;
        private SpriteBatch _spriteBatch;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Renderer class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager</param>
        internal Renderer(GraphicsDeviceManager deviceManager)
        {
            _graphicsDeviceManager = deviceManager;
            _graphicDevice = deviceManager.GraphicsDevice;
            deviceManager.DeviceCreated += GraphicsDeviceCreated;

            _spriteBatch = new SpriteBatch(_graphicDevice);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all ressources
        /// </summary>
        public void Dispose()
        {
            if(_disposed) return;

            _graphicsDeviceManager.DeviceCreated -= GraphicsDeviceCreated;
            if(_spriteBatch != null) 
                _spriteBatch.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Called each time graphics device is created
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void GraphicsDeviceCreated(object sender, EventArgs e)
        {
            _graphicDevice = _graphicsDeviceManager.GraphicsDevice;

            if(_spriteBatch != null) 
                _spriteBatch.Dispose();
            _spriteBatch = new SpriteBatch(_graphicDevice);
        }

        /// <summary>
        /// Resets current graphic device vertex and index buffers
        /// </summary>
        private void UnsetBuffers()
        {
            _graphicDevice.SetVertexBuffer(null);
            _graphicDevice.Indices = null;
        }

        /// <summary>
        /// Clears the back buffer
        /// </summary>
        /// <param name="clearColor">Color used to clear</param>
        internal void Clear(Color clearColor)
        {
            _graphicDevice.Clear(clearColor);
        }

        /// <summary>
        /// Sets a render target on the graphic device
        /// </summary>
        /// <param name="renderTarget">Render target to set</param>
        internal void SetRenderTarget(RenderTarget2D renderTarget)
        {
            _graphicDevice.SetRenderTarget(renderTarget);
        }

        /// <summary>
        /// Removes all render target from the graphic device
        /// </summary>
        internal void UnsetRenderTarget()
        {
            _graphicDevice.SetRenderTarget(null);
        }

        internal void SetDepthStencilState(DepthStencilState state)
        {
            _graphicDevice.DepthStencilState = state;
        }

        internal void SetRasterizerState(RasterizerState state)
        {
            _graphicDevice.RasterizerState = state;
        }

        internal void SetBlendState(BlendState state)
        {
            _graphicDevice.BlendState = state;
        }

        /// <summary>
        /// Draws all viewports of a RenderTarget into its own render target
        /// </summary>
        /// <param name="renderTarget">RenderTarget to render</param>
        internal void RenderToTarget(RenderTarget renderTarget)
        {
            _graphicDevice.SetRenderTarget(renderTarget.Target);
            if(renderTarget.AlwaysClear) 
                _graphicDevice.Clear(renderTarget.ClearColor);

            List<Viewport> viewports = renderTarget.Viewports;
            if (viewports.Count > 0)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
                    DepthStencilState.None, RasterizerState.CullCounterClockwise);
                for (int i = 0; i < viewports.Count; i++)
                {
                    Viewport viewport = viewports[i];
                    if(!viewport.IsRendered)
                        continue;

                    Rectangle rect = new Rectangle(viewport.PixelLeft, viewport.PixelTop, viewport.PixelWidth,
                        viewport.PixelHeight);
                    _spriteBatch.Draw(viewport.Target, rect, Color.White);
                }
                _spriteBatch.End();
            }

            _graphicDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws a texture on the entire screen
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

            FrameDetail.AddDrawCall(4, 2, 1);
        }

        /// <summary>
        /// Renders one 3D geometric shape
        /// </summary>
        /// <param name="geometry"></param>
        internal void Draw(IRenderable geometry)
        {
            RenderingInfo renderingInfo = geometry.RenderInfo;

            _graphicDevice.SetVertexBuffers(renderingInfo.VertexData.VertexBindings);
            if (renderingInfo.UseIndexes)
            {
                IndexData indexData = renderingInfo.IndexData;
                _graphicDevice.Indices = indexData.HardwareBuffer;
                _graphicDevice.DrawIndexedPrimitives(renderingInfo.PrimitiveType, 0, 0,
                    renderingInfo.VertexCount, indexData.StartIndex, renderingInfo.PrimitiveCount);
            }
            else
                _graphicDevice.DrawPrimitives(renderingInfo.PrimitiveType, 0, renderingInfo.PrimitiveCount);

            UnsetBuffers();

            FrameDetail.AddDrawCall((uint)renderingInfo.VertexCount, (uint)renderingInfo.PrimitiveCount, 1);
        }

        /*/// <summary>
        /// Draws a geometry batch
        /// </summary>
        /// <param name="batch">Bztch of geometric shapes</param>
        internal void DrawInstancedGeometry(InstanceBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo renderInfo = batch.RenderInfo;
            _graphicDevice.SetVertexBuffers(renderInfo.VertexData.VertexBindings);
            IndexData indexData = renderInfo.IndexData;
            _graphicDevice.Indices = indexData.HardwareBuffer;
            _graphicDevice.DrawInstancedPrimitives(renderInfo.PrimitiveType, 0, 0, renderInfo.VertexCount,
                indexData.StartIndex, renderInfo.PrimitiveCount, batch.InstanceCount);
            UnsetBuffers();

            int instanceCount = batch.InstanceCount;
            FrameDetail.AddDrawCall((uint)(renderInfo.VertexCount * instanceCount), (uint)(renderInfo.PrimitiveCount * instanceCount), 
                (uint)instanceCount);
        }*/

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graphic device used by this renderer
        /// </summary>
        internal GraphicsDevice GraphicsDevice
        {
            get { return _graphicDevice; }
        }

        #endregion
    }
}
