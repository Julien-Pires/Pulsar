using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
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
    /// Creates buffer object on a graphics device
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

        #region Static methods

        /// <summary>
        /// Resolves buffer type from a vertex buffer instance
        /// </summary>
        /// <param name="buffer">Vertex buffer</param>
        /// <returns>Returns a value of BufferType corresponding to the vertex buffer</returns>
        public static BufferType ResolveBufferType(VertexBuffer buffer)
        {
            DynamicVertexBuffer dynBuffer = buffer as DynamicVertexBuffer;
            if (dynBuffer != null)
                return (dynBuffer.BufferUsage == BufferUsage.None) ? BufferType.Dynamic : BufferType.DynamicWriteOnly;

            return (buffer.BufferUsage == BufferUsage.None) ? BufferType.Static : BufferType.StaticWriteOnly;
        }

        /// <summary>
        /// Resolves buffer type from an index buffer instance
        /// </summary>
        /// <param name="buffer">Index buffer</param>
        /// <returns>Returns a value of BufferType corresponding to the index buffer</returns>
        public static BufferType ResolveBufferType(IndexBuffer buffer)
        {
            DynamicIndexBuffer dynBuffer = buffer as DynamicIndexBuffer;
            if (dynBuffer != null)
                return (dynBuffer.BufferUsage == BufferUsage.None) ? BufferType.Dynamic : BufferType.DynamicWriteOnly;

            return (buffer.BufferUsage == BufferUsage.None) ? BufferType.Static : BufferType.StaticWriteOnly;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new VertexBufferObject
        /// </summary>
        /// <param name="bufferType">Type of buffer to create</param>
        /// <param name="vertexType">Type of vertex stored in the buffer</param>
        /// <param name="vertexCount">Number of vertices stored in the buffer</param>
        /// <returns>Returns a VertexBufferObject</returns>
        public VertexBufferObject CreateVertexBuffer(BufferType bufferType, Type vertexType, int vertexCount)
        {
            return new VertexBufferObject(_deviceManager.GraphicsDevice, bufferType, vertexType, vertexCount);
        }

        /// <summary>
        /// Creates a new IndexBufferObject
        /// </summary>
        /// <param name="bufferType">Type of buffer to create</param>
        /// <param name="elementSize">Size of index stored in the buffer</param>
        /// <param name="indexCount">Number of indices in the buffer</param>
        /// <returns>Returns an IndexBufferObject</returns>
        public IndexBufferObject CreateIndexBuffer(BufferType bufferType, IndexElementSize elementSize, int indexCount)
        {
            return new IndexBufferObject(_deviceManager.GraphicsDevice, bufferType, elementSize, indexCount);
        }

        /// <summary>
        /// Creates a new VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBuffer used by the VertexBufferObject</param>
        /// <returns>Returns a VertexBufferObject</returns>
        internal VertexBufferObject CreateVertexBuffer(VertexBuffer buffer)
        {
            BufferType bufferType = ResolveBufferType(buffer);

            return new VertexBufferObject(_deviceManager.GraphicsDevice, bufferType, buffer);
        }

        /// <summary>
        /// Creates a new IndexBufferObject
        /// </summary>
        /// <param name="buffer">IndexBuffer used by the IndexBufferObject</param>
        /// <returns>Returns an IndexBufferObject</returns>
        internal IndexBufferObject CreateIndexBuffer(IndexBuffer buffer)
        {
            BufferType bufferType = ResolveBufferType(buffer);

            return new IndexBufferObject(_deviceManager.GraphicsDevice, bufferType, buffer);
        }

        #endregion
    }
}
