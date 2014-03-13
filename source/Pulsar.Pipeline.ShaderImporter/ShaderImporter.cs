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
    /// <summary>
    /// Importer for shader definition file
    /// </summary>
    [ContentImporterAttribute(".shd", DefaultProcessor = "ShaderProcessor", DisplayName = "Shader Importer - Pulsar")]
    public class ShaderImporter : ContentImporter<ShaderDefinitionContent>
    {
        #region Fields

        private const string Schemas = "Schema/ShaderDefinition.schema.json";
        private const string EffectExtension = ".fx";

        private const string FileProperty = "File";
        private const string InstancingProperty = "Instancing";
        private const string FallbackProperty = "Fallback";

        private const string VariablesProperty = "Variables";
        private const string VariableSourceProperty = "Source";
        private const string VariableUsageProperty = "Usage";
        private const string VariableSemanticProperty = "Semantic";
        private const string VariableEquivalentTypeProperty = "EquivalentType";

        private const string TechniquesProperty = "Techniques";
        private const string TechniqueTransparentProperty = "Transparent";

        #endregion

        #region Static methods

        /// <summary>
        /// Gets the path to the effect file
        /// </summary>
        /// <param name="definition">Imported data</param>
        /// <param name="definitionPath">Path of the shader definition file</param>
        /// <returns>Returns the path to the associated effect file</returns>
        private static string GetEffectFilePath(JObject definition, string definitionPath)
        {
            JToken fileToken = definition[FileProperty];
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

        /// <summary>
        /// Extracts shader data
        /// </summary>
        /// <param name="shaderBlock">Imported data</param>
        /// <param name="content">Content object that received imported data</param>
        private static void ImportShaderInfo(JObject shaderBlock, ShaderDefinitionContent content)
        {
            JToken token = shaderBlock[InstancingProperty];
            if (token != null)
                content.Instancing = (string) token;

            token = shaderBlock[FallbackProperty];
            if (token != null)
                content.Fallback = (string) token;
        }

        /// <summary>
        /// Extracts shader technique data
        /// </summary>
        /// <param name="techniquesBlock">Imported data</param>
        /// <param name="content">Content object that received imported data</param>
        private static void ImportTechniques(JObject techniquesBlock, ShaderDefinitionContent content)
        {
            if(techniquesBlock == null)
                return;

            foreach (JProperty technique in techniquesBlock.Values<JProperty>())
            {
                ShaderTechniqueContent techniqueContent = new ShaderTechniqueContent(technique.Name);
                JObject techniqueInfo = (JObject)technique.Value;

                JToken prop = techniqueInfo[TechniqueTransparentProperty];
                if (prop != null)
                    techniqueContent.IsTransparent = (bool)prop;

                content.Techniques.Add(techniqueContent.Name, techniqueContent);
            }
        }

        /// <summary>
        /// Extracts shader variable data
        /// </summary>
        /// <param name="variablesBlock">Imported data</param>
        /// <param name="content">Content object that received imported data</param>
        private static void ImportVariables(JObject variablesBlock, ShaderDefinitionContent content)
        {
            if(variablesBlock == null)
                return;

            foreach (JProperty shaderVar in variablesBlock.Values<JProperty>())
            {
                ShaderVariableContent variableContent = new ShaderVariableContent(shaderVar.Name);
                JObject shaderVarInfo = (JObject)shaderVar.Value;

                string value = (string)shaderVarInfo[VariableSourceProperty];
                if(string.IsNullOrWhiteSpace(value))
                    throw new Exception("Shader variable source cannot be null or empty");

                ShaderVariableSource source;
                if(!Enum.TryParse(value, out source))
                    throw new Exception("Invalid shader variable source value provided");
                variableContent.Source = source;

                JToken token = shaderVarInfo[VariableUsageProperty];
                if (token != null)
                {
                    value = (string) token;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        ShaderVariableUsage usage;
                        if(!Enum.TryParse(value, out usage))
                            throw new Exception("Invalid shader variable usage value provided");
                        variableContent.Usage = usage;
                    }
                }

                token = shaderVarInfo[VariableSemanticProperty];
                if (token != null)
                    variableContent.Semantic = (string) token;

                token = shaderVarInfo[VariableEquivalentTypeProperty];
                if (token != null)
                    variableContent.EquivalentType = ((string)token).ToLower();

                content.Variables.Add(variableContent.Name, variableContent);
            }
        }

        /// <summary>
        /// Checks if the definition file match the definition schema
        /// </summary>
        /// <param name="definition">Imported data</param>
        /// <param name="error">Error message</param>
        /// <returns>Returns true if the definition is valid otherwise false</returns>
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

        /// <summary>
        /// Imports a shader definition file
        /// </summary>
        /// <param name="filename">Path of the definition file</param>
        /// <param name="context">Context</param>
        /// <returns>Returns an object that contains imported data from the file</returns>
        public override ShaderDefinitionContent Import(string filename, ContentImporterContext context)
        {
            string text = File.ReadAllText(filename);
            if(string.IsNullOrWhiteSpace(text))
                throw new Exception(string.Format("Failed to load shader definition, {0} is empty", filename));

            JObject definition = JObject.Parse(text);
            string error;
            if(!IsValidDefinitionFormat(definition, out error))
                throw new Exception(string.Format("Invalid definition format : {0}", error));

            string filePath = GetEffectFilePath(definition, filename);
            if(!File.Exists(filePath))
                throw new Exception("Failed to load shader definition, effect file missing");

            ShaderDefinitionContent content = new ShaderDefinitionContent {EffectFile = new ExternalReference<EffectContent>(filePath)};
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