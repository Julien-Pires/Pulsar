using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics;
using Pulsar.Extension;

namespace Pulsar.Graphics.Rendering
{
    public sealed class VertexData
    {
        #region Nested

        private class BindingInfo
        {
            #region Fields

            public VertexBufferObject BufferObject;

            public int VertexOffset;

            public int Frequency;

            #endregion
        }

        #endregion

        #region Fields

        internal VertexBufferBinding[] VertexBindings = new VertexBufferBinding[0];
        private List<BindingInfo> bindings = new List<BindingInfo>();

        #endregion

        #region Methods

        public VertexBufferObject GetBuffer(ushort index)
        {
            return this.bindings[index].BufferObject;
        }

        public void SetBinding(VertexBufferObject buffer)
        {
            this.SetBinding(buffer, 0, 0);
        }

        public ushort SetBinding(VertexBufferObject buffer, int vertexOffset, int frequency)
        {
            if(buffer == null)
            {
                throw new ArgumentNullException("Buffer cannot be null");
            }
            BindingInfo inf = new BindingInfo()
            {
                BufferObject = buffer,
                VertexOffset = vertexOffset,
                Frequency = frequency
            };
            this.bindings.Add(inf);
            this.UpdateBindingArray();

            return (ushort)(this.bindings.Count - 1);
        }

        public void UnsetBinding(ushort index)
        {
            this.bindings.RemoveAt(index);
            this.UpdateBindingArray();
        }

        public void UnsetAllBinding()
        {
            this.bindings.Clear();
            this.UpdateBindingArray();
        }

        private void UpdateBindingArray()
        {
            ushort newSize = (ushort)this.bindings.Count;
            VertexBufferBinding[] newBindings = new VertexBufferBinding[newSize];
            for (ushort i = 0; i < newSize; i++)
            {
                BindingInfo inf = this.bindings[i];
                VertexBufferBinding bufferBinding = new VertexBufferBinding(inf.BufferObject.Buffer, inf.VertexOffset, inf.Frequency);
                newBindings[i] = bufferBinding;
            }
            this.VertexBindings = newBindings;
        }

        #endregion

        #region Properties

        public int BufferCount
        {
            get { return this.bindings.Count; }
        }

        #endregion
    }
}