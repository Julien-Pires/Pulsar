using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class SerializerContext
    {
        #region Constructors

        public SerializerContext(ContentProcessorContext contentContext)
        {
            ContentContext = contentContext;
        }

        #endregion

        #region Properties

        public Dictionary<string, object> Parameters { get; internal set; }

        public ContentProcessorContext ContentContext { get; private set; }

        #endregion
    }
}
