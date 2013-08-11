using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Class containing all information about a geometry object
    /// </summary>
    public sealed class RenderingInfo
    {
        #region Fields

        private static uint _idCounter = uint.MinValue;

        internal VertexData VertexData;
        internal IndexData IndexData;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RenderingInfo class
        /// </summary>
        internal RenderingInfo()
        {
            PrimitiveType = PrimitiveType.TriangleList;
            Id = _idCounter++;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copy this instance of rendering info to an another
        /// </summary>
        /// <param name="other">Rendering info instance receiving all data of this instance</param>
        public void CopyTo(RenderingInfo other)
        {
            other.VertexData = VertexData;
            other.IndexData = IndexData;
            other.PrimitiveType = PrimitiveType;
            other.StartIndex = StartIndex;
            other.PrimitiveCount = PrimitiveCount;
            other.VertexCount = VertexCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the Id
        /// </summary>
        public uint Id { get; internal set; }

        /// <summary>
        /// Get the primitive type used for rendering
        /// </summary>
        public PrimitiveType PrimitiveType { get; internal set; }

        /// <summary>
        /// Get the start index
        /// </summary>
        public int StartIndex { get; internal set; }

        /// <summary>
        /// Get the number of primitive
        /// </summary>
        public int PrimitiveCount { get; internal set; }

        /// <summary>
        /// Get the number of vertex
        /// </summary>
        public int VertexCount { get; internal set; }

        /// <summary>
        /// Get a value indicating if index buffer is used
        /// </summary>
        public bool UseIndexes { get; internal set; }

        #endregion
    }
}
