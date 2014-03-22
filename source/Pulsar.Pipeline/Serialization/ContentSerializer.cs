using System;

namespace Pulsar.Pipeline.Serialization
{
    public abstract class ContentSerializer<T> : IContentSerializer
    {
        #region Constructors

        protected ContentSerializer()
        {
            TargetType = typeof (T);
        }

        #endregion

        #region Methods

        public virtual void Initialize(ReaderManager manager)
        {
        }

        object IContentSerializer.Read(string value, SerializerContext context)
        {
            return Read(value, context);
        }

        public abstract T Read(string value, SerializerContext context);

        #endregion

        #region Properties

        public Type TargetType { get; private set; }

        #endregion
    }
}
