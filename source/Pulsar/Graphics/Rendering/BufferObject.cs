using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public abstract class BufferObject<T> where T : IBufferWrapper, IDisposable
    {
        #region Fields

        protected T wrapper;
        private bool disposed;

        #endregion

        #region Methods

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.wrapper.Dispose();
            }
            this.disposed = true;
        }

        public void GetData<Y>(Y[] data) where Y : struct
        {
            this.wrapper.GetData(data);
        }

        public void SetData<Y>(Y[] data) where Y : struct
        {
            this.wrapper.SetData(data, 0, data.Length, SetDataOptions.Discard);
        }

        public void SetData<Y>(Y[] data, int startIdx, int elementCount) where Y : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, SetDataOptions.Discard);
        }

        public void SetData<Y>(Y[] data, int startIdx, int elementCount, SetDataOptions option) where Y : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}
