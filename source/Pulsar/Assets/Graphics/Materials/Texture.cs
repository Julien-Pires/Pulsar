using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Core;

namespace Pulsar.Assets.Graphics.Materials
{
    /// <summary>
    /// Class containing informations about a 2D texture
    /// </summary>
    public class Texture : Asset
    {
        #region Fields

        private static readonly int missingSize = 256;
        private static readonly int missingStripSize = 0x8;
        private static readonly Color missingColor = Color.Blue;

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
        /// <returns>Return a new texture</returns>
        public static Texture2D CreateMissingTexture()
        {
            Color[] texMap = new Color[Texture.missingSize * Texture.missingSize];

            for (int x = 0; x < Texture.missingSize; x++)
            {
                for (int y = 0; y < Texture.missingSize; y++)
                {
                    bool hasColor = (((x & Texture.missingStripSize) == 0) ^ ((y & Texture.missingStripSize) == 0));
                    int idx = (x * Texture.missingSize) + y;

                    if (hasColor)
                    {
                        texMap[idx] = Texture.missingColor;
                    }
                    else
                    {
                        texMap[idx] = Color.White;
                    }
                }
            }

            Texture2D tex2D = new Texture2D(GameApplication.GameGraphicsDevice, Texture.missingSize, Texture.missingSize);
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
