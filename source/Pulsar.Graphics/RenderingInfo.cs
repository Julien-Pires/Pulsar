using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains information used to draw a 3D shape
    /// </summary>
    public sealed class RenderingInfo
    {
        #region Fields

        private static uint _idCounter = uint.MinValue;

        /// <summary>
        /// Id
        /// </summary>
        public uint Id;

        /// <summary>
        /// Vertex data
        /// </summary>
        public  VertexData VertexData;

        /// <summary>
        /// Index data
        /// </summary>
        public IndexData IndexData;

        /// <summary>
        /// Primitive type used for rendering
        /// </summary>
        public PrimitiveType PrimitiveType;

        /// <summary>
        /// Number of primitive
        /// </summary>
        public int PrimitiveCount;

        /// <summary>
        /// Number of vertex
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// Indicates if an index buffer is used
        /// </summary>
        public bool UseIndexes;

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
            other.UseIndexes = UseIndexes;
        }

        #endregion
    }
}
