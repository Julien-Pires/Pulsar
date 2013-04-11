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

        private static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
        );

        private List<DynamicVertexBuffer> instancesBuffer = new List<DynamicVertexBuffer>();
        private GraphicsDevice graphicDevice = null;
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
        /// Clear all instance buffer
        /// </summary>
        private void ClearInstanceBuffer()
        {
            for (int i = 0; i < this.instancesBuffer.Count; i++)
            {
                this.instancesBuffer[i].Dispose();
            }

            this.instancesBuffer.Clear();
        }

        /// <summary>
        /// Create a dynamic vertex buffer for a geometry batch
        /// </summary>
        /// <param name="transforms">Array of instance transform matrix</param>
        /// <param name="instanceCount">Total of instance</param>
        private DynamicVertexBuffer CreateInstanceBuffer(Matrix[] transforms, int instanceCount)
        {
            DynamicVertexBuffer dynamicBuffer = new DynamicVertexBuffer(this.graphicDevice, Renderer.instanceVertexDeclaration,
                transforms.Length, BufferUsage.WriteOnly);

            dynamicBuffer.SetData(transforms, 0, instanceCount, SetDataOptions.Discard);
            this.instancesBuffer.Add(dynamicBuffer);

            return dynamicBuffer;
        }

        /// <summary>
        /// Begin the draw operation
        /// </summary>
        private void BeginFrame()
        {
            this.ClearInstanceBuffer();
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

        /// <summary>
        /// Draw a IRenderable instance
        /// </summary>
        /// <param name="geometry">IRenderable instance</param>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        internal void RenderGeometry(IRenderable geometry)
        {
            RenderingInfo renderInfo = geometry.RenderInfo;
            this.graphicDevice.SetVertexBuffer(renderInfo.VBuffer, renderInfo.VertexOffset);
            this.graphicDevice.Indices = renderInfo.IBuffer;
            this.graphicDevice.DrawIndexedPrimitives(renderInfo.Primitive, 0, 0, renderInfo.VertexCount,
                renderInfo.StartIndex, renderInfo.TriangleCount);

            this.UnsetBuffers();
        }

        /// <summary>
        /// Draw a geometry batch
        /// </summary>
        /// <param name="batch">Geometry batch to draw</param>
        internal void RenderInstancedGeometry(GeometryBatch batch)
        {
            if (batch.InstanceCount == 0)
                return;

            RenderingInfo info = batch.RenderInfo;
            DynamicVertexBuffer dynamicBuffer = this.CreateInstanceBuffer(batch.InstanceTransforms, batch.InstanceCount);
            this.graphicDevice.SetVertexBuffers(
                new VertexBufferBinding(info.VBuffer, info.VertexOffset, 0),
                new VertexBufferBinding(dynamicBuffer, 0, 1)
            );
            this.graphicDevice.Indices = info.IBuffer;
            this.graphicDevice.DrawInstancedPrimitives(info.Primitive, 0, 0, info.VertexCount, info.StartIndex, info.TriangleCount,
                batch.InstanceCount);

            this.UnsetBuffers();
        }

        #endregion
    }
}
