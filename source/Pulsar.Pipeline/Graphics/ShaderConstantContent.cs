using Pulsar.Graphics.Fx;

namespace Pulsar.Pipeline.Graphics
{
    /// <summary>
    /// Stores design-time data for a shader constant
    /// </summary>
    public sealed class ShaderConstantContent
    {
        #region Constructors

        /// <summary>
        /// Constructor of ShaderConstantContent class
        /// </summary>
        /// <param name="name">Name of the constant</param>
        internal ShaderConstantContent(string name)
        {
            Name = name;
            Semantic = string.Empty;
            EquivalentType = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the update source
        /// </summary>
        public ShaderConstantSource Source { get; internal set; }

        /// <summary>
        /// Gets the update frequency
        /// </summary>
        public UpdateFrequency UpdateFrequency { get; internal set; }
        
        /// <summary>
        /// Gets the semantic
        /// </summary>
        public string Semantic { get; internal set; }

        /// <summary>
        /// Gets the managed type used instead of the native type
        /// </summary>
        public string EquivalentType { get; internal set; }

        #endregion
    }
}
