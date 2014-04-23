using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a shader constant that use a delegate to update its value
    /// </summary>
    /// <typeparam name="T">Constant type</typeparam>
    public sealed class DelegateBinding<T> : BaseDelegateBinding<T>
    {
        #region Constructors

        /// <summary>
        /// Constructor of DelegateBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal DelegateBinding(ShaderConstantDefinition definition)
            : base(definition)
        {
            InternalUpdateFunction = DefaultUpdate;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Default method to update the value
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the default value for the current type</returns>
        private static T DefaultUpdate(FrameContext context)
        {
            return default(T);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the delegate used to update the value
        /// </summary>
        /// <param name="context">Frame context</param>
        public void SetUpdateFunction(Func<FrameContext, T> context)
        {
            InternalUpdateFunction = context ?? DefaultUpdate;
        }

        #endregion
    }
}
