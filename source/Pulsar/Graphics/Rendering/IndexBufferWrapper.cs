using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    internal abstract class IndexBufferWrapper<T> : IIndexBufferWrapper where T : IndexBuffer
    {
        #region Fields

        protected readonly T buffer;
        private bool disposed;

        #endregion

        #region Constructors

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

        public static implicit operator IndexBuffer(IndexBufferWrapper<T> wrapper)
        {
            return wrapper.buffer;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.buffer.Dispose();
            }
            this.disposed = true;
        }

        public void GetData<Y>(Y[] data) where Y : struct
        {
            this.buffer.GetData(data);
        }

        public abstract void SetData<Y>(Y[] data, int startIdx, int elementCount, SetDataOptions option) where Y : struct;

        #endregion

        #region Properties

        public IndexBuffer Buffer 
        {
            get { return this.buffer; }
        }

        #endregion
    }

    internal sealed class StaticIndexBufferWrapper : IndexBufferWrapper<IndexBuffer>
    {
        #region Constructors

        public StaticIndexBufferWrapper(IndexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            this.buffer.SetData(data, startIdx, elementCount);
        }

        #endregion
    }

    internal sealed class DynamicIndexBufferWrapper : IndexBufferWrapper<DynamicIndexBuffer>
    {
        #region Constructors

        public DynamicIndexBufferWrapper(DynamicIndexBuffer buffer) : base(buffer)
        {
        }

        #endregion

        #region Methods

        public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
        {
            this.buffer.SetData(data, startIdx, elementCount, option);
        }

        #endregion
    }
}
