using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    public sealed class Material
    {
        #region Fields

        private float _specularPower = 1.0f;
        private float _specularIntensity = 50.0f;

        #endregion

        #region Constructors

        internal Material(string name)
        {
            Name = name;
        }

        #endregion

        #region Methods

        public Material GetCopy()
        {
            Material material = new Material(Name)
            {
                TwoSide = TwoSide,
                WireFrame = WireFrame,
                IsTransparent = IsTransparent,
                DiffuseMap = DiffuseMap,
                SpecularMap = SpecularMap,
                NormalMap = NormalMap,
                DiffuseColor = DiffuseColor,
                SpecularPower = SpecularPower,
                SpecularIntensity = SpecularIntensity
            };

            return material;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

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
            get { return _specularPower; }
            set { _specularPower = (value < 0.0f) ? 0.0f : value; }
        }

        /// <summary>
        /// Get or set the specular intensity
        /// </summary>
        public float SpecularIntensity
        {
            get { return _specularIntensity; }
            set { _specularIntensity = (value < 0.0f) ? 0.0f : value; }
        }

        #endregion
    }
}
