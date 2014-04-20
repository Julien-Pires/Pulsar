using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Describes a shader constant
    /// </summary>
    public sealed class ShaderConstantDefinition
    {
        #region Constructor

        /// <summary>
        /// Constructor of ShaderConstantDefinition class
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <param name="type">Constant type</param>
        internal ShaderConstantDefinition(EffectParameter parameter, Type type)
        {
            Parameter = parameter;
            Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying effect parameter
        /// </summary>
        internal EffectParameter Parameter { get; private set; }

        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        public string Name
        {
            get { return Parameter.Name; }
        }

        /// <summary>
        /// Gets the type
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// Gets the semantic
        /// </summary>
        public string Semantic { get; internal set; }

        /// <summary>
        /// Gets the update frequency
        /// </summary>
        public UpdateFrequency UpdateFrequency { get; internal set; }

        /// <summary>
        /// Gets the source used to update the constant value
        /// </summary>
        public ShaderConstantSource Source { get; internal set; }

        #endregion
    }
}