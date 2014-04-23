using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a strongly-typed shader constant binding
    /// </summary>
    /// <typeparam name="T">Constant type</typeparam>
    public abstract class ShaderConstantBinding<T> : ShaderConstantBinding
    {
        #region Fields

        private static readonly Action<ShaderConstantBinding<T>> WriteToParameter;

        protected internal T InternalValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of ShaderConstantBinding class
        /// </summary>
        static ShaderConstantBinding()
        {
            WriteToParameter = EffectParameterWriter.GetWriteMethod<T>();
        }

        /// <summary>
        /// Constructor of ShaderConstantBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal ShaderConstantBinding(ShaderConstantDefinition definition)
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
        /// Updates the value
        /// </summary>
        /// <param name="context">Frame context</param>
        internal override void Update(FrameContext context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current value
        /// </summary>
        public T Value
        {
            get { return InternalValue; }
        }

        /// <summary>
        /// Gets or sets the value of the constant
        /// </summary>
        internal override object UntypedValue
        {
            get { return InternalValue; }
            set { InternalValue = (T)value; }
        }

        #endregion
    }
}
