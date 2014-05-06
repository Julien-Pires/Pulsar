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
        public VertexData VertexData;

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

        #region Static methods

        /// <summary>
        /// Computes number of primtives that composed the 3D shape
        /// </summary>
        public static int ComputePrimitiveCount(PrimitiveType primitive, int vertexCount, bool useIndices, int indexCount)
        {
            if (vertexCount == 0)
                return 0;

            int result = 0;
            switch (primitive)
            {
                case PrimitiveType.LineList:
                    result = (useIndices ? indexCount : vertexCount) / 2;
                    break;
                case PrimitiveType.LineStrip:
                    result = (useIndices ? indexCount : vertexCount) - 1;
                    break;
                case PrimitiveType.TriangleList:
                    result = (useIndices ? indexCount : vertexCount) / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    result = (useIndices ? indexCount : vertexCount) - 2;
                    break;
            }

            return result;
        }

        #endregion

        #region Methods

        public void UpdatePrimitiveCount()
        {
            int indicesCount = (IndexData == null) ? 0 : IndexData.IndexCount;
            PrimitiveCount = ComputePrimitiveCount(PrimitiveType, VertexCount, UseIndexes, indicesCount);
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
