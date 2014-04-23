using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a specific technique used by a material
    /// </summary>
    public sealed class TechniqueBinding : IDisposable
    {
        #region Fields

        private Shader _shader;

        #endregion

        #region Constructors

        internal TechniqueBinding(Shader shader, string technique) 
            : this(shader, shader.GetTechniqueDefinition(technique))
        {
        }

        /// <summary>
        /// Constructor of TechniqueBinding class
        /// </summary>
        /// <param name="shader">Shader that contains the technique</param>
        /// <param name="technique">Technique</param>
        internal TechniqueBinding(Shader shader, TechniqueDefinition technique)
        {
            _shader = shader;
            Definition = technique;

            PassDefinition[] passes = technique.Passes;
            PassesBindings = new PassBinding[passes.Length];
            for(int i = 0; i < PassesBindings.Length; i++)
                PassesBindings[i] = new PassBinding(passes[i]);

            MaterialConstantsBinding = shader.CreateConstantsBinding(UpdateFrequency.Material);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            try
            {
                MaterialConstantsBinding.Clear();
            }
            finally
            {
                MaterialConstantsBinding = null;
                _shader = null;
                Definition = null;
            }
        }

        /// <summary>
        /// Sets this technique as the current
        /// </summary>
        internal void UseTechnique()
        {
            _shader.SetCurrentTechnique(Definition);
        }

        /// <summary>
        /// Sets the value for a specified constant
        /// </summary>
        /// <param name="constant">Name of the constant</param>
        /// <param name="value">Value</param>
        internal void TrySetConstantValue(string constant, object value)
        {
            ShaderConstantDefinition definition = _shader.GetConstantDefinition(constant);
            if(definition == null)
                return;

            ShaderConstantBinding binding = null;
            switch (definition.UpdateFrequency)
            {
                case UpdateFrequency.Material:
                    binding = MaterialConstantsBinding.GetBinding(constant);
                    break;

                case UpdateFrequency.Instance:
                    binding = InstanceConstantsBinding.GetBinding(constant);
                    break;

                case UpdateFrequency.Global:
                    binding = GlobalConstantsBinding.GetBinding(constant);
                    break;
            }

            if(binding == null)
                return;

            binding.UntypedValue = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name
        {
            get { return Definition.Name; }
        }

        /// <summary>
        /// Gets the technique definition
        /// </summary>
        public TechniqueDefinition Definition { get; private set; }

        /// <summary>
        /// Gets the list of pass binding
        /// </summary>
        internal PassBinding[] PassesBindings { get; private set; }

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
        public ShaderConstantBindingCollection InstanceConstantsBinding
        {
            get { return _shader.InstanceConstantsBinding; }
        }

        #endregion
    }
}
