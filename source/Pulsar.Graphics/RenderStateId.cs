using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RenderStateId
    {
        #region Fields

        [FieldOffset(0)]
        public readonly ulong Id;

        [FieldOffset(0)]
        public readonly ushort RasterizerStateId;

        [FieldOffset(2)]
        public readonly ushort DepthStencilStateId;

        [FieldOffset(4)]
        public readonly ushort BlendStateId;

        [FieldOffset(6)]
        public readonly ushort SamplerStateId;

        #endregion

        #region Constructors

        internal RenderStateId(ulong id)
        {
            RasterizerStateId = 0;
            DepthStencilStateId = 0;
            BlendStateId = 0;
            SamplerStateId = 0;
            Id = id;
        }

        internal RenderStateId(ushort rasterizerStateid, ushort depthStencilStateId, ushort blendStateId,
            ushort samplerStateId)
        {
            Id = 0;
            RasterizerStateId = rasterizerStateid;
            DepthStencilStateId = depthStencilStateId;
            BlendStateId = blendStateId;
            SamplerStateId = samplerStateId;
        }

        #endregion

        #region Operators

        public static implicit operator ulong(RenderStateId renderStateId)
        {
            return renderStateId.Id;
        }

        public static implicit operator RenderStateId(ulong id)
        {
            return new RenderStateId(id);
        }

        #endregion
    }
}
