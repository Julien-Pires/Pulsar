using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Contains information used to draw a 3D shape
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
            Id = _idCounter++;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Computes number of primtives that composed the 3D shape
        /// </summary>
        public void ComputePrimitiveCount()
        {
            if (VertexCount == 0)
            {
                PrimitiveCount = 0;
                return;
            }

            switch (PrimitiveType)
            {
                case PrimitiveType.LineList:
                    PrimitiveCount = (UseIndexes ? IndexData.IndexCount : VertexCount) / 2;
                    break;
                case PrimitiveType.LineStrip:
                    PrimitiveCount = (UseIndexes ? IndexData.IndexCount : VertexCount) - 1;
                    break;
                case PrimitiveType.TriangleList:
                    PrimitiveCount = (UseIndexes ? IndexData.IndexCount : VertexCount) / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    PrimitiveCount = (UseIndexes ? IndexData.IndexCount : VertexCount) - 2;
                    break;
            }
        }

        /// <summary>
        /// Copies this instance of rendering info to an another
        /// </summary>
        /// <param name="other">Rendering info instance receiving all data of this instance</param>
        public void CopyTo(RenderingInfo other)
        {
            other.VertexData = VertexData;
            other.IndexData = IndexData;
            other.PrimitiveType = PrimitiveType;
            other.PrimitiveCount = PrimitiveCount;
            other.VertexCount = VertexCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Id
        /// </summary>
        public uint Id { get; internal set; }

        /// <summary>
        /// Gets the primitive type used for rendering
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; }

        /// <summary>
        /// Gets the number of primitive
        /// </summary>
        public int PrimitiveCount { get; set; }

        /// <summary>
        /// Gets the number of vertex
        /// </summary>
        public int VertexCount { get; set; }

        /// <summary>
        /// Gets a value indicating if an index buffer is used
        /// </summary>
        public bool UseIndexes { get; set; }

        #endregion
    }
}
