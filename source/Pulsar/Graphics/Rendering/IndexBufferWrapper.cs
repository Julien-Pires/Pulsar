using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Abstract the use of an index buffer
    /// </summary>
    /// <typeparam name="TBuffer">Type of index buffer used</typeparam>
    internal abstract class IndexBufferWrapper<TBuffer> : IIndexBufferWrapper where TBuffer : IndexBuffer
    {
        #region Fields

        protected readonly TBuffer Buffer;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of IndexBufferWrapper class
        /// </summary>
        /// <param name="buffer">Index buffer stored in the wrapper</param>
        protected IndexBufferWrapper(TBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            Buffer = buffer;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Cast implicitly the wrapper to an IndexBuffer
        /// </summary>
        /// <param name="wrapper">Wrapper to cast</param>
        /// <returns>Return an IndexBuffer</returns>
        public static implicit operator IndexBuffer(IndexBufferWrapper<TBuffer> wrapper)
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
        /// Get the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of index stored in the buffer</typeparam>
        /// <param name="data">Array in which to store the data</param>
        public void GetData<T>(T[] data) where T : struct
        {
            Buffer.GetData(data);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data stored in the buffer</typeparam>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public abstract void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Get the index buffer
        /// </summary>
        public IndexBuffer IndexBuffer 
        {
            get { return Buffer; }
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
            Buffer.SetData(data, startIdx, elementCount);
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
            Buffer.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}
