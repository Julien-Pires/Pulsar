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
        /// <param name="name">Name of the constant</param>
        /// <param name="parameter">Effect parameter</param>
        /// <param name="type">Constant type</param>
        internal ShaderConstantDefinition(string name, EffectParameter parameter, Type type)
        {
            Name = name;
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
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type
        /// </summary>
        public Type Type { get; private set; }

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