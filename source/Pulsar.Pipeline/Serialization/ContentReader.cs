using System;

namespace Pulsar.Pipeline.Serialization
{
    public abstract class ContentReader<T> : IContentReader
    {
        #region Constructors

        protected ContentReader()
        {
            TargetType = typeof (T);
        }

        #endregion

        #region Methods

        public virtual void Initialize(ReaderManager manager)
        {
        }

        object IContentReader.Read(string value, ReaderContext context)
        {
            return Read(value, context);
        }

        public abstract T Read(string value, ReaderContext context);

        #endregion

        #region Properties

        public Type TargetType { get; private set; }

        #endregion
    }
}
