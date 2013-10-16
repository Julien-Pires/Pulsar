using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Manages multiple vertex buffer object
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

            /// <summary>
            /// Vertex buffer object
            /// </summary>
            public VertexBufferObject BufferObject;

            /// <summary>
            /// Offset in the buffer
            /// </summary>
            public int VertexOffset;

            /// <summary>
            /// Instancing frequency
            /// </summary>
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
        /// Gets the VertexBufferObject at the specified index
        /// </summary>
        /// <param name="index">Index of the buffer</param>
        /// <returns>Returns a VertexBufferObject</returns>
        public VertexBufferObject GetBuffer(int index)
        {
            return _bindings[index].BufferObject;
        }

        /// <summary>
        /// Gets the offset in the buffer at the specified index
        /// </summary>
        /// <param name="index">Index of the offset</param>
        /// <returns>Returns an int that represents a vertex offset</returns>
        public int GetVertexOffset(int index)
        {
            return _bindings[index].VertexOffset;
        }

        /// <summary>
        /// Sets the offset in the buffer at the specified index
        /// </summary>
        /// <param name="offset">New offset value</param>
        /// <param name="index">Index of the offset to replace</param>
        public void SetVertexOffset(int offset, int index)
        {
            _bindings[index].VertexOffset = offset;
            UpdateBindingArray(false);
        }

        /// <summary>
        /// Sets a new binding
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        public int SetBinding(VertexBufferObject buffer)
        {
            return SetBinding(buffer, 0, 0);
        }

        /// <summary>
        /// Sets a new binding
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        /// <param name="vertexOffset">Vertex offset in the buffer</param>
        /// <param name="frequency">Number of instance to draw</param>
        /// <returns>Returns the index at which the binding is stored</returns>
        public int SetBinding(VertexBufferObject buffer, int vertexOffset, int frequency)
        {
            int index = _bindings.Count;
            SetBinding(buffer, vertexOffset, frequency, index);

            return index;
        }

        /// <summary>
        /// Sets a new binding
        /// </summary>
        /// <param name="buffer">VertexBufferObject for the binding</param>
        /// <param name="vertexOffset">Vertex offset in the buffer</param>
        /// <param name="frequency">Number of instance to draw</param>
        /// <param name="index">Index where to insert the binding</param>
        public void SetBinding(VertexBufferObject buffer, int vertexOffset, int frequency, int index)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            BindingInfo inf = new BindingInfo
            {
                BufferObject = buffer,
                VertexOffset = vertexOffset,
                Frequency = frequency
            };

            if(index >= _bindings.Count) _bindings.Add(inf);
            else _bindings.Insert(index, inf);

            buffer.VertexBufferAllocated += BufferAllocated;
            UpdateBindingArray(true);
        }

        /// <summary>
        /// Removes a vertex buffer binding
        /// </summary>
        /// <param name="index">Index of the binding to remove</param>
        public void UnsetBinding(int index)
        {
            if(index >= _bindings.Count) return;

            _bindings[index].BufferObject.VertexBufferAllocated -= BufferAllocated;
            _bindings.RemoveAt(index);
            UpdateBindingArray(true);
        }

        /// <summary>
        /// Removes all vertex buffer bindings
        /// </summary>
        public void UnsetAllBinding()
        {
            for (int i = 0; i < _bindings.Count; i++)
                _bindings[i].BufferObject.VertexBufferAllocated -= BufferAllocated;

            _bindings.Clear();
            UpdateBindingArray(true);
        }

        /// <summary>
        /// Updates the binding array
        /// </summary>
        private void UpdateBindingArray(bool resizeArray)
        {
            if (resizeArray || (VertexBindings == null))
                VertexBindings = new VertexBufferBinding[_bindings.Count];

            for (int i = 0; i < VertexBindings.Length; i++)
            {
                BindingInfo inf = _bindings[i];
                VertexBufferBinding bufferBinding = new VertexBufferBinding(inf.BufferObject.Buffer, 
                    inf.VertexOffset, inf.Frequency);
                VertexBindings[i] = bufferBinding;
            }
        }

        /// <summary>
        /// Calls when a vertex buffer object allocate a new hardware vertex buffer
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Argument</param>
        private void BufferAllocated(object sender, BufferAllocatedEventArgs e)
        {
            UpdateBindingArray(false);
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