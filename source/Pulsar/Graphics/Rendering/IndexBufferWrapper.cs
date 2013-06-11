using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Abstract the use of an index buffer
    /// </summary>
    /// <typeparam name="T">Type of index buffer used</typeparam>
    internal abstract class IndexBufferWrapper<T> : IIndexBufferWrapper where T : IndexBuffer
    {
        #region Fields

        protected readonly T buffer;
        private bool disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of IndexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Index buffer stored in the wrapper</param>
        public IndexBufferWrapper(T buffer)
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
        /// Cast implicitly the wrapper to an IndexBuffer
        /// </summary>
        /// <param name="wrapper">Wrapper to cast</param>
        /// <returns>Return an IndexBuffer</returns>
        public static implicit operator IndexBuffer(IndexBufferWrapper<T> wrapper)
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
        /// <typeparam name="Y">Type of index stored in the buffer</typeparam>
        /// <param name="data">Array in which to store the data</param>
        public void GetData<Y>(Y[] data) where Y : struct
        {
            this.buffer.GetData(data);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of data stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public abstract void SetData<Y>(Y[] data, int startIdx, int elementCount, SetDataOptions option) where Y : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Get the index buffer
        /// </summary>
        public IndexBuffer Buffer 
        {
            get { return this.buffer; }
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for a static index buffer
    /// </summary>
    internal sealed class StaticIndexBufferWrapper : IndexBufferWrapper<IndexBuffer>
    {
        #region Constructors

        /// <summary>
        /// Constructor of StaticIndexBufferWrapper class
        /// </summary>
        /// <param name="buffer">IndexBuffer stored in the wrapper</param>
        public StaticIndexBufferWrapper(IndexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of index stored in the buffer</typeparam>
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
    /// Wrapper for a dynamic index buffer
    /// </summary>
    internal sealed class DynamicIndexBufferWrapper : IndexBufferWrapper<DynamicIndexBuffer>
    {
        #region Constructors

        /// <summary>
        /// Constructor of DynamicIndexBufferWrapper class
        /// </summary>
        /// <param name="buffer">IndexBuffer stored in the wrapper</param>
        public DynamicIndexBufferWrapper(DynamicIndexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of index stored in the buffer</typeparam>
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
