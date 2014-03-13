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
        private readonly List<ShaderVariableContent> _variables = new List<ShaderVariableContent>();

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
            WriteVariables(output);
            output.Write(Fallback);
            output.Write(Instancing);
        }

        /// <summary>
        /// Writes a list of shader variables definition to a content writer
        /// </summary>
        /// <param name="output">Output</param>
        private void WriteVariables(ContentWriter output)
        {
            output.Write(_variables.Count);
            for(int i = 0; i < _variables.Count; i++)
            {
                output.Write(_variables[i].Name);
                output.Write((int)_variables[i].Source);
                output.Write((int)_variables[i].Usage);
                output.Write(_variables[i].Semantic);
                output.Write(_variables[i].EquivalentType);
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
        /// Gets a list of variable definition
        /// </summary>
        public List<ShaderVariableContent> Variables
        {
            get { return _variables; }
        }

        /// <summary>
        /// Gets a list of technique definition
        /// </summary>
        public List<ShaderTechniqueContent> Techniques
        {
            get { return _techniques; }
        }

        /// <summary>
        /// Gets the name of the fallback technique
        /// </summary>
        public string Fallback { get; internal set; }

        /// <summary>
        /// Gets the name of the instancing technique
        /// </summary>
        public string Instancing { get; internal set; }

        #endregion
    }
}
