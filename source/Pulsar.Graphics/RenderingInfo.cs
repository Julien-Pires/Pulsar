using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains information used to draw a 3D shape
    /// </summary>
    public sealed class RenderingInfo : IRenderingInfo
    {
        #region Fields

        private int _vertexCount;
        private PrimitiveType _primitiveType;
        private bool _useIndexes;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RenderingInfo class
        /// </summary>
        internal RenderingInfo()
        {
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

        internal void UpdatePrimitiveCount()
        {
            int indicesCount = (IndexData == null) ? 0 : IndexData.IndexCount;
            PrimitiveCount = ComputePrimitiveCount(PrimitiveType, VertexCount, UseIndexes, indicesCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Vertex data
        /// </summary>
        public VertexData VertexData { get; internal set; }

        /// <summary>
        /// Index data
        /// </summary>
        public IndexData IndexData { get; internal set; }

        /// <summary>
        /// Primitive type used for rendering
        /// </summary>
        public PrimitiveType PrimitiveType
        {
            get { return _primitiveType; }
            internal set
            {
                _primitiveType = value;

                UpdatePrimitiveCount();
            }
        }

        /// <summary>
        /// Number of primitive
        /// </summary>
        public int PrimitiveCount { get; internal set; }

        /// <summary>
        /// Number of vertex
        /// </summary>
        public int VertexCount
        {
            get { return _vertexCount; }
            internal set
            {
                _vertexCount = value;

                UpdatePrimitiveCount();
            }
        }

        /// <summary>
        /// Indicates if an index buffer is used
        /// </summary>
        public bool UseIndexes
        {
            get { return _useIndexes; }
            internal set
            {
                _useIndexes = value;

                UpdatePrimitiveCount();
            }
        }

        #endregion
    }
}
