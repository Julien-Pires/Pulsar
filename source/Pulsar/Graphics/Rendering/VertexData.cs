using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

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

        private readonly List<BindingInfo> _bindings = new List<BindingInfo>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the VertexBufferObject
        /// </summary>
        /// <param name="index">Index at which the buffer object is stored</param>
        /// <returns>Return a VertexBufferObject</returns>
        public VertexBufferObject GetBuffer(int index)
        {
            return _bindings[index].BufferObject;
        }

        /// <summary>
        /// Sets a new binding for a specified VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        public int SetBinding(VertexBufferObject buffer)
        {
            return SetBinding(buffer, 0, 0);
        }

        /// <summary>
        /// Sets a new binding for a specified VertexBufferObject
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        /// <param name="vertexOffset">Vertex offset in the buffer</param>
        /// <param name="frequency">Number of instance to draw</param>
        /// <returns>Return the index at which the binding is stored</returns>
        public int SetBinding(VertexBufferObject buffer, int vertexOffset, int frequency)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            BindingInfo inf = new BindingInfo
            {
                BufferObject = buffer,
                VertexOffset = vertexOffset,
                Frequency = frequency
            };
            _bindings.Add(inf);
            UpdateBindingArray();

            return _bindings.Count - 1;
        }

        /// <summary>
        /// Removes a vertex buffer binding
        /// </summary>
        /// <param name="index">Index of the binding to remove</param>
        public void UnsetBinding(int index)
        {
            _bindings.RemoveAt(index);
            UpdateBindingArray();
        }

        /// <summary>
        /// Removes all vertex buffer bindings
        /// </summary>
        public void UnsetAllBinding()
        {
            _bindings.Clear();
            UpdateBindingArray();
        }

        /// <summary>
        /// Updates the binding array
        /// </summary>
        private void UpdateBindingArray()
        {
            int newSize = _bindings.Count;
            VertexBufferBinding[] newBindings = new VertexBufferBinding[newSize];
            for (int i = 0; i < newSize; i++)
            {
                BindingInfo inf = _bindings[i];
                VertexBufferBinding bufferBinding = new VertexBufferBinding(inf.BufferObject.Buffer, inf.VertexOffset, inf.Frequency);
                newBindings[i] = bufferBinding;
            }
            VertexBindings = newBindings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of buffer
        /// </summary>
        public int BufferCount
        {
            get { return _bindings.Count; }
        }

        #endregion
    }
}