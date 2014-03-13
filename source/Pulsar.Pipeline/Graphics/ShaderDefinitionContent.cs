using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Pulsar.Pipeline.Graphics
{
    /// <summary>
    /// Stores design-time data for a shader definition
    /// </summary>
    public sealed class ShaderDefinitionContent
    {
        #region Fields

        private readonly ShaderConstantCollection _constants = new ShaderConstantCollection();
        private readonly ShaderTechniqueCollection _techniques = new ShaderTechniqueCollection();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderDefinitionContent class
        /// </summary>
        internal ShaderDefinitionContent()
        {
            Instancing = string.Empty;
            Fallback = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an external reference to the associated effect file
        /// </summary>
        public ExternalReference<EffectContent> EffectFile { get; internal set; }

        /// <summary>
        /// Gets a list of constant definition
        /// </summary>
        public ShaderConstantCollection Constants
        {
            get { return _constants; }
        }

        /// <summary>
        /// Gets a list of technique definition
        /// </summary>
        public ShaderTechniqueCollection Techniques
        {
            get { return _techniques; }
        }

        /// <summary>
        /// Gets the name of the instancing technique
        /// </summary>
        public string Instancing { get; internal set; }

        /// <summary>
        /// Gets the name of the fallback technique
        /// </summary>
        public string Fallback { get; internal set; }

        #endregion
    }
}
