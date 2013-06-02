using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
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

        internal void PrepareNewRendering()
        {
            this.drawCall = 0;
            this.subMeshCount = 0;
            this.primitiveCount = 0;
            this.vertexCount = 0;
        }

        #endregion

        #region Properties

        internal int Framecount
        {
            get { return this.framecount; }
            set { this.framecount = value; }
        }

        public int Framerate
        {
            get { return this.framerate; }
        }

        public int DrawCall
        {
            get { return this.drawCall; }
            internal set { this.drawCall = value; }
        }

        public int SubMeshCount
        {
            get { return this.subMeshCount; }
            internal set { this.subMeshCount = value; }
        }

        public int PrimitiveCount
        {
            get { return this.primitiveCount; }
            internal set { this.primitiveCount = value; }
        }

        public int VertexCount
        {
            get { return this.vertexCount; }
            internal set { this.vertexCount = value; }
        }

        #endregion
    }
}