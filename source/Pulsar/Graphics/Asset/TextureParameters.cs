using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Defines parameters to load a Texture instance
    /// </summary>
    public sealed class TextureParameters : Freezable
    {
        #region Fields

        private string _filename;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TextureParameters instance
        /// </summary>
        public TextureParameters()
        {
            Source = AssetSource.NewInstance;
            Level = TextureLevel.Texture2D;
            Format = SurfaceFormat.Color;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates parameters to load a 2D texture from a file
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <returns>Returns a TextureParameters instance</returns>
        public static TextureParameters FromTexture2DFile(string filename)
        {
            TextureParameters texParams = new TextureParameters
            {
                Filename = filename,
                Level = TextureLevel.Texture2D
            };

            return texParams;
        }

        /// <summary>
        /// Creates parameters to load a 3D texture from a file
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <returns>Returns a TextureParameters instance</returns>
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

        /// <summary>
        /// Gets the source of the material
        /// </summary>
        public AssetSource Source { get; private set; }

        /// <summary>
        /// Gets or sets the name of the file
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set 
            {
                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of texture to load
        /// </summary>
        public TextureLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the pixel format
        /// </summary>
        public SurfaceFormat Format { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates to generates a mipmap
        /// </summary>
        public bool MipMap { get; set; }

        /// <summary>
        /// Gets or sets the width of the texture
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the texture
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the depth of the texture
        /// </summary>
        public int Depth { get; set; }

        #endregion
    }
}
