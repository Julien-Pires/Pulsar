using Pulsar.Graphics.Fx;

namespace Pulsar.Pipeline.Graphics
{
    /// <summary>
    /// Stores design-time data for a shader variable
    /// </summary>
    public sealed class ShaderVariableContent
    {
        #region Constructors

        /// <summary>
        /// Constructor of ShaderVariableContent class
        /// </summary>
        /// <param name="name">Name of the variable</param>
        internal ShaderVariableContent(string name)
        {
            Name = name;
            Semantic = string.Empty;
            EquivalentType = string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the update source
        /// </summary>
        public ShaderVariableSource Source { get; internal set; }

        /// <summary>
        /// Gets the update frequency
        /// </summary>
        public ShaderVariableUsage Usage { get; internal set; }
        
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
