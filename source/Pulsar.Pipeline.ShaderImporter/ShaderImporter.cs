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
using Pulsar.Pipeline.ShaderImporter.Properties;

namespace Pulsar.Pipeline.ShaderImporter
{
    /// <summary>
    /// Importer for shader definition file
    /// </summary>
    [ContentImporterAttribute(".shd", DefaultProcessor = "ShaderProcessor", DisplayName = "Shader Importer - Pulsar")]
    public class ShaderImporter : ContentImporter<ShaderDefinitionContent>
    {
        #region Fields

        private const string EffectExtension = ".fx";

        private const string FileProperty = "File";
        private const string InstancingProperty = "Instancing";
        private const string FallbackProperty = "Fallback";

        private const string ConstantProperty = "Constants";
        private const string ConstantSourceProperty = "Source";
        private const string ConstantUpdateFrequencyProperty = "UpdateFrequency";
        private const string ConstantSemanticProperty = "Semantic";
        private const string ConstantEquivalentTypeProperty = "EquivalentType";

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
            content.Instancing = GetStringValue(shaderBlock, InstancingProperty);
            content.Fallback = GetStringValue(shaderBlock, FallbackProperty);
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
        /// Extracts shader constants data
        /// </summary>
        /// <param name="constantsBlock">Imported data</param>
        /// <param name="content">Content object that received imported data</param>
        private static void ImportConstants(JObject constantsBlock, ShaderDefinitionContent content)
        {
            if(constantsBlock == null)
                return;

            foreach (JProperty shaderConstant in constantsBlock.Values<JProperty>())
            {
                ShaderConstantContent constantContent = new ShaderConstantContent(shaderConstant.Name);
                JObject shaderConstantInfo = (JObject)shaderConstant.Value;

                ShaderConstantSource source;
                if (GetEnumValue(shaderConstantInfo, ConstantSourceProperty, out source))
                    constantContent.Source = source;

                UpdateFrequency update;
                if (GetEnumValue(shaderConstantInfo, ConstantUpdateFrequencyProperty, out update))
                    constantContent.UpdateFrequency = update;

                constantContent.Semantic = GetStringValue(shaderConstantInfo, ConstantSemanticProperty);
                constantContent.EquivalentType =
                    GetStringValue(shaderConstantInfo, ConstantEquivalentTypeProperty).ToLower();

                content.Constants.Add(constantContent.Name, constantContent);
            }
        }

        /// <summary>
        /// Parses a json string property to an enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="jObject">Json object</param>
        /// <param name="key">Name of the property</param>
        /// <param name="result">Result value</param>
        /// <returns>Returns true if the property is parsed successfully otherwise false</returns>
        private static bool GetEnumValue<T>(JObject jObject, string key, out T result) where T : struct
        {
            result = default(T);
            JToken token = jObject[key];
            if (token == null)
                return false;

            string value = (string)token;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (!Enum.TryParse(value, out result))
                throw new Exception(string.Format("Failed to cast {0} to enum {1}", value, typeof(T)));

            return true;
        }

        /// <summary>
        /// Gets the string value of a json property
        /// </summary>
        /// <param name="jObject">Json object</param>
        /// <param name="key">Name of the property</param>
        /// <returns>Returns the value of the property if found otherwise an empty string</returns>
        private static string GetStringValue(JObject jObject, string key)
        {
            JToken token = jObject[key];
            if (token == null)
                return string.Empty;

            return (string) token;
        }

        /// <summary>
        /// Checks if the definition file match the definition schema
        /// </summary>
        /// <param name="definition">Imported data</param>
        /// <param name="error">Error message</param>
        /// <returns>Returns true if the definition is valid otherwise false</returns>
        private static bool IsValidDefinitionFormat(JObject definition, out string error)
        {
            string schema = Encoding.UTF8.GetString(Resources.ShaderDefinitionSchema);
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

            JObject jsonObject = definition[TechniquesProperty] as JObject;
            ImportTechniques(jsonObject, content);

            jsonObject = definition[ConstantProperty] as JObject;
            ImportConstants(jsonObject, content);

            return content;
        }

        #endregion
    }
}