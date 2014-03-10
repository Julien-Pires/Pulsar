using System;

namespace Pulsar.Graphics.Fx
{
    internal sealed class ShaderBinding : IDisposable
    {
        #region Fields

        private Shader _shader;
        private ShaderTechniqueDefinition _technique;

        #endregion

        #region Constructors

        internal ShaderBinding(Shader shader, ShaderTechniqueDefinition technique)
        {
            _shader = shader;
            _technique = technique;
            MaterialBinding = shader.CreateVariableBinding(ShaderVariableUsage.Material);
            InstanceBinding = shader.CreateVariableBinding(ShaderVariableUsage.Instance);
        }

        #endregion

        #region Methods

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

        internal void UseTechnique()
        {
            _shader.SetCurrentTechnique(_technique);
        }

        internal ShaderPassEnumerator GetPassEnumerator()
        {
            return new ShaderPassEnumerator(_technique);
        }

        #endregion

        #region Properties

        internal ShaderVariableBindingCollection GlobalBinding
        {
            get { return _shader.GlobalVariablesBinding; }
        }

        internal ShaderVariableBindingCollection MaterialBinding { get; private set; }

        internal ShaderVariableBindingCollection InstanceBinding { get; private set; }

        public string Technique
        {
            get { return _technique.Name; }
        }

        public bool IsTransparent
        {
            get { return _technique.IsTransparent; }
        }

        #endregion
    }
}
