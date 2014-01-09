using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    public sealed class TextureParameters : Freezable
    {
        #region Fields

        private string _filename;

        #endregion

        #region Constructor

        public TextureParameters()
        {
            Source = AssetSource.NewInstance;
            Level = TextureLevel.Texture2D;
            Format = SurfaceFormat.Color;
        }

        #endregion

        #region Static methods

        public static TextureParameters FromTexture2DFile(string filename)
        {
            TextureParameters texParams = new TextureParameters
            {
                Filename = filename,
                Level = TextureLevel.Texture2D
            };

            return texParams;
        }

        public static TextureParameters FromTexture3DFile(string filename)
        {
            TextureParameters texParams = new TextureParameters
            {
                Filename = filename,
                Level = TextureLevel.Texture3D
            };

            return texParams;
        }

        #endregion

        #region Properties

        public AssetSource Source { get; private set; }

        public string Filename
        {
            get { return _filename; }
            set 
            {
                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        public TextureLevel Level { get; set; }

        public SurfaceFormat Format { get; set; }

        public bool MipMap { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        #endregion
    }
}
