using System;

namespace Pulsar.Graphics.Fx
{
    public sealed class DelegateBinding<T> : BaseDelegateBinding<T>
    {
        #region Constructors

        internal DelegateBinding(ShaderVariableDefinition definition)
            : base(definition.Parameter)
        {
            InternalUpdateFunction = DefaultUpdate;
        }

        #endregion

        #region Static methods

        private static T DefaultUpdate(FrameContext context)
        {
            return default(T);
        }

        #endregion

        #region Methods

        public void SetUpdateFunction(Func<FrameContext, T> context)
        {
            InternalUpdateFunction = context ?? DefaultUpdate;
        }

        #endregion
    }
}
