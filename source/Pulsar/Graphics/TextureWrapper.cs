using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    internal abstract class TextureWrapper<TTexture> : ITextureWrapper where TTexture : XnaTexture
    {
        #region Fields

        protected TTexture InternalTexture;

        private bool _isDisposed;

        #endregion

        #region Constructor

        protected TextureWrapper(TTexture texture)
        {
            Debug.Assert(texture != null);

            InternalTexture = texture;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (_isDisposed) return;

            InternalTexture.Dispose();
            _isDisposed = true;
        }

        public abstract void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct;

        public abstract void SetData<T>(T[] data, int startIndex, int elementCount, int left, int right, int top, int bottom, 
            int front, int back, int mipmapLevel) where T : struct;

        #endregion

        #region Properties

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract int Depth { get; }

        public XnaTexture Texture
        {
            get { return InternalTexture; }
        }

        #endregion
    }

    internal sealed class Texture2DWrapper : TextureWrapper<Texture2D>
    {
        #region Constructor

        public Texture2DWrapper(Texture2D texture) : base(texture)
        {
            Debug.Assert(texture != null);
        }

        #endregion

        #region Methods

        public override void SetData<T>(T[] data, int startIndex, int elementCount)
        {
            InternalTexture.SetData(data, startIndex, elementCount);
        }

        public override void SetData<T>(T[] data, int startIndex, int elementCount, int left, int right, int top, int bottom, 
            int front, int back, int mipmapLevel)
        {
            Rectangle? source = new Rectangle(left, top, right - left, top - bottom);
            InternalTexture.SetData(mipmapLevel, source, data, startIndex, elementCount);
        }

        #endregion

        #region Properties

        public override int Width
        {
            get { return InternalTexture.Width; }
        }

        public override int Height
        {
            get { return InternalTexture.Height; }
        }

        public override int Depth
        {
            get { return 0; }
        }

        #endregion
    }

    internal sealed class Texture3DWrapper : TextureWrapper<Texture3D>
    {
        #region Constructor

        public Texture3DWrapper(Texture3D texture) : base(texture)
        {
            Debug.Assert(texture != null);
        }

        #endregion

        #region Methods

        public override void SetData<T>(T[] data, int startIndex, int elementCount)
        {
            InternalTexture.SetData(data, startIndex, elementCount);
        }

        public override void SetData<T>(T[] data, int startIndex, int elementCount, int left, int right, int top, int bottom, 
            int front, int back, int mipmapLevel)
        {
            InternalTexture.SetData(mipmapLevel, left, top, right, bottom, front, back, data, startIndex, elementCount);
        }

        #endregion

        #region Properties

        public override int Width
        {
            get { return InternalTexture.Width; }
        }

        public override int Height
        {
            get { return InternalTexture.Height; }
        }

        public override int Depth
        {
            get { return InternalTexture.Depth; }
        }

        #endregion
    }
}
