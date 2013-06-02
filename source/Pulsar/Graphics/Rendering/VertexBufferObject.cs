using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public sealed class VertexBufferObject : BufferObject<IVertexBufferWrapper>
    {
        #region Constructors

        internal VertexBufferObject(VertexBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer cannot be null");
            }

            DynamicVertexBuffer dynBuffer = buffer as DynamicVertexBuffer;
            if (dynBuffer == null)
            {
                this.wrapper = new StaticVertexBufferWrapper(buffer);
            }
            else
            {
                this.wrapper = new DynamicVertexBufferWrapper(buffer as DynamicVertexBuffer);
            }
        }

        #endregion

        #region Properties

        public int VerticesCount
        {
            get { return this.wrapper.Buffer.VertexCount; }
        }

        public VertexDeclaration Declaration
        {
            get { return this.Buffer.VertexDeclaration; }
        }

        internal VertexBuffer Buffer
        {
            get { return this.wrapper.Buffer; }
        }

        #endregion
    }
}
