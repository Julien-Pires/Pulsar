using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Used to manage an IndexBufferObject. 
    /// This class is used to find index buffer during rendering operation.
    /// </summary>
    public sealed class IndexData
    {
        #region Fields

        internal IndexBufferObject IndexBufferObj;

        #endregion

        #region Properties

        /// <summary>
        /// Get or the managed IndexBufferObject
        /// </summary>
        public IndexBufferObject IndexBuffer
        {
            get { return IndexBufferObj; }
            set { IndexBufferObj = value; }
        }

        /// <summary>
        /// Get the associated IndexBuffer
        /// </summary>
        internal IndexBuffer Buffer
        {
            get { return IndexBufferObj.Buffer; }
        }

        #endregion
    }
}
