namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader variable binding to a constant value
    /// </summary>
    /// <typeparam name="T">Variable type</typeparam>
    public sealed class ConstantBinding<T> : ShaderVariableBinding<T>
    {
        #region Constructors

        /// <summary>
        /// Constructor of ConstantBinding class
        /// </summary>
        /// <param name="definition">Variable definition</param>
        /// <param name="value">Constant</param>
        public ConstantBinding(ShaderVariableDefinition definition, T value)
            : base(definition)
        {
            InternalValue = value;
        }

        #endregion
    }
}
