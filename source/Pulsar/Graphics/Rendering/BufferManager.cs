using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Enumerates type of buffer
    /// </summary>
    public enum BufferType
    {
        Static,
        StaticWriteOnly,
        Dynamic,
        DynamicWriteOnly
    }

    /// <summary>
    /// Used to create buffer object on a graphics device
    /// </summary>
    public sealed class BufferManager
    {
        #region Fields

        private GraphicsDeviceManager deviceManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BufferManager class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager used to create buffers</param>
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

        /// <summary>
        /// Create a new VertexBufferObject
        /// </summary>
        /// <param name="bufferType">Type of buffer to create</param>
        /// <param name="vertexType">Type of vertex stored in the buffer</param>
        /// <param name="vertexCount">Number of vertices stored in the buffer</param>
        /// <returns>Return a VertexBufferObject</returns>
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

        /// <summary>
        /// Create a new IndexBufferObject
        /// </summary>
        /// <param name="bufferType">Type of buffer to create</param>
        /// <param name="elementSize">Size of index stored in the buffer</param>
        /// <param name="indexCount">Number of indices in the buffer</param>
        /// <returns>Return an IndexBufferObject</returns>
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

        /// <summary>
        /// Create a new VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBuffer used by the VertexBufferObject</param>
        /// <returns>Return a VertexBufferObject</returns>
        internal VertexBufferObject CreateVertexBuffer(VertexBuffer buffer)
        {
            VertexBufferObject vbo = new VertexBufferObject(buffer);

            return vbo;
        }

        /// <summary>
        /// Create a new IndexBufferObject
        /// </summary>
        /// <param name="buffer">IndexBuffer used by the IndexBufferObject</param>
        /// <returns>Return an IndexBufferObject</returns>
        internal IndexBufferObject CreateIndexBuffer(IndexBuffer buffer)
        {
            IndexBufferObject ibo = new IndexBufferObject(buffer);

            return ibo;
        }

        #endregion
    }
}
