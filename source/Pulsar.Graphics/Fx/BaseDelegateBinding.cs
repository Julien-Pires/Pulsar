using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader constant binding that use a delegate to update its value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDelegateBinding<T> : ShaderConstantBinding<T>
    {
        #region Fields

        /// <summary>
        /// Update method
        /// </summary>
        protected Func<FrameContext, T> InternalUpdateFunction;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BaseDelegateBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal BaseDelegateBinding(ShaderConstantDefinition definition)
            : base(definition)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the value
        /// </summary>
        /// <param name="context">Frame context</param>
        internal override void Update(FrameContext context)
        {
            InternalValue = InternalUpdateFunction(context);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the function used to update the value
        /// </summary>
        public Func<FrameContext, T> UpdateFunction
        {
            get { return InternalUpdateFunction; }
        }

        #endregion
    }
}
