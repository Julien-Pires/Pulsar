using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Base class for all buffer objects
    /// </summary>
    /// <typeparam name="TWrapper">Type of buffer wrapper used</typeparam>
    public abstract class BufferObject<TWrapper> where TWrapper : IBufferWrapper, IDisposable
    {
        #region Fields

        protected TWrapper Wrapper;
        private bool _disposed;

        #endregion

        #region Methods

        /// <summary>
        /// Dispose this buffer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose this buffer
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Wrapper.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// Get the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void GetData<T>(T[] data) where T : struct
        {
            Wrapper.GetData(data);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void SetData<T>(T[] data) where T : struct
        {
            Wrapper.SetData(data, 0, data.Length, SetDataOptions.Discard);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        public void SetData<T>(T[] data, int startIdx, int elementCount) where T : struct
        {
            Wrapper.SetData(data, startIdx, elementCount, SetDataOptions.Discard);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct
        {
            Wrapper.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}
