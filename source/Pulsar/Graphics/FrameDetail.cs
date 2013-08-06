namespace Pulsar.Graphics
{
    public sealed class FrameDetail
    {
        #region Constructor

        internal FrameDetail()
        {
        }

        #endregion

        #region Methods

        internal void Reset()
        {
            DrawCall = 0;
            Vertices = 0;
            Primitives = 0;
            SubMeshes = 0;
        }

        internal void Merge(FrameDetail frame)
        {
            DrawCall += frame.DrawCall;
            Vertices += frame.Vertices;
            Primitives += frame.Primitives;
            SubMeshes += frame.SubMeshes;
        }

        internal void AddDrawCall(uint vertices, uint primitives, uint subMeshes)
        {
            DrawCall++;
            Vertices += vertices;
            Primitives += primitives;
            SubMeshes += subMeshes;
        }

        #endregion

        #region Properties

        public uint DrawCall { get; private set; }

        public uint Vertices { get; private set; }

        public uint Primitives { get; private set; }

        public uint SubMeshes { get; private set; }

        #endregion
    }
}