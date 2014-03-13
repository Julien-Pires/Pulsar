using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a shader constant binding that use a key to retrieve its value on a material instance
    /// </summary>
    /// <typeparam name="T">Constant type</typeparam>
    public sealed class KeyedBinding<T> : ShaderConstantBinding<T>
    {
        #region Fields

        private readonly string _key;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of KeyedBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal KeyedBinding(ShaderConstantDefinition definition)
            : base(definition)
        {
            if(string.IsNullOrEmpty(definition.Semantic))
                throw new ArgumentNullException("definition", "Semantic key cannot be null or empty");

            _key = definition.Semantic;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the value
        /// </summary>
        /// <param name="context">Frame context</param>
        internal override void Update(FrameContext context)
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
