namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains statistics for a frame
    /// </summary>
    public sealed class FrameDetail
    {
        #region Constructor

        /// <summary>
        /// Constructor of FrameDetail class
        /// </summary>
        internal FrameDetail()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset statistics
        /// </summary>
        internal void Reset()
        {
            DrawCall = 0;
            Vertices = 0;
            Primitives = 0;
            SubMeshes = 0;
        }

        /// <summary>
        /// Merge statistics from another instance with this one
        /// </summary>
        /// <param name="frame">FrameDetail instance to merge with</param>
        internal void Merge(FrameDetail frame)
        {
            DrawCall += frame.DrawCall;
            Vertices += frame.Vertices;
            Primitives += frame.Primitives;
            SubMeshes += frame.SubMeshes;
        }

        /// <summary>
        /// Add a new draw call to statistics
        /// </summary>
        /// <param name="vertices">Number of vertices used</param>
        /// <param name="primitives">Number of primitives drawn</param>
        /// <param name="subMeshes">Number of sub meshes drawn</param>
        internal void AddDrawCall(uint vertices, uint primitives, uint subMeshes)
        {
            DrawCall++;
            Vertices += vertices;
            Primitives += primitives;
            SubMeshes += subMeshes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the number of draw call
        /// </summary>
        public uint DrawCall { get; private set; }

        /// <summary>
        /// Get the number of vertices
        /// </summary>
        public uint Vertices { get; private set; }

        /// <summary>
        /// Get the number of primitives
        /// </summary>
        public uint Primitives { get; private set; }

        /// <summary>
        /// Get the number of sub meshes
        /// </summary>
        public uint SubMeshes { get; private set; }

        #endregion
    }
}