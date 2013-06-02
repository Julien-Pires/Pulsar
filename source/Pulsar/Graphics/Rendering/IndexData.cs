using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public sealed class IndexData
    {
        #region Fields

        internal IndexBufferObject indexBuffer;

        #endregion

        #region Properties

        public IndexBufferObject IndexBuffer
        {
            get { return this.indexBuffer; }
            set { this.indexBuffer = value; }
        }

        internal IndexBuffer Buffer
        {
            get { return this.indexBuffer.Buffer; }
        }

        #endregion
    }
}
