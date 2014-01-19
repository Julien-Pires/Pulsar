using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsar.Core;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics
{
    public enum TextureLevel
    {
        Texture,
        Texture2D,
        Texture3D
    }

    /// <summary>
    /// Class containing informations about a 2D texture
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

        public static explicit operator XnaTexture(Texture texture)
        {
            return texture.GetBaseTexture();
        }

        public static explicit operator Texture2D(Texture texture)
        {
            return texture.GetTexture2D();
        }

        public static explicit operator Texture3D(Texture texture)
        {
            return texture.GetTexture3D();
        }

        #endregion

        #region Methods

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
                throw new Exception("");

            CreateWrapper(texture);
        }

        public void ApplyChanges()
        {
            if(!_isDirty) return;

            CreateTexture();
            _isDirty = false;
        }

        public void SetSize(int width, int height)
        {
            SetSize(width, height, 0);
        }

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

        public void SetData<T>(T[] data) where T : struct
        {
            _textureWrapper.SetData(data, 0, data.Length);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, 0, Width, 0, Height, 0, 0, mipmapLevel);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel, int top, int bottom, 
            int left, int right) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, left, right, top, bottom, 0, 0, mipmapLevel);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount, int mipmapLevel, int left, int right, 
            int top, int bottom, int front, int back) where T : struct
        {
            _textureWrapper.SetData(data, startIndex, elementCount, left, right, top, bottom, front, back, mipmapLevel);   
        }

        XnaTexture ICastable<XnaTexture>.Cast()
        {
            return (XnaTexture)this;
        }

        Texture2D ICastable<Texture2D>.Cast()
        {
            return (Texture2D)this;
        }

        Texture3D ICastable<Texture3D>.Cast()
        {
            return (Texture3D)this;
        }

        public XnaTexture GetBaseTexture()
        {
            return _textureWrapper.Texture;
        }

        public Texture2D GetTexture2D()
        {
            return (Texture2D)_textureWrapper.Texture;
        }

        public Texture3D GetTexture3D()
        {
            return (Texture3D)_textureWrapper.Texture;
        }

        #endregion

        #region Properties

        internal XnaTexture InternalTexture
        {
            get { return _textureWrapper.Texture; }
        }

        public string Name { get; private set; }

        public int Width
        {
            get { return _textureWrapper.Width; }
        }

        public int Height
        {
            get { return _textureWrapper.Height; }
        }

        public int Depth
        {
            get { return _textureWrapper.Depth; }
        }

        public SurfaceFormat Format
        {
            get { return _textureWrapper.Texture.Format; }
        }

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
