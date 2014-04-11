using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Depth24
    {
        #region Fields

        [FieldOffset(0)]
        private readonly float _f;

        [FieldOffset(0)]
        private readonly uint _i;

        #endregion

        #region Constructors

        public Depth24(float depth)
        {
            _i = 0;
            _f = depth;
        }

        #endregion

        #region Methods

        private uint FloatFlip()
        {
            uint mask = (uint)(-(_i >> 31)) | 0x80000000;

            return _i ^ mask;
        }

        public int ToInt32()
        {
            return (int)(_i >> 8);
        }

        public int ToSignedInt32()
        {
            uint result = FloatFlip();

            return (int)(result >> 8);
        }

        public long ToInt64()
        {
            return _i >> 8;
        }

        public long ToSignedInt64()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        public uint ToUInt32()
        {
            return _i >> 8;
        }

        public uint ToSignedUInt32()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        public ulong ToUInt64()
        {
            return _i >> 8;
        }

        public ulong ToSignedUInt64()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        #endregion
    }
}
