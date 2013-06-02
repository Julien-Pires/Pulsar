using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public sealed class IndexBufferObject : BufferObject<IIndexBufferWrapper>
    {
        #region Constructors

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

        public int IndicesCount
        {
            get { return this.wrapper.Buffer.IndexCount; }
        }

        public IndexElementSize ElementSize
        {
            get { return this.wrapper.Buffer.IndexElementSize; }
        }

        internal IndexBuffer Buffer
        {
            get { return this.wrapper.Buffer; }
        }

        #endregion
    }
}