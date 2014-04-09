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

        internal TechniqueBinding(Shader shader, string technique, ushort materialId) 
            : this(shader, shader.GetTechniqueDefinition(technique), materialId)
        {
        }

        /// <summary>
        /// Constructor of TechniqueBinding class
        /// </summary>
        /// <param name="shader">Shader that contains the technique</param>
        /// <param name="technique">Technique</param>
        internal TechniqueBinding(Shader shader, TechniqueDefinition technique, ushort materialId)
        {
            _shader = shader;
            Definition = technique;

            PassDefinition[] passes = technique.Passes;
            PassesBindings = new PassBinding[passes.Length];
            for(int i = 0; i < PassesBindings.Length; i++)
                PassesBindings[i] = new PassBinding(passes[i], materialId);

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
        /// Gets an enumerator to a collection of pass for the technique
        /// </summary>
        /// <returns>Returns a pass enumerator</returns>
        internal PassBindingEnumerator GetPassEnumerator()
        {
            return new PassBindingEnumerator(this);
        }

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

        internal TechniqueDefinition Definition { get; private set; }

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
        public ShaderConstantBindingCollection InstanceConstantsBinding { get; private set; }

        /// <summary>
        /// Gets the name of the technique
        /// </summary>
        public string Name
        {
            get { return Definition.Name; }
        }

        /// <summary>
        /// Gets a value that indicate if the technique use transparency
        /// </summary>
        public bool IsTransparent
        {
            get { return Definition.IsTransparent; }
        }

        #endregion
    }
}
