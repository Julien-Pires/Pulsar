using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Store and manage a vertex buffer
    /// </summary>
    public sealed class VertexBufferObject : BufferObject<IVertexBufferWrapper>
    {
        #region Constructors

        /// <summary>
        /// Constructor of VertexBufferObject class
        /// </summary>
        /// <param name="buffer">Vertex buffer managed by the buffe object</param>
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

        /// <summary>
        /// Get the number of vertices in the buffer
        /// </summary>
        public int VerticesCount
        {
            get { return this.wrapper.Buffer.VertexCount; }
        }

        /// <summary>
        /// Get the vertex declaration associated with the buffer
        /// </summary>
        public VertexDeclaration Declaration
        {
            get { return this.Buffer.VertexDeclaration; }
        }

        /// <summary>
        /// Get the vertex buffer
        /// </summary>
        internal VertexBuffer Buffer
        {
            get { return this.wrapper.Buffer; }
        }

        #endregion
    }
}
