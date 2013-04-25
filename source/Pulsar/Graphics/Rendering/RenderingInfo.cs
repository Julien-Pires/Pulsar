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
        /// Id counter
        /// </summary>
        private static uint idCounter = uint.MinValue;

        /// <summary>
        /// ID of the rendering information
        /// </summary>
        internal uint id = uint.MinValue;

        /// <summary>
        /// Primitive type
        /// </summary>
        internal PrimitiveType Primitive = PrimitiveType.TriangleList;

        /// <summary>
        /// Offset in the vertex buffer for this instance
        /// </summary>
        internal int vertexOffset = 0;

        /// <summary>
        /// Get the starting index in the vertex buffer
        /// </summary>
        internal int startIndex = 0;

        /// <summary>
        /// Total of triangle for this instance
        /// </summary>
        internal int triangleCount = 0;

        /// <summary>
        /// Total of vertex for this instance
        /// </summary>
        internal int vertexCount = 0;

        /// <summary>
        /// Indicates if an index buffer is used
        /// </summary>
        internal bool useIndexes = false;

        /// <summary>
        /// Vertex buffer for this instance
        /// </summary>
        internal VertexBuffer vBuffer = null;

        /// <summary>
        /// Index buffer for this instance
        /// </summary>
        internal IndexBuffer iBuffer = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RenderingInfo class
        /// </summary>
        internal RenderingInfo()
        {
            this.id = RenderingInfo.idCounter;
            RenderingInfo.idCounter++;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copy this instance of rendering info to an another
        /// </summary>
        /// <param name="other">Rendering info instance receiving all data of this instance</param>
        public void CopyTo(RenderingInfo other)
        {
            other.vBuffer = this.vBuffer;
            other.iBuffer = this.iBuffer;
            other.Primitive = this.Primitive;
            other.startIndex = this.startIndex;
            other.triangleCount = this.triangleCount;
            other.vertexOffset = this.vertexOffset;
            other.vertexCount = this.vertexCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the Id
        /// </summary>
        public uint ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Get the primitive type used for rendering
        /// </summary>
        public PrimitiveType PrimitiveType
        {
            get { return this.Primitive; }
        }

        /// <summary>
        /// Get the vertex offset
        /// </summary>
        public int VertexOffset
        {
            get { return this.vertexOffset; }
        }

        /// <summary>
        /// Get the start index
        /// </summary>
        public int StartIndex
        {
            get { return this.startIndex; }
        }

        /// <summary>
        /// Get the number of primitive
        /// </summary>
        public int PrimitiveCount
        {
            get { return this.triangleCount; }
        }

        /// <summary>
        /// Get the number of vertex
        /// </summary>
        public int VertexCount
        {
            get { return this.vertexCount; }
        }

        #endregion
    }
}
