using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader variable binding that use a delegate to update its value
    /// </summary>
    /// <typeparam name="T">Variable type</typeparam>
    public sealed class DelegateBinding<T> : BaseDelegateBinding<T>
    {
        #region Constructors

        /// <summary>
        /// Constructor of DelegateBinding class
        /// </summary>
        /// <param name="definition">Variable definition</param>
        internal DelegateBinding(ShaderVariableDefinition definition)
            : base(definition)
        {
            InternalUpdateFunction = DefaultUpdate;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Default method to update the variable
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
        /// Sets the delegate used to update the variable
        /// </summary>
        /// <param name="context">Frame context</param>
        public void SetUpdateFunction(Func<FrameContext, T> context)
        {
            InternalUpdateFunction = context ?? DefaultUpdate;
        }

        #endregion
    }
}
