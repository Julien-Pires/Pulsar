using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Game;
using Pulsar.Core;

namespace Pulsar.Assets.Graphics.Materials
{
    /// <summary>
    /// Class containing informations about a 2D texture
    /// </summary>
    public class Texture : Asset
    {
        #region Fields

        private int width;
        private int height;
        private Texture2D tex2D = null;
        private SamplerState samplerState = SamplerState.AnisotropicWrap;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Texture class
        /// </summary>
        /// <param name="owner">Creator of this instance</param>
        /// <param name="name">Name of this instance</param>
        internal Texture(TextureManager owner, string name): base(name)
        {
            this.assetManager = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract the informations of a Texture2D
        /// </summary>
        private void ExtractInfo()
        {
            if (this.tex2D == null)
            {
                return;
            }

            this.width = this.tex2D.Width;
            this.height = this.tex2D.Height;
        }

        /// <summary>
        /// Create a checkerboard texture
        /// </summary>
        /// <param name="size">Size of the texture</param>
        /// <param name="stripSize">Size of the stripe</param>
        /// <param name="odd">Odd color</param>
        /// <param name="even">Even color</param>
        /// <returns>Return a new texture</returns>
        public static Texture2D CreateMissingTexture(int size, int stripSize, Color odd, Color even)
        {
            Color[] texMap = new Color[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    bool hasColor = (((x & stripSize) == 0) ^ ((y & stripSize) == 0));
                    int idx = (x * size) + y;

                    if (hasColor)
                    {
                        texMap[idx] = odd;
                    }
                    else
                    {
                        texMap[idx] = even;
                    }
                }
            }

            GraphicsDeviceManager gDeviceMngr = GameApplication.GameServices.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;
            if (gDeviceMngr == null)
            {
                throw new ArgumentException("GraphicsDeviceManager cannot be found");
            }
            Texture2D tex2D = new Texture2D(gDeviceMngr.GraphicsDevice, size, size);
            tex2D.SetData<Color>(texMap);

            return tex2D;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the Texture2D associated to this instance
        /// </summary>
        public Texture2D Image
        {
            get { return this.tex2D; }

            internal set 
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Texture2D is null");
                }

                this.tex2D = value;
                this.ExtractInfo();
            }
        }

        /// <summary>
        /// Get the filename of the Texture2D associated to this instance
        /// </summary>
        public string File { get; internal set; }

        /// <summary>
        /// Get the SamplerState
        /// </summary>
        public SamplerState Sampler
        {
            get { return this.samplerState; }
            internal set { this.samplerState = value; }
        }

        #endregion
    }
}
