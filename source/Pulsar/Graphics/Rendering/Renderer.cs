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
        
        private GraphicsDevice graphicDevice;
        private InstanceBatchManager instancingManager;
        private GBufferPass gBufferPass;
        private FrameInfo frameInfo;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the GraphicsRenderer class
        /// </summary>
        /// <param name="gDevice">Graphic device used by this instance</param>
        internal Renderer(GraphicsDevice gDevice, FrameInfo frameInfo)
        {
            this.graphicDevice = gDevice;
            this.frameInfo = frameInfo;
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
            this.frameInfo.PrepareNewRendering();
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
            this.frameInfo.Framecount++;
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
            this.graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this.graphicDevice.Indices = renderInfo.indexData.Buffer;
            this.graphicDevice.DrawIndexedPrimitives(renderInfo.primitive, 0, 0, renderInfo.vertexCount,
                renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();

            this.frameInfo.PrimitiveCount += renderInfo.triangleCount;
            this.frameInfo.VertexCount += renderInfo.vertexCount;
            this.frameInfo.SubMeshCount++;
            this.frameInfo.DrawCall++;
        }

        internal void RenderNonIndexedGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this.graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this.graphicDevice.DrawPrimitives(renderInfo.primitive, renderInfo.startIndex, renderInfo.triangleCount);
            this.UnsetBuffers();

            this.frameInfo.PrimitiveCount += renderInfo.triangleCount;
            this.frameInfo.VertexCount += renderInfo.vertexCount;
            this.frameInfo.SubMeshCount++;
            this.frameInfo.DrawCall++;
        }

        /// <summary>
        /// Draw a geometry batch
        /// </summary>
        /// <param name="batch">Geometry batch to draw</param>
        internal void RenderInstancedGeometry(InstanceBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo renderInfo = batch.RenderInfo;
            this.graphicDevice.SetVertexBuffers(renderInfo.vertexData.VertexBindings);
            this.graphicDevice.Indices = renderInfo.indexData.Buffer;
            this.graphicDevice.DrawInstancedPrimitives(renderInfo.primitive, 0, 0, renderInfo.vertexCount, 
                renderInfo.startIndex, renderInfo.triangleCount, batch.InstanceCount);
            this.UnsetBuffers();

            this.frameInfo.PrimitiveCount += renderInfo.triangleCount;
            this.frameInfo.VertexCount += renderInfo.vertexCount;
            this.frameInfo.SubMeshCount++;
            this.frameInfo.DrawCall++;
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
