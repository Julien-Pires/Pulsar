using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Defines the base implementation of a texture wrapper
    /// </summary>
    /// <typeparam name="TTexture">Type of texture</typeparam>
    internal abstract class TextureWrapper<TTexture> : ITextureWrapper where TTexture : XnaTexture
    {
        #region Fields

        protected TTexture InternalTexture;

        private bool _isDisposed;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TextureWrapper class
        /// </summary>
        /// <param name="texture">Raw texture</param>
        protected TextureWrapper(TTexture texture)
        {
            Debug.Assert(texture != null);

            InternalTexture = texture;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            InternalTexture.Dispose();
            _isDisposed = true;
        }

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        public abstract void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct;

        /// <summary>
        /// Sets data of the texture for a specific area
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="mipmapLevel">MipMap level</param>
        /// <param name="top">Top position</param>
        /// <param name="bottom">Bottom position</param>
        /// <param name="left">Left position</param>
        /// <param name="right">Right position</param>
        /// <param name="front">Front position</param>
        /// <param name="back">Back position</param>
        public abstract void SetData<T>(T[] data, int startIndex, int elementCount, int top, int bottom,
            int left, int right, int front, int back, int mipmapLevel) where T : struct;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of the texture
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// Gets the height of the texture
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// Gets the depth of the texture
        /// </summary>
        public abstract int Depth { get; }

        /// <summary>
        /// Gets the underlying texture
        /// </summary>
        public XnaTexture Texture
        {
            get { return InternalTexture; }
        }

        #endregion
    }

    /// <summary>
    /// Represents a wrapper for a Texture2D
    /// </summary>
    internal sealed class Texture2DWrapper : TextureWrapper<Texture2D>
    {
        #region Constructor

        /// <summary>
        /// Constructor of Texture2DWrapper
        /// </summary>
        /// <param name="texture">Raw texture</param>
        public Texture2DWrapper(Texture2D texture) : base(texture)
        {
            Debug.Assert(texture != null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        public override void SetData<T>(T[] data, int startIndex, int elementCount)
        {
            InternalTexture.SetData(data, startIndex, elementCount);
        }

        /// <summary>
        /// Sets data of the texture for a specific area
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="mipmapLevel">MipMap level</param>
        /// <param name="top">Top position</param>
        /// <param name="bottom">Bottom position</param>
        /// <param name="left">Left position</param>
        /// <param name="right">Right position</param>
        /// <param name="front">Front position</param>
        /// <param name="back">Back position</param>
        public override void SetData<T>(T[] data, int startIndex, int elementCount, int top, int bottom,
            int left, int right, int front, int back, int mipmapLevel)
        {
            Rectangle? source = new Rectangle(left, top, right - left, top - bottom);
            InternalTexture.SetData(mipmapLevel, source, data, startIndex, elementCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of the texture
        /// </summary>
        public override int Width
        {
            get { return InternalTexture.Width; }
        }

        /// <summary>
        /// Gets the height of the texture
        /// </summary>
        public override int Height
        {
            get { return InternalTexture.Height; }
        }

        /// <summary>
        /// Gets the depth of the texture
        /// </summary>
        public override int Depth
        {
            get { return 0; }
        }

        #endregion
    }

    /// <summary>
    /// Represents a wrapper for a Texture2D
    /// </summary>
    internal sealed class Texture3DWrapper : TextureWrapper<Texture3D>
    {
        #region Constructor

        /// <summary>
        /// Constructor of Texture3DWrapper
        /// </summary>
        /// <param name="texture">Raw texture</param>
        public Texture3DWrapper(Texture3D texture) : base(texture)
        {
            Debug.Assert(texture != null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        public override void SetData<T>(T[] data, int startIndex, int elementCount)
        {
            InternalTexture.SetData(data, startIndex, elementCount);
        }

        /// <summary>
        /// Sets data of the texture for a specific area
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="mipmapLevel">MipMap level</param>
        /// <param name="top">Top position</param>
        /// <param name="bottom">Bottom position</param>
        /// <param name="left">Left position</param>
        /// <param name="right">Right position</param>
        /// <param name="front">Front position</param>
        /// <param name="back">Back position</param>
        public override void SetData<T>(T[] data, int startIndex, int elementCount, int top, int bottom,
            int left, int right, int front, int back, int mipmapLevel)
        {
            InternalTexture.SetData(mipmapLevel, left, top, right, bottom, front, back, data, startIndex, elementCount);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of the texture
        /// </summary>
        public override int Width
        {
            get { return InternalTexture.Width; }
        }

        /// <summary>
        /// Gets the height of the texture
        /// </summary>
        public override int Height
        {
            get { return InternalTexture.Height; }
        }

        /// <summary>
        /// Gets the depth of the texture
        /// </summary>
        public override int Depth
        {
            get { return InternalTexture.Depth; }
        }

        #endregion
    }
}
