using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Assets.Graphics.Materials
{
    /// <summary>
    /// Class describing a material composed of many informations for rendering pass
    /// </summary>
    public sealed class Material : Asset
    {
        #region Fields

        private float specularPower = 1.0f;
        private float specularIntensity = 50.0f;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Material class
        /// </summary>
        /// <param name="owner">Creator of this instance</param>
        /// <param name="name">Name of this instance</param>
        internal Material(MaterialManager owner, string name) : base(name) 
        {
            this.assetManager = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set a boolean to disable backface culling
        /// </summary>
        public bool TwoSide { get; set; }

        /// <summary>
        /// Get or set a boolean to draw in wireframe mode
        /// </summary>
        public bool WireFrame { get; set; }

        /// <summary>
        /// Get or set a boolean indicating this material is transparent
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// Get or set the diffuse map
        /// </summary>
        public Texture DiffuseMap { get; set; }

        /// <summary>
        /// Get or set the specular map
        /// </summary>
        public Texture SpecularMap { get; set; }

        /// <summary>
        /// Get or set the normal map
        /// </summary>
        public Texture NormalMap { get; set; }

        /// <summary>
        /// Get or set the diffuse color
        /// </summary>
        public Color DiffuseColor { get; set; }

        /// <summary>
        /// Get or set the specular power
        /// </summary>
        public float SpecularPower
        {
            get { return this.specularPower; }
            set
            {
                if (value < 0.0f)
                {
                    this.specularPower = 0.0f;
                }
                else
                {
                    this.specularPower = value;
                }
            }
        }

        /// <summary>
        /// Get or set the specular intensity
        /// </summary>
        public float SpecularIntensity
        {
            get { return this.specularIntensity; }
            set
            {
                if (value < 0.0f)
                {
                    this.specularIntensity = 0.0f;
                }
                else
                {
                    this.specularIntensity = value;
                }
            }
        }

        #endregion
    }
}
