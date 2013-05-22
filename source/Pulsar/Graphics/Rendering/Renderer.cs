using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering.RenderPass;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// GraphicsRenderer is used to do operations on graphic device
    /// </summary>
    internal sealed class Renderer
    {
        #region Fields
        
        private GraphicsDevice graphicDevice = null;
        private InstanceBatchManager instancingManager;
        private GBufferPass gBufferPass = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsRenderer class
        /// </summary>
        /// <param name="gDevice">Graphic device used by this instance</param>
        internal Renderer(GraphicsDevice gDevice)
        {
            this.graphicDevice = gDevice;
            this.instancingManager = new InstanceBatchManager(this);
            this.gBufferPass = new GBufferPass(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all buffers
        /// </summary>
        private void ClearBuffer()
        {
            this.graphicDevice.Clear((ClearOptions.Stencil | ClearOptions.DepthBuffer), Color.Black, 1.0f, 0);
        }

        /// <summary>
        /// Begin the draw operation
        /// </summary>
        private void BeginFrame()
        {
            this.instancingManager.Reset();
            this.graphicDevice.Clear(Color.Black);
        }

        /// <summary>
        /// End the draw operation
        /// </summary>
        private void EndFrame()
        {
        }

        /// <summary>
        /// Clear graphic device buffers(Vertex, Index, ...)
        /// </summary>
        private void UnsetBuffers()
        {
            this.graphicDevice.SetVertexBuffer(null);
            this.graphicDevice.Indices = null;
        }

        /// <summary>
        /// Perform the rendering pipeline
        /// </summary>
        /// <param name="queue">Render queue to draw</param>
        /// <param name="cam">Camera used as point of view for rendering</param>
        internal void Render(RenderQueue queue, Camera cam)
        {
            this.BeginFrame();

            this.gBufferPass.Render(queue, cam);

            this.EndFrame();
        }

        internal void RenderGeometry(IRenderable geometry)
        {
            if (geometry.RenderInfo.useIndexes)
            {
                this.RenderIndexedGeometry(geometry);
            }
            else
            {
                this.RenderNonIndexedGeometry(geometry);
            }
        }

        /// <summary>
        /// Draw a IRenderable instance
        /// </summary>
        /// <param name="geometry">IRenderable instance</param>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        internal void RenderIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this.graphicDevice.SetVertexBuffer(renderInfo.vBuffer, renderInfo.vertexOffset);
            this.graphicDevice.Indices = renderInfo.iBuffer;
            this.graphicDevice.DrawIndexedPrimitives(renderInfo.Primitive, 0, 0, renderInfo.vertexCount,
                renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();
        }

        internal void RenderNonIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this.graphicDevice.SetVertexBuffer(renderInfo.vBuffer, renderInfo.vertexOffset);
            this.graphicDevice.DrawPrimitives(renderInfo.Primitive, renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();
        }

        /// <summary>
        /// Draw a geometry batch
        /// </summary>
        /// <param name="batch">Geometry batch to draw</param>
        internal void RenderInstancedGeometry(InstanceBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo info = batch.RenderInfo;
            this.graphicDevice.SetVertexBuffers(
                new VertexBufferBinding(info.vBuffer, info.vertexOffset, 0),
                new VertexBufferBinding(batch.Buffer, 0, 1)
            );
            this.graphicDevice.Indices = info.iBuffer;
            this.graphicDevice.DrawInstancedPrimitives(info.Primitive, 0, 0, info.vertexCount, info.startIndex, info.triangleCount,
                batch.InstanceCount);

            this.UnsetBuffers();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the graphic device used by this renderer
        /// </summary>
        internal GraphicsDevice GraphicsDevice
        {
            get { return this.graphicDevice; }
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
