using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Pulsar.Pipeline.Graphics
{
    /// <summary>
    /// Stores design-time data for a Shader asset
    /// </summary>
    public sealed class ShaderContent
    {
        #region Fields

        private readonly List<ShaderTechniqueContent> _techniques = new List<ShaderTechniqueContent>();
        private readonly List<ShaderConstantContent> _constants = new List<ShaderConstantContent>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderContent class
        /// </summary>
        internal ShaderContent(byte[] compiledEffect)
        {
            CompiledEffect = compiledEffect;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes this instance to a content writer
        /// </summary>
        /// <param name="output">Output</param>
        internal void Write(ContentWriter output)
        {
            output.Write(CompiledEffect.Length);
            output.Write(CompiledEffect);
            WriteTechniques(output);
            WriteConstants(output);

            output.Write(Fallback);
            output.Write(Instancing);
            output.Write(Default);
        }

        /// <summary>
        /// Writes a list of shader constants definition to a content writer
        /// </summary>
        /// <param name="output">Output</param>
        private void WriteConstants(ContentWriter output)
        {
            output.Write(_constants.Count);
            for(int i = 0; i < _constants.Count; i++)
            {
                output.Write(_constants[i].Name);
                output.Write((int)_constants[i].Source);
                output.Write((int)_constants[i].UpdateFrequency);
                output.Write(_constants[i].Semantic);
                output.Write(_constants[i].EquivalentType);
            }
        }

        /// <summary>
        /// Writes a list of shader techniques definition to a content writer
        /// </summary>
        /// <param name="output">Output</param>
        private void WriteTechniques(ContentWriter output)
        {
            output.Write(_techniques.Count);
            for (int i = 0; i < _techniques.Count; i++)
            {
                output.Write(_techniques[i].Name);
                output.Write(_techniques[i].IsTransparent);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the compiled effect
        /// </summary>
        public byte[] CompiledEffect { get; private set; }

        /// <summary>
        /// Gets a list of constant definition
        /// </summary>
        public List<ShaderConstantContent> Constants
        {
            get { return _constants; }
        }

        /// <summary>
        /// Gets a list of technique definition
        /// </summary>
        public List<ShaderTechniqueContent> Techniques
        {
            get { return _techniques; }
        }

        public string Default { get; set; }

        /// <summary>
        /// Gets the name of the fallback technique
        /// </summary>
        public string Fallback { get; set; }

        /// <summary>
        /// Gets the name of the instancing technique
        /// </summary>
        public string Instancing { get; set; }

        #endregion
    }
}
