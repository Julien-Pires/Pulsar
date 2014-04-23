using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Abstracts the use of an index buffer
    /// </summary>
    /// <typeparam name="TBuffer">Type of index buffer used</typeparam>
    internal abstract class IndexBufferWrapper<TBuffer> : IIndexBufferWrapper where TBuffer : IndexBuffer
    {
        #region Fields

        protected TBuffer Buffer;

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
        /// Casts implicitly the wrapper to an IndexBuffer
        /// </summary>
        /// <param name="wrapper">Wrapper to cast</param>
        /// <returns>Returns an IndexBuffer</returns>
        public static implicit operator IndexBuffer(IndexBufferWrapper<TBuffer> wrapper)
        {
            return wrapper.Buffer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
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
            if (_disposed) 
                return;

            try
            {
                if (disposing)
                    Buffer.Dispose();
            }
            finally
            {
                Buffer = null;

                _disposed = true;
            }
        }

        /// <summary>
        /// Gets the elements stored in the buffer from a starting index
        /// </summary>
        /// <typeparam name="T">Type of index stored in the buffer</typeparam>
        /// <param name="bufferOffset">Starting offset in the buffer</param>
        /// <param name="data">Array in which to store the data</param>
        /// <param name="startIndex">First element to get</param>
        /// <param name="elementCount">Number of element to get</param>
        public void GetData<T>(int bufferOffset, T[] data, int startIndex, int elementCount) where T : struct
        {
            int offsetInByte = bufferOffset * ElementSizeInByte;
            Buffer.GetData(offsetInByte, data, startIndex, elementCount);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data stored in the buffer</typeparam>
        /// <param name="bufferOffset"></param>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public abstract void SetData<T>(int bufferOffset, T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size in byte of one index in the buffer
        /// </summary>
        public int ElementSizeInByte
        {
            get { return (Buffer.IndexElementSize == IndexElementSize.SixteenBits) ? 2 : 4; }
        }

        /// <summary>
        /// Gets the number of elements in the buffer
        /// </summary>
        public int ElementCount
        {
            get { return Buffer.IndexCount; }
        }

        /// <summary>
        /// Gets the index buffer
        /// </summary>
        public IndexBuffer IndexBuffer 
        {
            get { return Buffer; }
        }

        #endregion
    }

    /// <summary>
    /// Encapsulates a static index buffer
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
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of index stored in the buffer</typeparam>
        /// <param name="bufferOffset">Starting offset in the buffer</param>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(int bufferOffset, T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            int offsetInByte = bufferOffset * ElementSizeInByte;
            Buffer.SetData(offsetInByte, data, startIdx, elementCount);
        }

        #endregion
    }

    /// <summary>
    /// Encapsulates a dynamic index buffer
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
        /// <param name="bufferOffset">Starting offset in the buffer</param>
        /// <param name="data">Data to set</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public override void SetData<T>(int bufferOffset, T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            int offsetInByte = bufferOffset * ElementSizeInByte;
            Buffer.SetData(offsetInByte, data, startIdx, elementCount, option);
        }

        #endregion
    }
}
