using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public interface IRenderingInfo
    {
        #region Properties

        PrimitiveType PrimitiveType { get; }

        int PrimitiveCount { get; }

        int VertexCount { get; }

        VertexData VertexData { get; }

        IndexData IndexData { get; }

        bool UseIndexes { get; }

        #endregion
    }
}
