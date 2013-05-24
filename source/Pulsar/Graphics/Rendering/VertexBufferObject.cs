using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public sealed class VertexBufferObject : IDisposable
    {
        #region Nested

        private abstract class BufferWrapper
        {
            #region Methods

            public abstract void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct;

            #endregion

            #region Properties

            internal abstract VertexBuffer Buffer { get; }

            #endregion
        }

        private sealed class StaticWrapper : BufferWrapper
        {
            #region Fields

            private readonly VertexBuffer buffer;

            #endregion

            #region Constructors

            public StaticWrapper(VertexBuffer buffer)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("Buffer cannot be null");
                }
                this.buffer = buffer;
            }

            #endregion

            #region Methods

            public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
            {
                this.buffer.SetData(data, startIdx, elementCount);
            }

            #endregion

            #region Properties

            internal override VertexBuffer Buffer
            {
                get { return this.buffer; }
            }

            #endregion
        }

        private sealed class DynamicWrapper : BufferWrapper
        {
            #region Fields

            private readonly DynamicVertexBuffer buffer;

            #endregion

            #region Constructors

            public DynamicWrapper(DynamicVertexBuffer buffer)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("Buffer cannot be null");
                }
                this.buffer = buffer;
            }

            #endregion

            #region Methods

            public override void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option)
            {
                this.buffer.SetData(data, startIdx, elementCount, option);
            }

            #endregion

            #region Properties

            internal override VertexBuffer Buffer
            {
                get { return this.buffer; }
            }

            #endregion
        }

        #endregion

        #region Fields

        private bool disposed;
        private BufferWrapper wrapper;
        private VertexDeclaration declaration;

        #endregion

        #region Constructors

        internal VertexBufferObject(VertexBuffer buffer, VertexDeclaration declaration)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer cannot be null");
            }
            if (declaration == null)
            {
                throw new ArgumentNullException("Vertex declaration cannot be null");
            }
            if (buffer is DynamicVertexBuffer)
            {
                this.wrapper = new DynamicWrapper(buffer as DynamicVertexBuffer);
            }
            else
            {
                this.wrapper = new StaticWrapper(buffer);
            }
            this.declaration = declaration;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (!disposed)
            {
                this.wrapper.Buffer.Dispose();
                this.declaration.Dispose();
            }
            this.disposed = true;
        }

        public void GetData<T>(T[] data) where T : struct
        {
            this.wrapper.Buffer.GetData(data);
        }

        public void SetData<T>(T[] data) where T : struct
        {
            this.wrapper.SetData(data, 0, data.Length, SetDataOptions.Discard);
        }

        public void SetData<T>(T[] data, int startIdx, int elementCount) where T : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, SetDataOptions.Discard);
        }

        public void SetData<T>(T[] data, int startIdx, int elementCount, SetDataOptions option) where T : struct
        {
            this.wrapper.SetData(data, startIdx, elementCount, option);
        }

        #endregion

        #region Properties

        public int VerticesCount
        {
            get { return this.wrapper.Buffer.VertexCount; }
        }

        public VertexDeclaration Declaration
        {
            get { return this.declaration; }
        }

        internal VertexBuffer Buffer
        {
            get { return this.wrapper.Buffer; }
        }

        #endregion
    }
}
