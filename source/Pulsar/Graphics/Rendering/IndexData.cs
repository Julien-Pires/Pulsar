using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Manages informations about an index buffer and how to use it during rendering operation
    /// </summary>
    public sealed class IndexData
    {
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
        public IndexBufferObject IndexBuffer { get; set; }

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
