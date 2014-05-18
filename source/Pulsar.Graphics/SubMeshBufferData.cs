namespace Pulsar.Graphics
{
    public sealed partial class Mesh
    {
        #region Nested

        /// <summary>
        /// Contains informations about a submesh buffer
        /// </summary>
        private struct SubMeshBufferData
        {
            #region Fields

            /// <summary>
            /// A buffer used by a submesh
            /// </summary>
            public BufferObject Buffer;

            /// <summary>
            /// Starting offset in the buffer
            /// </summary>
            public int Offset;

            /// <summary>
            /// Number of element used in the buffer
            /// </summary>
            public int Count;

            /// <summary>
            /// Indicates that the buffer is shared with others submesh
            /// </summary>
            public readonly bool Shared;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of SubMeshBufferData struct
            /// </summary>
            /// <param name="buffer">Buffer object</param>
            /// <param name="offset">Starting offset</param>
            /// <param name="count">Number of elements used</param>
            /// <param name="shared">Indicates that buffer is shared</param>
            public SubMeshBufferData(BufferObject buffer, int offset, int count, bool shared)
            {
                Buffer = buffer;
                Offset = offset;
                Count = count;
                Shared = shared;
            }

            #endregion
        }

        #endregion
    }
}
