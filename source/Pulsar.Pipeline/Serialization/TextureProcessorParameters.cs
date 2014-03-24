using Microsoft.Xna.Framework.Content.Pipeline;

namespace Pulsar.Pipeline.Serialization
{
    public sealed partial class TextureSerializer
    {
        private sealed class TextureProcessorParameters
        {
            #region Fields

            public string Processor = DefaultProcessor;

            public OpaqueDataDictionary OpaqueData;

            #endregion            
        }
    }
}
