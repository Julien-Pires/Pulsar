using System;

namespace Pulsar.Pipeline.Serialization
{
    public interface IContentSerializer
    {
        #region Methods

        void Initialize(ReaderManager manager);

        object Read(string value, SerializerContext context);

        #endregion

        #region Properties

        Type TargetType { get; }

        #endregion
    }
}
