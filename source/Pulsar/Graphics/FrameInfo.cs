using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains information about one frame
    /// </summary>
    public sealed class FrameInfo
    {
        #region Fields

        private int framerate;
        private TimeSpan elapsedTime;
        private int framecount;
        private int drawCall;
        private int subMeshCount;
        private int primitiveCount;
        private int vertexCount;

        #endregion

        #region Methods

        /// <summary>
        /// Update FrameInfo counter
        /// </summary>
        /// <param name="time">Time since the last update</param>
        internal void Update(GameTime time)
        {
            this.elapsedTime += time.ElapsedGameTime;
            if (this.elapsedTime > TimeSpan.FromSeconds(1.0))
            {
                this.elapsedTime -= TimeSpan.FromSeconds(1.0);
                this.framerate = this.framecount;
                this.framecount = 0;
            }
        }

        /// <summary>
        /// Reset the frame info
        /// </summary>
        internal void PrepareNewRendering()
        {
            this.drawCall = 0;
            this.subMeshCount = 0;
            this.primitiveCount = 0;
            this.vertexCount = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the number of frame drawn since the last reset
        /// </summary>
        internal int Framecount
        {
            get { return this.framecount; }
            set { this.framecount = value; }
        }

        /// <summary>
        /// Get the number of frame drawn by seconds
        /// </summary>
        public int Framerate
        {
            get { return this.framerate; }
        }

        /// <summary>
        /// Get the number of draw call
        /// </summary>
        public int DrawCall
        {
            get { return this.drawCall; }
            internal set { this.drawCall = value; }
        }

        /// <summary>
        /// Get the number of submesh drawn
        /// </summary>
        public int SubMeshCount
        {
            get { return this.subMeshCount; }
            internal set { this.subMeshCount = value; }
        }

        /// <summary>
        /// Get the number of primitive drawn
        /// </summary>
        public int PrimitiveCount
        {
            get { return this.primitiveCount; }
            internal set { this.primitiveCount = value; }
        }

        /// <summary>
        /// Get the number of vertices drawn
        /// </summary>
        public int VertexCount
        {
            get { return this.vertexCount; }
            internal set { this.vertexCount = value; }
        }

        #endregion
    }
}