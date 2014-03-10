using System;

namespace Pulsar.Graphics.Fx
{
    public sealed class KeyedBinding<T> : ShaderVariableBinding<T>
    {
        #region Fields

        private readonly string _key;

        #endregion

        #region Constructors

        public KeyedBinding(ShaderVariableDefinition definition)
            : base(definition.Parameter)
        {
            if(string.IsNullOrEmpty(definition.Semantic))
                throw new ArgumentNullException("definition", "Semantic key cannot be null or empty");

            _key = definition.Semantic;
        }

        #endregion

        #region Methods

        public override void Update(FrameContext context)
        {
            InternalValue = context.Renderable.Material.UnsafeGetValue<T>(_key);
        }

        #endregion

        #region Properties

        public string Key
        {
            get { return _key; }
        }

        #endregion
    }
}
