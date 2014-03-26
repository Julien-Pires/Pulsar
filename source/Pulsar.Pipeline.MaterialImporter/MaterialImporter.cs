using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;

using Newtonsoft.Json.Linq;

using Pulsar.Pipeline.Graphics;
using Pulsar.Pipeline.MaterialImporter.Properties;

namespace Pulsar.Pipeline.MaterialImporter
{
    [ContentImporterAttribute(".pmtl", DefaultProcessor = "MaterialDefinitionProcessor", DisplayName = "Material Importer - Pulsar")]
    public class MaterialImporter : ContentImporter<RawMaterialContent>
    {
        #region Fields

        private const string NameProperty = "Name";
        private const string ShaderProperty = "Shader";
        private const string DataProperty = "Data";

        private const string DataTypeProperty = "Type";
        private const string DataValueProperty = "Value";
        private const string DataParametersProperty = "Parameters";

        #endregion

        #region Static methods

        private static void ImportData(JObject dataObj, RawMaterialContent content)
        {
            Debug.Assert(content != null);

            if(dataObj == null)
                return;

            foreach (JProperty property in dataObj.Values<JProperty>())
            {
                RawMaterialDataContent data = new RawMaterialDataContent(property.Name);
                JObject value = (JObject)property.Value;
                data.Type = JsonHelper.GetString(value, DataTypeProperty);
                data.Value = JsonHelper.GetStringAsArray(value, DataValueProperty);
                data.IsNativeArray = value[DataValueProperty] is JArray;
                ImportExtraParameters(value[DataParametersProperty], data);

                content.Data.Add(data);
            }
        }

        private static void ImportExtraParameters(JToken parameters, RawMaterialDataContent data)
        {
            if(data.Value.Length <= 0)
                return;

            JArray array = parameters as JArray;
            if (array != null)
            {
                for (int i = 0; i < data.Value.Length; i++)
                {
                    Dictionary<string, object> map = (i < array.Count) ? 
                        ConvertToMap(array[i] as JObject) : new Dictionary<string, object>();
                    data.Parameters.Add(map);
                }
            }
            else
                data.Parameters.Add(ConvertToMap(parameters as JObject));
        }

        private static Dictionary<string, object> ConvertToMap(JObject jObject)
        {
            return (jObject == null) ? new Dictionary<string, object>() : JsonHelper.ConvertObjectToMap(jObject);
        }

        #endregion

        #region Methods

        public override RawMaterialContent Import(string filename, ContentImporterContext context)
        {
            string text = File.ReadAllText(filename);
            if (string.IsNullOrWhiteSpace(text))
                throw new Exception(string.Format("Failed to load material, {0} is empty", filename));

            string error;
            string schema = Encoding.UTF8.GetString(Resources.MaterialSchema);
            if (!JsonHelper.IsValid(text, schema, out error))
                throw new Exception(string.Format("Invalid material format : {0}", error));

            JObject jObject = JObject.Parse(text);
            string name = JsonHelper.GetString(jObject, NameProperty);
            string shader = JsonHelper.GetString(jObject, ShaderProperty);
            RawMaterialContent material = new RawMaterialContent(name, shader);
            ImportData((JObject)jObject[DataProperty], material);

            return material;
        }

        #endregion
    }
}