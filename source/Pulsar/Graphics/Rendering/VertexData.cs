using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics;
using Pulsar.Extension;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Used to manage multiple vertex buffer binding.
    /// This class allow to set multiple vertex buffer binding during rendering operation.
    /// </summary>
    public sealed class VertexData
    {
        #region Nested

        /// <summary>
        /// Allows to associate binding data with a VertexBufferObject
        /// </summary>
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

        /// <summary>
        /// Get the VertexBufferObject
        /// </summary>
        /// <param name="index">Index at which the buffer object is stored</param>
        /// <returns>Return a VertexBufferObject</returns>
        public VertexBufferObject GetBuffer(ushort index)
        {
            return this.bindings[index].BufferObject;
        }

        /// <summary>
        /// Set a new binding for a specified VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        public void SetBinding(VertexBufferObject buffer)
        {
            this.SetBinding(buffer, 0, 0);
        }

        /// <summary>
        /// Set a new binding for a specified VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        /// <param name="vertexOffset">Vertex offset in the buffer</param>
        /// <param name="frequency">Number of instance to draw</param>
        /// <returns>Return the index at which the binding is stored</returns>
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

        /// <summary>
        /// Remove a vertex buffer binding
        /// </summary>
        /// <param name="index">Index of the binding to remove</param>
        public void UnsetBinding(ushort index)
        {
            this.bindings.RemoveAt(index);
            this.UpdateBindingArray();
        }

        /// <summary>
        /// Remove all vertex buffer bindings
        /// </summary>
        public void UnsetAllBinding()
        {
            this.bindings.Clear();
            this.UpdateBindingArray();
        }

        /// <summary>
        /// Update the binding array
        /// </summary>
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

        /// <summary>
        /// Get the number of buffer
        /// </summary>
        public int BufferCount
        {
            get { return this.bindings.Count; }
        }

        #endregion
    }
}