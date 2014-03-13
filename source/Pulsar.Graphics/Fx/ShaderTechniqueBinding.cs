using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a specific technique used by a material
    /// </summary>
    public sealed class ShaderTechniqueBinding : IDisposable
    {
        #region Fields

        private Shader _shader;
        private ShaderTechniqueDefinition _technique;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderTechniqueBinding class
        /// </summary>
        /// <param name="shader">Shader that contains the technique</param>
        /// <param name="technique">Technique</param>
        internal ShaderTechniqueBinding(Shader shader, ShaderTechniqueDefinition technique)
        {
            _shader = shader;
            _technique = technique;
            MaterialConstantsBinding = shader.CreateConstantsBinding(UpdateFrequency.Material);
            InstanceConstantsBinding = shader.CreateConstantsBinding(UpdateFrequency.Instance);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            try
            {
                MaterialConstantsBinding.Clear();
                InstanceConstantsBinding.Clear();
            }
            finally
            {
                MaterialConstantsBinding = null;
                InstanceConstantsBinding = null;
                _shader = null;
                _technique = null;
            }
        }

        /// <summary>
        /// Sets this technique as the current
        /// </summary>
        internal void UseTechnique()
        {
            _shader.SetCurrentTechnique(_technique);
        }

        /// <summary>
        /// Gets an enumerator to a collection of pass for the technique
        /// </summary>
        /// <returns>Returns a pass enumerator</returns>
        internal ShaderPassEnumerator GetPassEnumerator()
        {
            return new ShaderPassEnumerator(_technique);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of constants binding used for a global update
        /// </summary>
        public ShaderConstantBindingCollection GlobalConstantsBinding
        {
            get { return _shader.GlobalConstantsBinding; }
        }

        /// <summary>
        /// Gets the collection of constants binding used for material update
        /// </summary>
        public ShaderConstantBindingCollection MaterialConstantsBinding { get; private set; }

        /// <summary>
        /// Gets the collection of constants binding used for instance update
        /// </summary>
        public ShaderConstantBindingCollection InstanceConstantsBinding { get; private set; }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name
        {
            get { return _technique.Name; }
        }

        /// <summary>
        /// Gets a value that indicate if the technique use transparency
        /// </summary>
        public bool IsTransparent
        {
            get { return _technique.IsTransparent; }
        }

        #endregion
    }
}
