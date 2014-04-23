using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a depth value encoded on 24 bit
    /// </summary>
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

        /// <summary>
        /// Constructor of Depth24 struct
        /// </summary>
        /// <param name="depth">Depth</param>
        public Depth24(float depth)
        {
            _i = 0;
            _f = depth;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the depth to keep its sign
        /// </summary>
        /// <returns></returns>
        private uint FloatFlip()
        {
            uint mask = (uint)(-(_i >> 31)) | 0x80000000;

            return _i ^ mask;
        }

        /// <summary>
        /// Gets the depth as an int32
        /// </summary>
        /// <returns></returns>
        public int ToInt32()
        {
            return (int)(_i >> 8);
        }

        /// <summary>
        /// Gets the signed depth as an int32
        /// </summary>
        /// <returns></returns>
        public int ToSignedInt32()
        {
            uint result = FloatFlip();

            return (int)(result >> 8);
        }

        /// <summary>
        /// Gets the depth as an int64
        /// </summary>
        /// <returns></returns>
        public long ToInt64()
        {
            return _i >> 8;
        }

        /// <summary>
        /// Gets the signed depth as an int64
        /// </summary>
        /// <returns></returns>
        public long ToSignedInt64()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        /// <summary>
        /// Gets the depth as an unsigned int32
        /// </summary>
        /// <returns></returns>
        public uint ToUInt32()
        {
            return _i >> 8;
        }

        /// <summary>
        /// Gets the signed depth as an unsigned int32
        /// </summary>
        /// <returns></returns>
        public uint ToSignedUInt32()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        /// <summary>
        /// Gets the depth as an unsigned int64
        /// </summary>
        /// <returns></returns>
        public ulong ToUInt64()
        {
            return _i >> 8;
        }

        /// <summary>
        /// Gets the signed depth as an unsigned int64
        /// </summary>
        /// <returns></returns>
        public ulong ToSignedUInt64()
        {
            uint result = FloatFlip();

            return result >> 8;
        }

        #endregion
    }
}
