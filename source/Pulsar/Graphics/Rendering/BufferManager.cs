using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Enumerates type of buffer
    /// </summary>
    public enum BufferType
    {
        /// <summary>
        /// A buffer used for static vertices
        /// </summary>
        Static,
        /// <summary>
        /// A buffer used for static vertices in write only mode
        /// </summary>
        StaticWriteOnly,
        /// <summary>
        /// A buffer used when vertices are often replaced
        /// </summary>
        Dynamic,
        /// <summary>
        /// A buffer used when vertices are often replaced and is used only in write mode
        /// </summary>
        DynamicWriteOnly
    }

    /// <summary>
    /// Used to create buffer object on a graphics device
    /// </summary>
    public sealed class BufferManager
    {
        #region Fields

        private readonly GraphicsDeviceManager _deviceManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BufferManager class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager used to create buffers</param>
        internal BufferManager(GraphicsDeviceManager deviceManager)
        {
            if (deviceManager == null) throw new ArgumentNullException("deviceManager");
            _deviceManager = deviceManager;
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
                case BufferType.Static: buffer = new VertexBuffer(_deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly: buffer = new VertexBuffer(_deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic: buffer = new DynamicVertexBuffer(_deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly: buffer = new DynamicVertexBuffer(_deviceManager.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null) throw new Exception("Failed to create buffer, wrong buffer type provided");

            return new VertexBufferObject(buffer);
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
                case BufferType.Static: buffer = new IndexBuffer(_deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly: buffer = new IndexBuffer(_deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic: buffer = new DynamicIndexBuffer(_deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly: buffer = new DynamicIndexBuffer(_deviceManager.GraphicsDevice, elementSize,
                    indexCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null) throw new Exception("Failed to create buffer, wrong buffer type provided");

            return new IndexBufferObject(buffer);
        }

        /// <summary>
        /// Create a new VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBuffer used by the VertexBufferObject</param>
        /// <returns>Return a VertexBufferObject</returns>
        internal VertexBufferObject CreateVertexBuffer(VertexBuffer buffer)
        {
            return new VertexBufferObject(buffer);
        }

        /// <summary>
        /// Create a new IndexBufferObject
        /// </summary>
        /// <param name="buffer">IndexBuffer used by the IndexBufferObject</param>
        /// <returns>Return an IndexBufferObject</returns>
        internal IndexBufferObject CreateIndexBuffer(IndexBuffer buffer)
        {
            return new IndexBufferObject(buffer);
        }

        #endregion
    }
}
