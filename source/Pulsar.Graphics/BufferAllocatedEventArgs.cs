using System;

namespace Pulsar.Graphics
{
    public class BufferAllocatedEventArgs : EventArgs
    {
        #region Constructors

        internal BufferAllocatedEventArgs(BufferObject buffer, int size)
        {
            Buffer = buffer;
            Size = size;
        }

        #endregion

        #region Properties

        public BufferObject Buffer { get; private set; }

        public int Size { get; private set; }

        #endregion
    }
}
