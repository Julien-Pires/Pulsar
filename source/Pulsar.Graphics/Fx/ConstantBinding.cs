namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader constant binding to a constant value
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

        public new T Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }

        #endregion
    }
}
