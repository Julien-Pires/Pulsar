using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents an id for a RenderState instance
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RenderStateId
    {
        #region Fields

        /// <summary>
        /// Id
        /// </summary>
        [FieldOffset(0)]
        public readonly ulong Id;

        /// <summary>
        /// Rasterizer state id
        /// </summary>
        [FieldOffset(0)]
        public readonly ushort RasterizerStateId;

        /// <summary>
        /// DepthStencil state id
        /// </summary>
        [FieldOffset(2)]
        public readonly ushort DepthStencilStateId;

        /// <summary>
        /// Blend state id
        /// </summary>
        [FieldOffset(4)]
        public readonly ushort BlendStateId;

        /// <summary>
        /// Sampler state id
        /// </summary>
        [FieldOffset(6)]
        public readonly ushort SamplerStateId;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of RenderStateId struct
        /// </summary>
        /// <param name="id">Id</param>
        internal RenderStateId(ulong id)
        {
            RasterizerStateId = 0;
            DepthStencilStateId = 0;
            BlendStateId = 0;
            SamplerStateId = 0;
            Id = id;
        }

        /// <summary>
        /// Constructor of RenderStateId struct
        /// </summary>
        /// <param name="rasterizerStateid">Rasterizer id</param>
        /// <param name="depthStencilStateId">DepthStencil id</param>
        /// <param name="blendStateId">Blend id</param>
        /// <param name="samplerStateId">Sampler id</param>
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

        /// <summary>
        /// Converts a RenderStateId to an unsigned long
        /// </summary>
        /// <param name="renderStateId">RenderStateId</param>
        /// <returns>Returns a 64bit value</returns>
        public static implicit operator ulong(RenderStateId renderStateId)
        {
            return renderStateId.Id;
        }

        /// <summary>
        /// Converts an unsigned long to a RenderStateId instance
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Returns a new instance of RenderStateId</returns>
        public static implicit operator RenderStateId(ulong id)
        {
            return new RenderStateId(id);
        }

        #endregion
    }
}
