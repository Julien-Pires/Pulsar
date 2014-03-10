using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;

namespace Pulsar.Pipeline.ShaderImporter
{
    [ContentImporterAttribute(".shd", DefaultProcessor = "", DisplayName = "Shader Definition Importer")]
    public class ShaderImporter : ContentImporter<ShaderContent>
    {
        #region Fields

        private const string Schemas = "Schema/ShaderDefinition.schema.json";
        private const string EffectExtension = ".fx";

        private const string FileProperty = "File";
        private const string InstancingTechniqueProperty = "InstancingTechnique";
        private const string FallbackProperty = "Fallback";

        private const string VariablesProperty = "Variables";
        private const string VariableSourceProperty = "Source";
        private const string VariableUsageProperty = "Usage";
        private const string VariableKeyProperty = "Semantic";

        private const string TechniquesProperty = "Techniques";
        private const string TechniqueTransparentProperty = "Transparent";

        #endregion

        #region Static methods

        private static string GetEffectFilePath(JObject definition, string definitionPath)
        {
            JProperty fileToken = definition[FileProperty] as JProperty;
            string filePath;
            if (fileToken == null)
            {
                filePath = Path.GetFileName(definitionPath);
                filePath = Path.ChangeExtension(filePath, EffectExtension);
            }
            else
                filePath = (string)fileToken;

            return filePath;
        }

        private static void ImportShaderInfo(JObject shaderBlock, ShaderContent content)
        {
            JProperty prop = shaderBlock[InstancingTechniqueProperty] as JProperty;
            if (prop != null)
                content.InstancingTechnique = (string) prop;

            prop = shaderBlock[FallbackProperty] as JProperty;
            if (prop != null)
                content.Fallback = (string) prop;
        }

        private static void ImportTechniques(JObject techniquesBlock, ShaderContent content)
        {
            if(techniquesBlock == null)
                return;

            foreach (JProperty technique in techniquesBlock.Values<JProperty>())
            {
                ShaderTechniqueContent techniqueContent = new ShaderTechniqueContent {Name = technique.Name};
                JObject techniqueInfo = (JObject)technique.Value;

                JProperty prop = techniqueInfo[TechniqueTransparentProperty] as JProperty;
                if (prop != null)
                    techniqueContent.IsTransparent = (bool)prop;

                content.Techniques.Add(techniqueContent.Name, techniqueContent);
            }
        }

        private static void ImportVariables(JObject variablesBlock, ShaderContent content)
        {
            if(variablesBlock == null)
                return;

            foreach (JProperty shaderVar in variablesBlock.Values<JProperty>())
            {
                ShaderVariableContent variableContent = new ShaderVariableContent {Name = shaderVar.Name};
                JObject shaderVarInfo = (JObject)shaderVar.Value;

                string value = (string)shaderVarInfo[VariableSourceProperty];
                if(string.IsNullOrWhiteSpace(value))
                    throw new Exception("Shader variable source cannot be null or empty");

                ShaderVariableSource source;
                if(!Enum.TryParse(value, out source))
                    throw new Exception("Invalid shader variable source value provided");
                variableContent.Source = source;

                JProperty prop = shaderVarInfo[VariableUsageProperty] as JProperty;
                if (prop != null)
                {
                    value = (string) prop;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ShaderVariableUsage usage;
                        if(!Enum.TryParse(value, out usage))
                            throw new Exception("Invalid shader variable usage value provided");
                        variableContent.Usage = usage;
                    }
                }

                prop = shaderVarInfo[VariableKeyProperty] as JProperty;
                if (prop != null)
                    variableContent.Semantic = (string) prop;

                content.Variables.Add(variableContent.Name, variableContent);
            }
        }

        private static bool IsValidDefinitionFormat(JObject definition, out string error)
        {
            string schema = File.ReadAllText(Schemas);
            JsonSchema jSchema = JsonSchema.Parse(schema);
            IList<string> validationMsg;
            bool result = definition.IsValid(jSchema, out validationMsg);

            error = null;
            if (!result)
            {
                int length = 0;
                for (int i = 0; i < validationMsg.Count; i++)
                    length += validationMsg[i].Length + Environment.NewLine.Length;

                StringBuilder strBuilder = new StringBuilder(length);
                for (int i = 0; i < validationMsg.Count; i++)
                    strBuilder.AppendLine(validationMsg[i]);

                error = strBuilder.ToString();
            }

            return result;
        }

        #endregion

        #region Methods

        public override ShaderContent Import(string filename, ContentImporterContext context)
        {
            string text = File.ReadAllText(filename);
            if(string.IsNullOrWhiteSpace(text))
                throw new Exception(string.Format("Failed to load shader definition, {0} is empty", filename));

            JObject definition = new JObject(text);
            string error;
            if(!IsValidDefinitionFormat(definition, out error))
                throw new Exception(string.Format("Invalid definition format : {0}", error));

            string filePath = GetEffectFilePath(definition, filename);
            if(!File.Exists(filePath))
                throw new Exception("Failed to load shader definition, effect file missing");

            ShaderContent content = new ShaderContent {Effect = new ExternalReference<EffectContent>(filePath)};
            ImportShaderInfo(definition, content);

            JObject jsonObject = definition[VariablesProperty] as JObject;
            ImportVariables(jsonObject, content);

            jsonObject = definition[TechniquesProperty] as JObject;
            ImportTechniques(jsonObject, content);

            return content;
        }

        #endregion
    }
}