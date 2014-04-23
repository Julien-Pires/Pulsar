using System;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents an argument used when a buffer changed
    /// </summary>
    public class BufferAllocatedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor of BufferAllocatedEventArgs class
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="size">Size</param>
        internal BufferAllocatedEventArgs(BufferObject buffer, int size)
        {
            Buffer = buffer;
            Size = size;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the buffer
        /// </summary>
        public BufferObject Buffer { get; private set; }

        /// <summary>
        /// Gets the size of the buffer
        /// </summary>
        public int Size { get; private set; }

        #endregion
    }
}
