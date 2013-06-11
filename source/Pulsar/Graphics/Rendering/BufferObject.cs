using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Base class for all buffer objects
    /// </summary>
    /// <typeparam name="T">Type of buffer wrapper used</typeparam>
    public abstract class BufferObject<T> where T : IBufferWrapper, IDisposable
    {
        #region Fields

        protected T wrapper;
        private bool disposed;

        #endregion

        #region Methods

        /// <summary>
        /// Dispose this buffer
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose this buffer
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.wrapper.Dispose();
            }
            this.disposed = true;
        }

        /// <summary>
        /// Get the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void GetData<Y>(Y[] data) where Y : struct
        {
            this.wrapper.GetData(data);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void SetData<Y>(Y[] data) where Y : struct
        {
            this.wrapper.SetData(data, 0, data.Length, SetDataOptions.Discard);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        public void SetData<Y>(Y[] data, int startIdx, int elementCount) where Y : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, SetDataOptions.Discard);
        }

        /// <summary>
        /// Set the data stored in the buffer
        /// </summary>
        /// <typeparam name="Y">Type of data</typeparam>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIdx">Starting index in the buffer</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public void SetData<Y>(Y[] data, int startIdx, int elementCount, SetDataOptions option) where Y : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}
