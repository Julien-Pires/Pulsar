using System;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class ArrayReader<T> : ContentReader<T[]>
    {
        #region Fields

        private const char ValueSeparator = ',';
        private const string FirstBracket = "[";
        private const string LastBracket = "]";

        private ReaderManager _manager;
        private IContentReader _reader;

        #endregion

        public override void Initialize(ReaderManager manager)
        {
            if(manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
            _reader = _manager.GetReader(typeof (T));
        }

        public override T[] Read(string value, ReaderContext context)
        {
            if (value.StartsWith(FirstBracket))
                value = value.Substring(1);
            if (value.EndsWith(LastBracket))
                value = value.Substring(0, value.Length);

            string[] splitValue = value.Split(ValueSeparator);
            T[] result = new T[splitValue.Length];
            for (int i = 0; i < splitValue.Length; i++)
                result[i] = (T)_reader.Read(splitValue[i], context);

            return result;
        }
    }
}
