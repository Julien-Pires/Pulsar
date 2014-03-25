using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed partial class TextureSerializer : ContentSerializer<ExternalReference<TextureContent>>
    {
        #region Fields

        private const string DefaultProcessor = "TextureProcessor";
        private const string DefaultProcessorKey = "default";

        private const string ProcessorKey = "Processor";
        private const string ProcessorNameKey = "Name";
        private const string OpaqueDataKey = "Parameters";

        #endregion

        #region Constructors

        internal TextureSerializer()
        {
        }

        #endregion

        #region Static methods

        private static OpaqueDataDictionary ProcessOpaqueData(Dictionary<string, object> input)
        {
            OpaqueDataDictionary result = new OpaqueDataDictionary();
            foreach (KeyValuePair<string, object> pair in input)
                result.Add(pair.Key, pair.Value);

            return result;
        }

        #endregion

        #region Methods

        public override ExternalReference<TextureContent> Read(string value, SerializerContext context)
        {
            if(context == null)
                throw new ArgumentNullException("context");

            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            TextureProcessorParameters processorInfos = GetProcessorParameters(context.Parameters);
            ExternalReference<TextureContent> extRef = new ExternalReference<TextureContent>(value);

            return context.ContentContext.BuildAsset<TextureContent, TextureContent>(extRef, processorInfos.Processor,
                processorInfos.OpaqueData, null, null);
        }

        private TextureProcessorParameters GetProcessorParameters(Dictionary<string, object> parameters)
        {
            TextureProcessorParameters result = new TextureProcessorParameters();
            if (parameters.ContainsKey(ProcessorKey))
            {
                Dictionary<string, object> processorsParameters = parameters[ProcessorKey] as Dictionary<string, object>;
                if (processorsParameters == null)
                    throw new Exception("Invalid processor parameters format");

                string processorName = (string)processorsParameters[ProcessorNameKey];
                if (!string.Equals(processorName.ToLower(), DefaultProcessorKey))
                    result.Processor = processorName;

                if (processorsParameters.ContainsKey(OpaqueDataKey))
                {
                    Dictionary<string, object> rawOpaqueData = processorsParameters[OpaqueDataKey] as Dictionary<string, object>;
                    if (rawOpaqueData == null)
                        throw new Exception("Invalid opaque data format");

                    result.OpaqueData = ProcessOpaqueData(rawOpaqueData);
                }
            }
            else
                result.OpaqueData = ProcessOpaqueData(parameters);

            return result;
        }

        #endregion
    }
}
