using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a specific technique used by a material
    /// </summary>
    internal sealed class ShaderTechniqueBinding : IDisposable
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
            MaterialBinding = shader.CreateVariableBinding(ShaderVariableUsage.Material);
            InstanceBinding = shader.CreateVariableBinding(ShaderVariableUsage.Instance);
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
                MaterialBinding.Clear();
                InstanceBinding.Clear();
            }
            finally
            {
                _shader = null;
                _technique = null;
                MaterialBinding = null;
                InstanceBinding = null;
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
        /// Gets the collection of variable binding used for a global update
        /// </summary>
        internal ShaderVariableBindingCollection GlobalBinding
        {
            get { return _shader.GlobalVariablesBinding; }
        }

        /// <summary>
        /// Gets the collection of variable binding used for material update
        /// </summary>
        internal ShaderVariableBindingCollection MaterialBinding { get; private set; }

        /// <summary>
        /// Gets the collection of variable binding used for instance update
        /// </summary>
        internal ShaderVariableBindingCollection InstanceBinding { get; private set; }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Technique
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
