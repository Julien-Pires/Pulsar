namespace Pulsar.Graphics
{
    /// <summary>
    /// Manages multiple vertex buffer object
    /// </summary>
    public sealed partial class VertexData
    {
        #region Nested

        /// <summary>
        /// Allows to associate binding data with a VertexBufferObject
        /// </summary>
        private class VertexBufferBindingInfo
        {
            #region Fields

            /// <summary>
            /// Vertex buffer object
            /// </summary>
            public VertexBufferObject BufferObject;

            /// <summary>
            /// Offset in the buffer
            /// </summary>
            public int Offset;

            /// <summary>
            /// Instancing frequency
            /// </summary>
            public int Frequency;

            #endregion
        }

        #endregion
    }
}
