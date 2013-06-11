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
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer cannot be null");
            }

            DynamicIndexBuffer dynBuffer = buffer as DynamicIndexBuffer;
            if (dynBuffer == null)
            {
                this.wrapper = new StaticIndexBufferWrapper(buffer);
            }
            else
            {
                this.wrapper = new DynamicIndexBufferWrapper(dynBuffer);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of indices in the buffer
        /// </summary>
        public int IndicesCount
        {
            get { return this.wrapper.Buffer.IndexCount; }
        }

        /// <summary>
        /// Get the size of an index in the buffer
        /// </summary>
        public IndexElementSize ElementSize
        {
            get { return this.wrapper.Buffer.IndexElementSize; }
        }

        /// <summary>
        /// Get the underlying index buffer
        /// </summary>
        internal IndexBuffer Buffer
        {
            get { return this.wrapper.Buffer; }
        }

        #endregion
    }
}