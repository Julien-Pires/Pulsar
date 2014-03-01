using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Enumerates type of texture
    /// </summary>
    public enum TextureLevel
    {
        Texture,
        Texture2D,
        Texture3D
    }

    /// <summary>
    /// Contains information about a texture
    /// </summary>
    public class Texture : ICastable<XnaTexture>, ICastable<Texture2D>, ICastable<Texture3D>,
        IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private bool _isDirty;
        private int _width;
        private int _height;
        private int _depth;
        private bool _mipMap;
        private readonly SurfaceFormat _format;
        private readonly TextureLevel _textureLevel;
        private ITextureWrapper _textureWrapper;
        private readonly GraphicsDeviceManager _deviceManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Texture class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager</param>
        /// <param name="name">Name of the texture</param>
        /// <param name="texture">Raw texture</param>
        internal Texture(GraphicsDeviceManager deviceManager, string name, XnaTexture texture)
        {
            Debug.Assert(texture != null);

            _deviceManager = deviceManager;

            Name = name;
            _format = texture.Format;

            Texture2D texture2D = texture as Texture2D;
            _textureLevel = (texture2D != null) ? TextureLevel.Texture2D : TextureLevel.Texture3D;
            CreateWrapper(texture);
        }

        /// <summary>
        /// Constructor of Texture class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager</param>
        /// <param name="name">Name of the texture</param>
        /// <param name="level">Type of texture to use</param>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        /// <param name="depth">Depth of the texture</param>
        /// <param name="mipMap">MipMap</param>
        /// <param name="format">Pixel format</param>
        internal Texture(GraphicsDeviceManager deviceManager, string name, TextureLevel level, int width, 
            int height, int depth, bool mipMap, SurfaceFormat format)
        {
            _deviceManager = deviceManager;

            Name = name;
            _textureLevel = level;
            _format = format;
            MipMap = mipMap;
            SetSize(width, height, depth);
            CreateTexture();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Converts this instance to an Xna Texture
        /// </summary>
        /// <param name="texture">Texture instance</param>
        /// <returns>Returns an Xna Texture instance</returns>
        public static explicit operator XnaTexture(Texture texture)
        {
            return texture.GetBaseTexture();
        }

        /// <summary>
        /// Converts this instance to a Texture2D
        /// </summary>
        /// <param name="texture">Texture instance</param>
        /// <returns>Returns a Texture2D instance</returns>
        public static explicit operator Texture2D(Texture texture)
        {
            return texture.GetTexture2D();
        }

        /// <summary>
        /// Converts this instance to a Texture3D
        /// </summary>
        /// <param name="texture">Texture instance</param>
        /// <returns>Returns a Texture3D instance</returns>
        public static explicit operator Texture3D(Texture texture)
        {
            return texture.GetTexture3D();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates a checker texture
        /// </summary>
        /// <typeparam name="T">Pixel format</typeparam>
        /// <param name="texture">Texture to fill</param>
        /// <param name="stripSize">Size of the stripe</param>
        /// <param name="odd">Odd color</param>
        /// <param name="even">Even color</param>
        public static void CreateCheckerTexture<T>(Texture texture, int stripSize, T odd, T even) where T : struct
        {
            T[] data = new T[texture.Width * texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool hasColor = (((x & stripSize) == 0) ^ ((y & stripSize) == 0));
                    int idx = (x * texture.Width) + y;
                    if (hasColor)
                        data[idx] = odd;
                    else
                        data[idx] = even;
                }
            }
            texture.SetData(data);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                _textureWrapper.Dispose();
            }
            finally
            {
                _textureWrapper = null;
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Creates a texture wrapper
        /// </summary>
        /// <param name="texture">Raw texture</param>
        private void CreateWrapper(XnaTexture texture)
        {
            if (_textureWrapper != null)
            {
                _textureWrapper.Dispose();
                _textureWrapper = null;
            }

            Texture2D texture2D = texture as Texture2D;
            if(texture2D != null)
                _textureWrapper = new Texture2DWrapper(texture2D);
            else
                _textureWrapper = new Texture3DWrapper(texture as Texture3D);
        }

        /// <summary>
        /// Creates a texture instance
        /// </summary>
        private void CreateTexture()
        {
            XnaTexture texture = null;
            switch (_textureLevel)
            {
                case TextureLevel.Texture2D:
                    texture = new Texture2D(_deviceManager.GraphicsDevice, _width, _height, _mipMap, _format);
                    break;
                case TextureLevel.Texture3D:
                    texture = new Texture3D(_deviceManager.GraphicsDevice, _width, _height, _depth, _mipMap, _format);
                    break;
            }

            if(texture == null)
                throw new Exception("Failed to create a texture");

            CreateWrapper(texture);
        }

        /// <summary>
        /// Applies changes
        /// </summary>
        public void ApplyChanges()
        {
            if(!_isDirty) return;

            CreateTexture();
            _isDirty = false;
        }

        /// <summary>
        /// Sets the size of the texture
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void SetSize(int width, int height)
        {
            SetSize(width, height, 0);
        }

        /// <summary>
        /// Sets the size of the texture
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        public void SetSize(int width, int height, int depth)
        {
            if (width <= 0)
                throw new ArgumentException("width must be superior to zero");

            if (height <= 0)
                throw new ArgumentException("height must be superior to zero");

            if (_textureLevel == TextureLevel.Texture3D)
            {
                if (depth <= 0)
                    throw new ArgumentException("depth must be superior to zero");
            }

            _width = width;
            _height = height;
            _depth = depth;
            _isDirty = true;
        }

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        public void SetData<T>(T[] data) where T : struct
        {
            _textureWrapper.SetData(data, 0, data.Length);
        }

        /// <summary>
        /// Sets data of the texture
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount);
        }

        /// <summary>
        /// Sets data of the texture for a specified mipmap level
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="mipmapLevel">MipMap level</param>
        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, 0, Width, 0, Height, 0, 0, mipmapLevel);
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
        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel, int top, int bottom, 
            int left, int right) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, left, right, top, bottom, 0, 0, mipmapLevel);
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
        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel, int top, int bottom,
            int left, int right, int front, int back) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, left, right, top, bottom, front, back, mipmapLevel);   
        }

        /// <summary>
        /// Casts this instance to an Xna Texture
        /// </summary>
        /// <returns>Returns an Xna texture instance</returns>
        XnaTexture ICastable<XnaTexture>.Cast()
        {
            return (XnaTexture)this;
        }

        /// <summary>
        /// Casts this instance to a Texture2D
        /// </summary>
        /// <returns>Returns a Texture2D instance</returns>
        Texture2D ICastable<Texture2D>.Cast()
        {
            return (Texture2D)this;
        }

        /// <summary>
        /// Casts this instance to a Texture3D
        /// </summary>
        /// <returns>Returns a Texture3D instance</returns>
        Texture3D ICastable<Texture3D>.Cast()
        {
            return (Texture3D)this;
        }

        /// <summary>
        /// Gets the underlying Xna Texture
        /// </summary>
        /// <returns>Returns an Xna texture instance</returns>
        public XnaTexture GetBaseTexture()
        {
            return _textureWrapper.Texture;
        }

        /// <summary>
        /// Gets the underlying Texture2D
        /// </summary>
        /// <returns>Returns a Texture2D instance</returns>
        public Texture2D GetTexture2D()
        {
            return (Texture2D)_textureWrapper.Texture;
        }

        /// <summary>
        /// Gets the underlying Texture3D
        /// </summary>
        /// <returns>Returns a Texture3D instance</returns>
        public Texture3D GetTexture3D()
        {
            return (Texture3D)_textureWrapper.Texture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying texture
        /// </summary>
        internal XnaTexture InternalTexture
        {
            get { return _textureWrapper.Texture; }
        }

        /// <summary>
        /// Gets the name of the texture
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the width of the texture
        /// </summary>
        public int Width
        {
            get { return _textureWrapper.Width; }
        }

        /// <summary>
        /// Gets the height of the texture
        /// </summary>
        public int Height
        {
            get { return _textureWrapper.Height; }
        }

        /// <summary>
        /// Gets the depth of the texture
        /// </summary>
        public int Depth
        {
            get { return _textureWrapper.Depth; }
        }

        /// <summary>
        /// Gets the pixel format
        /// </summary>
        public SurfaceFormat Format
        {
            get { return _textureWrapper.Texture.Format; }
        }

        /// <summary>
        /// Gets or sets a value that indicate to generate a mipmap chain
        /// </summary>
        public bool MipMap
        {
            get { return _mipMap; }
            set
            {
                _mipMap = value;
                _isDirty = true;
            }
        }

        #endregion
    }
}
