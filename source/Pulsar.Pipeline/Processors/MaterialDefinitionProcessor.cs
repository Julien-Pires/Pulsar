using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;

using Pulsar.Pipeline.Graphics;
using Pulsar.Pipeline.Serialization;
using MaterialContent = Pulsar.Pipeline.Graphics.MaterialContent;

namespace Pulsar.Pipeline.Processors
{
    /// <summary>
    /// Processes a material asset to a Material for used at runtime
    /// </summary>
    [ContentProcessor(DisplayName = "Material - Pulsar")]
    public sealed class MaterialDefinitionProcessor : ContentProcessor<RawMaterialContent, MaterialContent>
    {
        #region Fields

        private readonly List<MaterialDataContent> _datas = new List<MaterialDataContent>();
        private readonly SerializerManager _serializerManager = new SerializerManager();

        #endregion

        #region Methods

        /// <summary>
        /// Converts raw material data to material content
        /// </summary>
        /// <param name="input">Input content</param>
        /// <param name="context">Context</param>
        /// <returns>Returns material content</returns>
        public override MaterialContent Process(RawMaterialContent input, ContentProcessorContext context)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (context == null)
                throw new ArgumentNullException("context");

            GenerateData(input.Data, context);

            return new MaterialContent(input.Name, _datas, input.Shader, input.Technique);
        }

        /// <summary>
        /// Converts raw string data to strongly typed data
        /// </summary>
        /// <param name="rawCollection">Raw datas</param>
        /// <param name="context">Context</param>
        private void GenerateData(List<RawMaterialDataContent> rawCollection, ContentProcessorContext context)
        {
            for (int i = 0; i < rawCollection.Count; i++)
            {
                RawMaterialDataContent rawData = rawCollection[i];
                if (rawData.Value.Length == 0)
                    continue;

                Tuple<Type, Type> type = MaterialDataTypeParser.GetType(rawData.Type, rawData.Value);
                MaterialDataContent data = new MaterialDataContent(rawData.Name)
                {
                    BuildType = type.Item1,
                    RuntimeType = type.Item2
                };

                object value;
                string[] values = rawData.Value;
                List<Dictionary<string, object>> parameters = rawData.Parameters;
                if (rawData.IsNativeArray)
                {
                    SerializerContext[] contexts = new SerializerContext[values.Length];
                    for (int j = 0; j < values.Length; j++)
                    {
                        int index = Math.Min(j, parameters.Count - 1);
                        SerializerContext serializerCtx = new SerializerContext(context)
                        {
                            Parameters = (index > -1) ? parameters[index] : null
                        };
                        contexts[j] = serializerCtx;
                    }

                    value = _serializerManager.Deserialize(type.Item1, values, contexts);
                    data.BuildType = data.BuildType.MakeArrayType();
                    data.RuntimeType = data.RuntimeType.MakeArrayType();
                }
                else
                {
                    SerializerContext serializerCtx = new SerializerContext(context)
                    {
                        Parameters = (parameters.Count > 0) ? parameters[0] : null
                    };
                    value = _serializerManager.Deserialize(type.Item1, rawData.Value[0], serializerCtx);
                }
                data.Value = value;

                _datas.Add(data);
            }
        }

        #endregion
    }
}
