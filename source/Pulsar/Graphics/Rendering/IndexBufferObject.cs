using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Store and manage an index buffer
    /// </summary>
    public sealed class IndexBufferObject : BufferObject<IIndexBufferWrapper>
    {
        #region Constructors

        /// <summary>
        /// Constructor of IndexBufferObject class
        /// </summary>
        /// <param name="buffer">Index buffer managed by the buffer object</param>
        internal IndexBufferObject(IndexBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            DynamicIndexBuffer dynBuffer = buffer as DynamicIndexBuffer;
            if (dynBuffer == null) Wrapper = new StaticIndexBufferWrapper(buffer);
            else Wrapper = new DynamicIndexBufferWrapper(dynBuffer);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of indices in the buffer
        /// </summary>
        public int IndicesCount
        {
            get { return Wrapper.IndexBuffer.IndexCount; }
        }

        /// <summary>
        /// Get the size of an index in the buffer
        /// </summary>
        public IndexElementSize ElementSize
        {
            get { return Wrapper.IndexBuffer.IndexElementSize; }
        }

        /// <summary>
        /// Get the underlying index buffer
        /// </summary>
        internal IndexBuffer Buffer
        {
            get { return Wrapper.IndexBuffer; }
        }

        #endregion
    }
}