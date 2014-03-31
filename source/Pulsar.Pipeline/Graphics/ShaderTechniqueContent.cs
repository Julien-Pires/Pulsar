using System.Collections.Generic;

namespace Pulsar.Pipeline.Graphics
{
    /// <summary>
    /// Stores design-time data for a shader technique
    /// </summary>
    public sealed class ShaderTechniqueContent
    {
        #region Fields

        private readonly List<ShaderPassContent> _passes = new List<ShaderPassContent>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderTechniqueContent class
        /// </summary>
        /// <param name="name">Name of the technique</param>
        internal ShaderTechniqueContent(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value that indicate if the technique use transparency
        /// </summary>
        public bool IsTransparent { get; set; }

        public List<ShaderPassContent> Passes
        {
            get { return _passes; }
        }

        #endregion
    }
}
