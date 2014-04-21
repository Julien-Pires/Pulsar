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
            if (input == null)
                return result;

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
            Dictionary<string, object> opaqueData = parameters;
            if (parameters.ContainsKey(ProcessorKey))
            {
                Dictionary<string, object> processorsParameters = parameters[ProcessorKey] as Dictionary<string, object>;
                if (processorsParameters == null)
                    throw new Exception("Invalid processor parameters format");

                object processorName;
                if (parameters.TryGetValue(ProcessorNameKey, out processorName))
                    result.Processor = (string) processorName;
                else
                    result.Processor = DefaultProcessor;

                opaqueData = null;
                if (processorsParameters.ContainsKey(OpaqueDataKey))
                    opaqueData = processorsParameters[OpaqueDataKey] as Dictionary<string, object>;
            }

            result.OpaqueData = ProcessOpaqueData(opaqueData);

            return result;
        }

        #endregion
    }
}
