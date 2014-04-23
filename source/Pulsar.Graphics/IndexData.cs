using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Manages informations about an index buffer and how to use it during rendering operation
    /// </summary>
    public sealed class IndexData : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private IndexBufferObject _indexBuffer;

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                if (_indexBuffer != null)
                    _indexBuffer.Dispose();
            }
            finally
            {
                _indexBuffer = null;

                _isDisposed = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the starting index in the index buffer
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of elements used
        /// </summary>
        public int IndexCount { get; set; }

        /// <summary>
        /// Gets or sets the index buffer
        /// </summary>
        public IndexBufferObject IndexBuffer
        {
            get { return _indexBuffer; }
            set
            {
                if(_isDisposed)
                    throw new Exception("");

                _indexBuffer = value;
            }
        }

        /// <summary>
        /// Gets the hardware index buffer
        /// </summary>
        internal IndexBuffer HardwareBuffer
        {
            get { return IndexBuffer.Buffer; }
        }

        #endregion
    }
}
