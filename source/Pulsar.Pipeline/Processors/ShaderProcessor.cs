using System;

using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Processors
{
    /// <summary>
    /// Processes a shader definition to a shader content that can be load at runtime
    /// </summary>
    [ContentProcessor(DisplayName = "Shader - Pulsar")]
    public class ShaderProcessor : ContentProcessor<ShaderDefinitionContent, ShaderContent>
    {
        #region Static methods

        /// <summary>
        /// Validates shader constants definition
        /// </summary>
        /// <param name="input">Input</param>
        private static void ValidateConstantsDefinition(ShaderDefinitionContent input)
        {
            foreach (ShaderConstantContent shaderConstant in input.Constants.Values)
            {
                switch (shaderConstant.Source)
                {
                    case ShaderConstantSource.Auto:
                        ShaderConstantSemantic semantic;
                        if (!Enum.TryParse(shaderConstant.Semantic, out semantic))
                            throw new Exception(string.Format("{0} is an invalid semantic when source is set to Auto", shaderConstant.Semantic));
                        break;

                    case ShaderConstantSource.Keyed:
                        if (string.IsNullOrWhiteSpace(shaderConstant.Semantic))
                            throw new Exception("Semantic cannot be null or empty when source is set to Keyed");
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts shader definition to shader content
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="context">Context</param>
        /// <returns>Returns a shader content</returns>
        public override ShaderContent Process(ShaderDefinitionContent input, ContentProcessorContext context)
        {
            if(input == null)
                throw new ArgumentNullException("input");

            if(context == null)
                throw new ArgumentNullException("context");

            ValidateConstantsDefinition(input);

            CompiledEffectContent compiledFx = context.BuildAndLoadAsset<EffectContent, CompiledEffectContent>(input.EffectFile, "EffectProcessor");
            ShaderContent shader = new ShaderContent(compiledFx.GetEffectCode())
            {
                Fallback = input.Fallback,
                Instancing = input.Instancing
            };
            
            foreach (ShaderTechniqueContent shaderTech in input.Techniques.Values)
                shader.Techniques.Add(shaderTech);
            foreach (ShaderConstantContent shaderConstant in input.Constants.Values)
                shader.Constants.Add(shaderConstant);

            return shader;
        }

        #endregion
    }
}
