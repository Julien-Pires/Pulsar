using System;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class ArraySerializer<T> : ContentSerializer<T[]>
    {
        #region Fields

        private const char ValueSeparator = ',';
        private const string FirstBracket = "[";
        private const string LastBracket = "]";

        private ReaderManager _manager;
        private IContentSerializer _serializer;

        #endregion

        public override void Initialize(ReaderManager manager)
        {
            if(manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
            _serializer = _manager.GetReader(typeof (T));
        }

        public override T[] Read(string value, SerializerContext context)
        {
            if (value.StartsWith(FirstBracket))
                value = value.Substring(1);
            if (value.EndsWith(LastBracket))
                value = value.Substring(0, value.Length);

            string[] splitValue = value.Split(ValueSeparator);
            T[] result = new T[splitValue.Length];
            for (int i = 0; i < splitValue.Length; i++)
                result[i] = (T)_serializer.Read(splitValue[i], context);

            return result;
        }
    }
}
