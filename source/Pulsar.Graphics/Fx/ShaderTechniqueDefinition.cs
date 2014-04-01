using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Describes a shader technique
    /// </summary>
    public sealed class ShaderTechniqueDefinition
    {
        #region Fields

        private Dictionary<string, ShaderPassDefinition> _passesMap = new Dictionary<string, ShaderPassDefinition>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of ShaderTechniqueDefinition class
        /// </summary>
        /// <param name="name">Name of the technique</param>
        /// <param name="technique">Effect technique</param>
        internal ShaderTechniqueDefinition(string name, EffectTechnique technique)
        {
            Name = name;
            Technique = technique;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying effect technique
        /// </summary>
        internal EffectTechnique Technique { get; private set; }

        internal Dictionary<string, ShaderPassDefinition> Passes
        {
            get { return _passesMap; }
        }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value that indicate if the technique is used as fallback
        /// </summary>
        public bool IsFallback { get; internal set; }

        /// <summary>
        /// Gets a value that indicates if the technique use transparency
        /// </summary>
        public bool IsTransparent { get; internal set; }

        /// <summary>
        /// Gets a value that indicates if the technique is used for instancing
        /// </summary>
        public bool IsInstancing { get; internal set; }

        #endregion
    }
}
