using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Manages a vertex buffer
    /// </summary>
    public sealed class VertexBufferObject : BufferObject
    {
        #region Fields

        private IVertexBufferWrapper _wrapper;
        private readonly GraphicsDevice _device;
        private readonly BufferType _bufferType;
        private readonly VertexDeclaration _vertexDeclaration;

        #endregion

        #region Events

        /// <summary>
        /// Occured when a new underlying vertex buffer is allocated and replaces the current one
        /// </summary>
        public event EventHandler<BufferAllocatedEventArgs> VertexBufferAllocated;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of VertexBufferObject class
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="type">Type of buffer to use</param>
        /// <param name="vertexType">Type of vertex used in the buffer</param>
        /// <param name="vertexCount">Initial size of the buffer</param>
        internal VertexBufferObject(GraphicsDevice device, BufferType type, Type vertexType, int vertexCount)
        {
            if(device == null) throw new ArgumentNullException("device");
            if(vertexType == null) throw new ArgumentNullException("vertexType");

            IVertexType vertex = Activator.CreateInstance(vertexType) as IVertexType;
            if(vertex == null) throw new ArgumentException("Invalid vertex type");

            _device = device;
            _bufferType = type;
            _vertexDeclaration = vertex.VertexDeclaration;
            CreateBuffer(vertexCount);
        }

        /// <summary>
        /// Constructor of VertexBufferObject class
        /// </summary>
        /// <param name="device">Graphics device</param>
        /// <param name="type">Type of buffer to use</param>
        /// <param name="buffer">Underlying vertex buffer</param>
        internal VertexBufferObject(GraphicsDevice device, BufferType type, VertexBuffer buffer)
        {
            _device = device;
            _bufferType = type;
            _vertexDeclaration = buffer.VertexDeclaration;
            CreateWrapper(buffer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a wrapper to encapsulate a vertex buffer 
        /// </summary>
        /// <param name="buffer"></param>
        private void CreateWrapper(VertexBuffer buffer)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                _wrapper = null;
            }

            DynamicVertexBuffer dynBuffer = buffer as DynamicVertexBuffer;
            if (dynBuffer == null) _wrapper = new StaticVertexBufferWrapper(buffer);
            else _wrapper = new DynamicVertexBufferWrapper(dynBuffer);
        }

        /// <summary>
        /// Creates a new vertex buffer with a specific size
        /// </summary>
        /// <param name="elementCount"></param>
        protected override void CreateBuffer(int elementCount)
        {
            VertexBuffer buffer = null;
            switch (_bufferType)
            {
                case BufferType.Static:
                    buffer = new VertexBuffer(_device, _vertexDeclaration, elementCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly:
                    buffer = new VertexBuffer(_device, _vertexDeclaration, elementCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic:
                    buffer = new DynamicVertexBuffer(_device, _vertexDeclaration, elementCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly:
                    buffer = new DynamicVertexBuffer(_device, _vertexDeclaration, elementCount, BufferUsage.WriteOnly);
                    break;
            }
            if(buffer == null) throw new InvalidOperationException("Failed to create a vertex buffer");
            CreateWrapper(buffer);

            BufferAllocatedEventArgs e = new BufferAllocatedEventArgs(this, elementCount);
            OnVertexBufferAllocated(e);
        }

        /// <summary>
        /// Raises the VertexBufferAllocated event
        /// </summary>
        /// <param name="e">Argument</param>
        private void OnVertexBufferAllocated(BufferAllocatedEventArgs e)
        {
            EventHandler<BufferAllocatedEventArgs> bufferAllocated = VertexBufferAllocated;
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
        /// Gets the vertex declaration associated with the buffer
        /// </summary>
        public VertexDeclaration Declaration
        {
            get { return _wrapper.VertexBuffer.VertexDeclaration; }
        }

        /// <summary>
        /// Gets the underlying vertex buffer
        /// </summary>
        internal VertexBuffer Buffer
        {
            get { return _wrapper.VertexBuffer; }
        }

        #endregion
    }
}
