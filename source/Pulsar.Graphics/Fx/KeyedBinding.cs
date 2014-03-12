using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader variable binding that use a key to retrieve its value on a material instance
    /// </summary>
    /// <typeparam name="T">Variable type</typeparam>
    public sealed class KeyedBinding<T> : ShaderVariableBinding<T>
    {
        #region Fields

        private readonly string _key;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of KeyedBinding class
        /// </summary>
        /// <param name="definition">Variable definition</param>
        public KeyedBinding(ShaderVariableDefinition definition)
            : base(definition)
        {
            if(string.IsNullOrEmpty(definition.Semantic))
                throw new ArgumentNullException("definition", "Semantic key cannot be null or empty");

            _key = definition.Semantic;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the variable value
        /// </summary>
        /// <param name="context">Frame context</param>
        public override void Update(FrameContext context)
        {
            InternalValue = context.Renderable.Material.UnsafeGetValue<T>(_key);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the key
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        #endregion
    }
}
