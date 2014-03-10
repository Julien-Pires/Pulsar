using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class ConstantBinding<T> : ShaderVariableBinding<T>
    {
        #region Constructors

        public ConstantBinding(ShaderVariableDefinition definition, T value)
            : base(definition.Parameter)
        {
            InternalValue = value;
        }

        #endregion
    }
}
