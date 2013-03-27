using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Class containing all information about a geometry object
    /// </summary>
    public sealed class RenderingInfo
    {
        #region Fields

        /// <summary>
        /// ID of the rendering information
        /// </summary>
        public uint ID = uint.MinValue;

        /// <summary>
        /// Primitive type
        /// </summary>
        public PrimitiveType Primitive = PrimitiveType.TriangleList;

        /// <summary>
        /// Offset in the vertex buffer for this instance
        /// </summary>
        public int VertexOffset = 0;

        /// <summary>
        /// Get the starting index in the vertex buffer
        /// </summary>
        public int StartIndex = 0;

        /// <summary>
        /// Total of triangle for this instance
        /// </summary>
        public int TriangleCount = 0;

        /// <summary>
        /// Total of vertex for this instance
        /// </summary>
        public int VertexCount = 0;

        /// <summary>
        /// Vertex buffer for this instance
        /// </summary>
        public VertexBuffer VBuffer = null;

        /// <summary>
        /// Index buffer for this instance
        /// </summary>
        public IndexBuffer IBuffer = null;

        #endregion

        #region Methods

        /// <summary>
        /// Copy this instance of rendering info to an another
        /// </summary>
        /// <param name="other">Rendering info instance receiving all data of this instance</param>
        public void CopyTo(RenderingInfo other)
        {
            other.VBuffer = this.VBuffer;
            other.IBuffer = this.IBuffer;
            other.Primitive = this.Primitive;
            other.StartIndex = this.StartIndex;
            other.TriangleCount = this.TriangleCount;
            other.VertexOffset = this.VertexOffset;
            other.VertexCount = this.VertexCount;
        }

        #endregion
    }
}
