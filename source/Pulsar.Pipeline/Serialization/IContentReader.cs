using System;

namespace Pulsar.Pipeline.Serialization
{
    public interface IContentReader
    {
        #region Methods

        void Initialize(ReaderManager manager);

        object Read(string value, ReaderContext context);

        #endregion

        #region Properties

        Type TargetType { get; }

        #endregion
    }
}
