namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a shader constant that need to be manually updated
    /// </summary>
    /// <typeparam name="T">Constant type</typeparam>
    public sealed class ConstantBinding<T> : ShaderConstantBinding<T>
    {
        #region Constructors

        /// <summary>
        /// Constructor of ConstantBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal ConstantBinding(ShaderConstantDefinition definition)
            : base(definition)
        {
            InternalValue = default(T);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of the constant
        /// </summary>
        public new T Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }

        #endregion
    }
}
