using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Base class for a wrapper of vertex buffer
    /// </summary>
    /// <typeparam name="TBuffer">Type of vertex buffer stored in the wrapper</typeparam>
    internal abstract class VertexBufferWrapper<TBuffer> : IVertexBufferWrapper where TBuffer : VertexBuffer
    {
        #region Fields

        protected readonly TBuffer Buffer;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of VertexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Vertex buffer stored in the wrapper</param>
        protected VertexBufferWrapper(TBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            Buffer = buffer;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Casts implicitly a VertexBufferWrapper to a VertexBuffer
        /// </summary>
        /// <param name="wrapper">Wrapper to cast</param>
        /// <returns>Return a VertexBuffer</returns>
        public static implicit operator VertexBuffer(VertexBufferWrapper<TBuffer> wrapper)
        {
            return wrapper.Buffer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">Indicates whether the methods is called from IDisposable.Dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing) Buffer.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Gets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of vertex in the buffer</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void GetData<T>(T[] data) where T : struct
        {
            Buffer.GetData(data);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public abstract void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer 
        {
            get { return Buffer; }
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for a static vertex buffer
    /// </summary>
    internal sealed class StaticVertexBufferWrapper : VertexBufferWrapper<VertexBuffer>
    {
        #region Constructors

        /// <summary>
        /// Constructor of StaticVertexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Vertex buffer stored in the wrapper</param>
        public StaticVertexBufferWrapper(VertexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            Buffer.SetData(data, startIdx, elementCount);
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for a dynamic vertex buffer
    /// </summary>
    internal sealed class DynamicVertexBufferWrapper : VertexBufferWrapper<DynamicVertexBuffer>
    {
        #region Constructors

        /// <summary>
        /// Constructor of DynamicVertexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Vertex buffer stored in the wrapper</param>
        public DynamicVertexBufferWrapper(DynamicVertexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            Buffer.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}