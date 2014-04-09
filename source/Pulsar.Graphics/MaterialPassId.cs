using System.Runtime.InteropServices;

namespace Pulsar.Graphics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MaterialPassId
    {
        #region Fields

        [FieldOffset(0)]
        public readonly uint Id;

        [FieldOffset(0)]
        public readonly ushort MaterialId;

        [FieldOffset(2)]
        public readonly ushort PassId;

        #endregion

        #region Constructors

        internal MaterialPassId(uint id)
        {
            MaterialId = 0;
            PassId = 0;
            Id = id;
        }

        internal MaterialPassId(ushort materialId, ushort passId)
        {
            Id = 0;
            MaterialId = materialId;
            PassId = passId;
        }

        #endregion

        #region Operators

        public static explicit operator uint(MaterialPassId materialPassId)
        {
            return materialPassId.Id;
        }

        public static explicit operator MaterialPassId(uint id)
        {
            return new MaterialPassId(id);
        }

        #endregion
    }
}
