namespace Pulsar.Graphics.Fx
{
    internal sealed partial class PassIdPool
    {
        private struct PassIdRange
        {
            #region Fields

            public readonly ushort FirstId;

            public readonly ushort Length;

            #endregion

            #region Constructors

            public PassIdRange(ushort firstId, ushort length)
            {
                FirstId = firstId;
                Length = length;
            }

            #endregion
        }
    }
}
