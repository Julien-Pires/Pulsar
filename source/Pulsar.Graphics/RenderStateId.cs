using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RenderStateId
    {
        #region Fields

        [FieldOffset(0)]
        public readonly ulong _id;

        [FieldOffset(0)]
        public readonly ushort _rasterizerStateId;

        [FieldOffset(2)]
        public readonly ushort _depthStencilStateId;

        [FieldOffset(4)]
        public readonly ushort _blendStateId;

        [FieldOffset(6)]
        public readonly ushort _samplerStateId;

        #endregion

        #region Constructors

        internal RenderStateId(ulong id)
        {
            _rasterizerStateId = 0;
            _depthStencilStateId = 0;
            _blendStateId = 0;
            _samplerStateId = 0;
            _id = id;
        }

        internal RenderStateId(ushort rasterizerStateid, ushort depthStencilStateId, ushort blendStateId,
            ushort samplerStateId)
        {
            _id = 0;
            _rasterizerStateId = rasterizerStateid;
            _depthStencilStateId = depthStencilStateId;
            _blendStateId = blendStateId;
            _samplerStateId = samplerStateId;
        }

        #endregion

        #region Operators

        public static implicit operator ulong(RenderStateId renderStateId)
        {
            return renderStateId._id;
        }

        public static implicit operator RenderStateId(ulong id)
        {
            return new RenderStateId(id);
        }

        #endregion
    }
}
