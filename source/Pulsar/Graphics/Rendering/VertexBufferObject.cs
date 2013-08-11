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
            if (buffer == null) throw new ArgumentNullException("buffer");

            DynamicVertexBuffer dynBuffer = buffer as DynamicVertexBuffer;
            if (dynBuffer == null) Wrapper = new StaticVertexBufferWrapper(buffer);
            else Wrapper = new DynamicVertexBufferWrapper(dynBuffer);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of vertices in the buffer
        /// </summary>
        public int VerticesCount
        {
            get { return Wrapper.VertexBuffer.VertexCount; }
        }

        /// <summary>
        /// Gets the vertex declaration associated with the buffer
        /// </summary>
        public VertexDeclaration Declaration
        {
            get { return Buffer.VertexDeclaration; }
        }

        /// <summary>
        /// Gets the vertex buffer
        /// </summary>
        internal VertexBuffer Buffer
        {
            get { return Wrapper.VertexBuffer; }
        }

        #endregion
    }
}
