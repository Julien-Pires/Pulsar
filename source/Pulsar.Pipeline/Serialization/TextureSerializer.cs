using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

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
        private const string UnprocessedParametersKey = "UnprocessedParameters";

        private const string ColorKeyColor = "ColorKeyColor";
        private const string ColorKeyEnabled = "ColorKeyEnabled";
        private const string GenerateMipmaps = "GenerateMipmaps";
        private const string PremultiplyAlpha = "PremultiplyAlpha";
        private const string ResizeToPowerOfTwo = "ResizeToPowerOfTwo";
        private const string TextureFormat = "TextureFormat";

        private ReaderManager _manager;

        #endregion

        #region Constructors

        internal TextureSerializer()
        {
        }

        #endregion

        #region Methods

        public override void Initialize(ReaderManager manager)
        {
            _manager = manager;
        }

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

        private OpaqueDataDictionary ProcessOpaqueData(Dictionary<string, object> input)
        {
            Dictionary<string, string> rawOpaqueData = input.ToDictionary(c => c.Key, c => (string) c.Value);
            OpaqueDataDictionary result = new OpaqueDataDictionary{{UnprocessedParametersKey, true}};
            foreach (KeyValuePair<string, string> pair in rawOpaqueData)
                result.Add(pair.Key, pair.Value);

            ConvertCommonParameters(result);

            return result;
        }

        private void ConvertCommonParameters(OpaqueDataDictionary parameters)
        {
            object value;
            if (parameters.TryGetValue(ColorKeyColor, out value))
            {
                ColorSerializer colorSerializer = (ColorSerializer)_manager.GetReader(typeof(Color));
                Color keyColor = colorSerializer.Read((string)value, null);
                parameters[ColorKeyColor] = keyColor;
            }

            BoolSerializer boolSerializer = (BoolSerializer)_manager.GetReader(typeof(bool));
            if (parameters.TryGetValue(ColorKeyEnabled, out value))
                parameters[ColorKeyEnabled] = boolSerializer.Read((string)value, null);

            if (parameters.TryGetValue(GenerateMipmaps, out value))
                parameters[GenerateMipmaps] = boolSerializer.Read((string)value, null);

            if (parameters.TryGetValue(PremultiplyAlpha, out value))
                parameters[PremultiplyAlpha] = boolSerializer.Read((string)value, null);

            if (parameters.TryGetValue(ResizeToPowerOfTwo, out value))
                parameters[ResizeToPowerOfTwo] = boolSerializer.Read((string)value, null);

            if (parameters.TryGetValue(TextureFormat, out value))
                parameters[TextureFormat] = Enum.Parse(typeof(TextureProcessorOutputFormat), (string)value);
        }

        #endregion
    }
}
