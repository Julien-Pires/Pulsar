using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Shader - Pulsar")]
    public class ShaderProcessor : ContentProcessor<ShaderContent, ShaderContent>
    {
        #region Static methods

        private static Type ExtractVariableType(EffectParameter fxParameter)
        {
            Type result = null;
            switch (fxParameter.ParameterClass)
            {
                case EffectParameterClass.Matrix:
                    result = typeof (Matrix);
                    break;

                case EffectParameterClass.Object:
                    result = GetObjectType(fxParameter);
                    break;

                case EffectParameterClass.Scalar:
                    result = GetScalarType(fxParameter);
                    break;

                case EffectParameterClass.Vector:
                    result = GetVectorType(fxParameter);
                    break;
            }

            if(result == null)
                throw new Exception("Failed to load shader, unsupported parameter type detected");

            return result;
        }

        private static Type GetVectorType(EffectParameter fxParameter)
        {
            Type result = null;
            switch (fxParameter.ColumnCount)
            {
                case 2:
                    result = typeof (Vector2);
                    break;

                case 3:
                    result = typeof (Vector3);
                    break;

                case 4:
                    result = typeof (Vector4);
                    break;
            }

            return result;
        }

        private static Type GetObjectType(EffectParameter fxParameter)
        {
            Type result = null;
            if (fxParameter.Elements.Count == 0)
            {
                switch (fxParameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(Texture);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string);
                        break;
                }
            }
            else
            {
                switch (fxParameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(Texture[]);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D[]);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D[]);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string[]);
                        break;
                }
            }

            return result;
        }

        private static Type GetScalarType(EffectParameter fxParameter)
        {
            Type result = null;
            if (fxParameter.Elements.Count == 0)
            {
                switch (fxParameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof (bool);
                        break;
                        
                    case EffectParameterType.Int32:
                        result = typeof (int);
                        break;

                    case EffectParameterType.Single:
                        result = typeof (float);
                        break;
                }
            }
            else
            {
                switch (fxParameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof(bool[]);
                        break;

                    case EffectParameterType.Int32:
                        result = typeof(int[]);
                        break;

                    case EffectParameterType.Single:
                        result = typeof(float[]);
                        break;
                }
            }

            return result;
        }

        private static ShaderVariableContent CreateVariableDefinition(EffectParameter fxParameter)
        {
            ShaderVariableContent content = new ShaderVariableContent
            {
                Name = fxParameter.Name,
                Usage = ShaderVariableUsage.Instance
            };

            if (!string.IsNullOrWhiteSpace(fxParameter.Semantic))
            {
                ShaderVariableSemantic semantic;
                content.Source = Enum.TryParse(fxParameter.Semantic, true, out semantic) ? ShaderVariableSource.Auto : ShaderVariableSource.Keyed;
                content.Semantic = fxParameter.Semantic;
            }
            else
                content.Source = ShaderVariableSource.Custom;

            return content;
        }

        private static void ExtractVariablesDefinition(Effect fx, ShaderContent content)
        {
            foreach (EffectParameter fxParam in fx.Parameters)
            {
                ShaderVariableContent shaderVar;
                if (!content.Variables.TryGetValue(fxParam.Name, out shaderVar))
                    shaderVar = CreateVariableDefinition(fxParam);

                shaderVar.Type = ExtractVariableType(fxParam).AssemblyQualifiedName;
            }
        }

        private static void ValidateVariablesDefinition(ShaderContent content)
        {
            foreach (ShaderVariableContent shaderVar in content.Variables.Values)
            {
                switch (shaderVar.Source)
                {
                    case ShaderVariableSource.Auto:
                        ShaderVariableSemantic semantic;
                        if(!Enum.TryParse(shaderVar.Semantic, out semantic))
                            throw new Exception(string.Format("{0} is an invalid semantic when source is set to Auto", shaderVar.Semantic));
                        break;

                    case ShaderVariableSource.Keyed:
                        if(string.IsNullOrWhiteSpace(shaderVar.Semantic))
                            throw new Exception("Semantic cannot be null or empty when source is set to Keyed");
                        break;
                }
            }
        }

        private static void ValidateTechniquesDefinition(Effect fx, ShaderContent content)
        {
            if (!string.IsNullOrWhiteSpace(content.InstancingTechnique))
            {
                if(fx.Techniques[content.InstancingTechnique] == null)
                    throw new Exception(string.Format("Failed to find {0} instancing technique in effect file", content.InstancingTechnique));
            }

            if (!string.IsNullOrWhiteSpace(content.Fallback))
            {
                if (fx.Techniques[content.Fallback] == null)
                    throw new Exception(string.Format("Failed to find {0} fallback technique in effect file", content.Fallback));
            }

            foreach (ShaderTechniqueContent shaderTech in content.Techniques.Values)
            {
                if(fx.Techniques[shaderTech.Name] == null)
                    throw new Exception(string.Format("Failed to find {0} technique in effect file", shaderTech.Name));
            }
        }

        #endregion

        #region Methods

        public override ShaderContent Process(ShaderContent input, ContentProcessorContext context)
        {
            if(input == null)
                throw new ArgumentNullException("input");

            if(context == null)
                throw new ArgumentNullException("context");

            Effect fx = context.BuildAndLoadAsset<EffectContent, Effect>(input.Effect, "EffectProcessor");
            ExtractVariablesDefinition(fx, input);
            ValidateVariablesDefinition(input);
            ValidateTechniquesDefinition(fx, input);

            return input;
        }

        #endregion
    }
}
