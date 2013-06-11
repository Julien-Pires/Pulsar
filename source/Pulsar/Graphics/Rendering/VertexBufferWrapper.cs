using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Base class for a wrapper of vertex buffer
    /// </summary>
    /// <typeparam name="T">Type of vertex buffer stored in the wrapper</typeparam>
    internal abstract class VertexBufferWrapper<T> : IVertexBufferWrapper where T : VertexBuffer
    {
        #region Fields

        protected readonly T buffer;
        private bool disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of VertexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Vertex buffer stored in the wrapper</param>
        public VertexBufferWrapper(T buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer cannot be null");
            }
            this.buffer = buffer;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Cast implicitly a VertexBufferWrapper to a VertexBuffer
        /// </summary>
        /// <param name="wrapper">Wrapper to cast</param>
        /// <returns>Return a VertexBuffer</returns>
        public static implicit operator VertexBuffer(VertexBufferWrapper<T> wrapper)
        {
            return wrapper.buffer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose the wrapper
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.buffer.Dispose();
            }
            this.disposed = true;
        }

        /// <summary>
        /// Get the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of vertex in the buffer</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void GetData<Y>(Y[] data) where Y : struct
        {
            this.buffer.GetData(data);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public abstract void SetData<Y>(Y[] data, int startIdx, int elementCount, SetDataOptions option) where Y : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Get the vertex buffer
        /// </summary>
        public VertexBuffer Buffer 
        {
            get { return this.buffer; }
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
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            this.buffer.SetData(data, startIdx, elementCount);
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
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of vertex stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            this.buffer.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}