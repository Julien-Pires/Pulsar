using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public enum BufferType
    {
        Static,
        StaticWriteOnly,
        Dynamic,
        DynamicWriteOnly
    }

    public sealed class BufferManager
    {
        #region Fields

        private GraphicsDeviceManager deviceManager;

        #endregion

        #region Constructors

        internal BufferManager(GraphicsDeviceManager deviceManager)
        {
            if (deviceManager == null)
            {
                throw new ArgumentNullException("GraphicsDeviceManager cannot be null");
            }
            this.deviceManager = deviceManager;
        }

        #endregion

        #region Methods

        public VertexBufferObject CreateVertexBuffer(BufferType bufferType, Type vertexType, int vertexCount)
        {
            VertexBuffer buffer = null;
            switch (bufferType)
            {
                case BufferType.Static: buffer = new VertexBuffer(this.deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly: buffer = new VertexBuffer(this.deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic: buffer = new DynamicVertexBuffer(this.deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly: buffer = new DynamicVertexBuffer(this.deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null)
            {
                throw new Exception("Failed to create buffer, wrong buffer type provided");
            }
            
            VertexBufferObject vbo = new VertexBufferObject(buffer);

            return vbo;
        }

        public IndexBufferObject CreateIndexBuffer(BufferType bufferType, IndexElementSize elementSize, int indexCount)
        {
            IndexBuffer buffer = null;
            switch (bufferType)
            {
                case BufferType.Static: buffer = new IndexBuffer(this.deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly: buffer = new IndexBuffer(this.deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic: buffer = new DynamicIndexBuffer(this.deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly: buffer = new DynamicIndexBuffer(this.deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null)
            {
                throw new Exception("Failed to create buffer, wrong buffer type provided");
            }

            IndexBufferObject ibo = new IndexBufferObject(buffer);

            return ibo;
        }

        internal VertexBufferObject CreateVertexBuffer(VertexBuffer buffer)
        {
            VertexBufferObject vbo = new VertexBufferObject(buffer);

            return vbo;
        }

        internal IndexBufferObject CreateIndexBuffer(IndexBuffer buffer)
        {
            IndexBufferObject ibo = new IndexBufferObject(buffer);

            return ibo;
        }

        #endregion
    }
}
