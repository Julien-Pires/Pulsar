using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Describes a shader variable
    /// </summary>
    public sealed class ShaderVariableDefinition
    {
        #region Constructor

        /// <summary>
        /// Constructor of ShaderVariableDefinition class
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="parameter">Effect parameter</param>
        /// <param name="type">Variable type</param>
        internal ShaderVariableDefinition(string name, EffectParameter parameter, Type type)
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
        /// Gets the name of the variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the variable
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the semantic of the variable
        /// </summary>
        public string Semantic { get; internal set; }

        /// <summary>
        /// Gets the update frequency of the variable
        /// </summary>
        public ShaderVariableUsage Usage { get; internal set; }

        /// <summary>
        /// Gets the source used to update the variable value
        /// </summary>
        public ShaderVariableSource Source { get; internal set; }

        #endregion
    }
}