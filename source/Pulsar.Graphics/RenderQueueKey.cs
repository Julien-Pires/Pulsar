using System;

namespace Pulsar.Graphics
{
    public sealed class RenderQueueKey
    {
        #region Fields

        private ulong _id;
        private ulong _material;
        private ulong _pass;
        private ulong _depth;
        private ulong _transparency;
        private ulong _group;
        private Action _update;

        #endregion

        #region Constructors

        internal RenderQueueKey()
        {
            _update = UpdateMaterialFirst;
        }

        #endregion

        #region Methods

        private void UpdateDepthFirst()
        {
            _id |= _pass;
            _id |= _material << 16;
            _id |= _depth << 32;
            _id |= _transparency << 56;
            _id |= _group << 57;
        }

        private void UpdateMaterialFirst()
        {
            _id |= _depth;
            _id |= _pass << 24;
            _id |= _material << 40;
            _id |= _transparency << 56;
            _id |= _group << 57;
        }

        #endregion

        #region Properties

        public ulong Id
        {
            get { return _id; }
        }

        public byte Group
        {
            get { return (byte)_group; }
            set
            {
                _group = (ulong)(value & 0x0F);
                _update();
            }
        }
        
        public bool Transparency
        {
            get { return Convert.ToBoolean(_transparency); }
            set
            {
                _transparency = Convert.ToUInt64(value);
                _update = value ? (Action)UpdateDepthFirst : UpdateMaterialFirst;
                _update();
            }
        }

        public float Depth
        {
            get { return _depth; }
            set
            {
                Depth24 depth = new Depth24(value);
                _depth = depth.ToUInt64();
                _update();
            }
        }

        public ushort Material
        {
            get { return (ushort) _material; }
            set
            {
                _material = value;
                _update();
            }
        }

        public ushort Pass
        {
            get { return (ushort) _pass; }
            set
            {
                _pass = value;
                _update();
            }
        }

        #endregion
    }
}
