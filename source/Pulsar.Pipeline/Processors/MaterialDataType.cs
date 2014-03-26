using System;

namespace Pulsar.Pipeline.Processors
{
    public sealed partial class MaterialDefinitionProcessor
    {
        #region Nested

        private sealed class MaterialDataType
        {
            #region Constructors

            public MaterialDataType(Type readerType, Type runtimeType)
            {
                ReaderType = readerType;
                RuntimeType = runtimeType;
            }

            #endregion

            #region Properties

            public Type ReaderType { get; private set; }

            public Type RuntimeType { get; private set; }

            #endregion
        }

        #endregion
    }
}
