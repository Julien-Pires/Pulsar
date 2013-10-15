using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Manages an index buffer
    /// </summary>
    public sealed class IndexBufferObject : BufferObject
    {
        #region Fields

        private IIndexBufferWrapper _wrapper;
        private readonly BufferType _bufferType;
        private readonly GraphicsDevice _device;
        private readonly IndexElementSize _elementSize;

        #endregion

        #region Events

        /// <summary>
        /// Occured when a new underlying index buffer is allocated and replaces the current one
        /// </summary>
        public event EventHandler<BufferAllocatedEventArgs> IndexBufferAllocated;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of IndexBufferObject class
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="type">Type of buffer to use</param>
        /// <param name="elementSize">Size of one element stored in the buffer</param>
        /// <param name="indexCount">Initial size of the buffer</param>
        internal IndexBufferObject(GraphicsDevice device, BufferType type, IndexElementSize elementSize, int indexCount)
        {
            _device = device;
            _bufferType = type;
            _elementSize = elementSize;
            CreateBuffer(indexCount);
        }

        /// <summary>
        /// Constructor of IndexBufferObject class
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="type">Type of buffer to use</param>
        /// <param name="buffer">Underlying index buffer</param>
        internal IndexBufferObject(GraphicsDevice device, BufferType type, IndexBuffer buffer)
        {
            _device = device;
            _bufferType = type;
            _elementSize = buffer.IndexElementSize;
            CreateWrapper(buffer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a wrapper to encapsulate an index buffer
        /// </summary>
        /// <param name="buffer">Buffer to encapsulate</param>
        private void CreateWrapper(IndexBuffer buffer)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                _wrapper = null;
            }

            DynamicIndexBuffer dynBuffer = buffer as DynamicIndexBuffer;
            if (dynBuffer == null) _wrapper = new StaticIndexBufferWrapper(buffer);
            else _wrapper = new DynamicIndexBufferWrapper(dynBuffer);
        }

        /// <summary>
        /// Creates a new index buffer with a specific size
        /// </summary>
        /// <param name="elementCount">Size of the index buffer</param>
        protected override void CreateBuffer(int elementCount)
        {
            IndexBuffer buffer = null;
            switch (_bufferType)
            {
                case BufferType.Static:
                    buffer = new IndexBuffer(_device, _elementSize, elementCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly:
                    buffer = new IndexBuffer(_device, _elementSize, elementCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic:
                    buffer = new DynamicIndexBuffer(_device, _elementSize, elementCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly:
                    buffer = new DynamicIndexBuffer(_device, _elementSize, elementCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null) throw new InvalidOperationException("Failed to create an index buffer");
            CreateWrapper(buffer);

            BufferAllocatedEventArgs e = new BufferAllocatedEventArgs(this, elementCount);
            OnIndexBufferAllocated(e);
        }

        /// <summary>
        /// Raises the IndexBufferAllocated event
        /// </summary>
        /// <param name="e">Argument</param>
        private void OnIndexBufferAllocated(BufferAllocatedEventArgs e)
        {
            EventHandler<BufferAllocatedEventArgs> bufferAllocated = IndexBufferAllocated;
            if (bufferAllocated != null)
                bufferAllocated(this, e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the wrapper
        /// </summary>
        public override IBufferWrapper Wrapper
        {
            get { return _wrapper; }
        }

        /// <summary>
        /// Gets the size of an element in the buffer
        /// </summary>
        public IndexElementSize ElementSize
        {
            get { return _wrapper.IndexBuffer.IndexElementSize; }
        }

        /// <summary>
        /// Gets the underlying index buffer
        /// </summary>
        internal IndexBuffer Buffer
        {
            get { return _wrapper.IndexBuffer; }
        }

        #endregion
    }
}