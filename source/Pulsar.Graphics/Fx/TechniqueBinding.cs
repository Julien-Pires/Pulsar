using System;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a specific technique used by a material
    /// </summary>
    public sealed class TechniqueBinding : IDisposable
    {
        #region Fields

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
            Shader = shader;
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
                Shader = null;
                Definition = null;
            }
        }

        /// <summary>
        /// Sets this technique as the current
        /// </summary>
        internal void UseTechnique()
        {
            Shader.SetCurrentTechnique(Definition);
        }

        internal void SetConstantValue<T>(string constant, T value)
        {
            ShaderConstantDefinition definition = Shader.GetConstantDefinition(constant);
            if (definition == null)
                return;

            ShaderConstantBindingCollection collection = null;
            switch (definition.UpdateFrequency)
            {
                case UpdateFrequency.Material:
                    collection = MaterialConstantsBinding;
                    break;

                case UpdateFrequency.Instance:
                    collection = InstanceConstantsBinding;
                    break;

                case UpdateFrequency.Global:
                    collection = GlobalConstantsBinding;
                    break;
            }

            if(collection == null)
                return;

            ShaderConstantBinding<T> binding = collection.GetBinding<ShaderConstantBinding<T>>(constant);
            if(binding == null)
                return;

            binding.Value = value;
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

        public Shader Shader { get; private set; }

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
            get { return Shader.GlobalConstantsBinding; }
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
            get { return Shader.InstanceConstantsBinding; }
        }

        #endregion
    }
}
