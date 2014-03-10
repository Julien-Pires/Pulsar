using System;
using Pulsar.Extension;

namespace Pulsar.Graphics.Fx
{
    public sealed class AutomaticBinding<T> : BaseDelegateBinding<T>
    {
        #region Constructors

        internal AutomaticBinding(ShaderVariableDefinition definition)
            : base(definition.Parameter)
        {
            if (string.IsNullOrEmpty(definition.Semantic))
                throw new ArgumentNullException("definition", "Semantic cannot be null or empty");

            ShaderVariableSemantic semantic;
            if(!EnumExtension.TryParse(definition.Semantic, true, out semantic))
                throw new Exception(string.Format("{0} is not a supported semantic for automatic binding", definition.Semantic));

            InternalUpdateFunction = AutomaticDelegateMapper.GetMethod<T>((int) semantic);
            Semantic = semantic;
        }

        #endregion

        #region Properties

        public ShaderVariableSemantic Semantic { get; private set; }

        #endregion
    }
}
