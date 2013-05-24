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
        internal PrimitiveType primitive = PrimitiveType.TriangleList;

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
        internal VertexData vertexData = null;

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
            other.vertexData = this.vertexData;
            other.iBuffer = this.iBuffer;
            other.primitive = this.primitive;
            other.startIndex = this.startIndex;
            other.triangleCount = this.triangleCount;
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
            get { return this.primitive; }
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
