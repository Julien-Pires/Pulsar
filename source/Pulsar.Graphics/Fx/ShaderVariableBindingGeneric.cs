using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a strongly-typed shader variable binding
    /// </summary>
    /// <typeparam name="T">Variable type</typeparam>
    public abstract class ShaderVariableBinding<T> : ShaderVariableBinding
    {
        #region Fields

        private static readonly Action<ShaderVariableBinding<T>> WriteToParameter;

        internal T InternalValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of ShaderVariableBinding class
        /// </summary>
        static ShaderVariableBinding()
        {
            WriteToParameter = EffectParameterWriter.GetWriteMethod<T>();
        }

        /// <summary>
        /// Constructor of ShaderVariableBinding class
        /// </summary>
        /// <param name="definition">Variable definition</param>
        internal ShaderVariableBinding(ShaderVariableDefinition definition)
            : base(definition)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the value to the effect parameter
        /// </summary>
        internal override void Write()
        {
            WriteToParameter(this);
        }

        /// <summary>
        /// Updates the variable value
        /// </summary>
        /// <param name="context">Frame context</param>
        internal override void Update(FrameContext context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current value of the variable
        /// </summary>
        public T Value
        {
            get { return InternalValue; }
        }

        #endregion
    }
}
