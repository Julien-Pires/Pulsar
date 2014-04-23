using System;

using Pulsar.Extension;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a shader constant that retrieve the value automatically
    /// </summary>
    /// <typeparam name="T">Constant type</typeparam>
    public sealed class AutomaticBinding<T> : BaseDelegateBinding<T>
    {
        #region Constructors

        /// <summary>
        /// Constructor of AutomaticBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal AutomaticBinding(ShaderConstantDefinition definition)
            : base(definition)
        {
            if (string.IsNullOrEmpty(definition.Semantic))
                throw new ArgumentNullException("definition", "Semantic cannot be null or empty");

            ShaderConstantSemantic semantic;
            if (!EnumExtension.TryParse(definition.Semantic, true, out semantic))
                throw new Exception(string.Format("{0} is not a supported semantic for automatic binding",
                    definition.Semantic));

            InternalUpdateFunction = AutomaticDelegateMapper.GetMethod<T>(semantic);
            Semantic = semantic;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the semantic
        /// </summary>
        public ShaderConstantSemantic Semantic { get; private set; }

        #endregion
    }
}
