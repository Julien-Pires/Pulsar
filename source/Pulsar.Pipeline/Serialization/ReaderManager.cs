using System;
using System.Collections.Generic;

using Pulsar.System;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class ReaderManager
    {
        #region Fields

        private readonly Dictionary<Type, IContentReader> _readersMap = new Dictionary<Type, IContentReader>();

        #endregion

        #region Constructors

        public ReaderManager()
        {
            TypeDetector detector = new TypeDetector
            {
                BaseType = typeof (ContentReader<>),
                Exclude = (TypeDetectorRule.Abstract | TypeDetectorRule.Interface | TypeDetectorRule.Private |
                           TypeDetectorRule.ValueType | TypeDetectorRule.NoParameterLessCtor | TypeDetectorRule.Nested)
            };
            detector.Attributes.Add(typeof(ContentReaderAttribute));

            foreach (Type type in detector.GetTypes())
            {
                IContentReader reader = (IContentReader)Activator.CreateInstance(type);
                _readersMap.Add(reader.TargetType, reader);
            }
        }

        #endregion

        #region Methods

        public object Read(Type type, string value, ReaderContext context = null)
        {
            IContentReader reader = GetReader(type);

            return reader.Read(value, context);
        }

        public T Read<T>(string value, ReaderContext context = null)
        {
            return (T)Read(typeof(T), value, context);
        }

        public object[] ReadMultiples(Type type, IList<string> values, ReaderContext[] contextArr = null)
        {
            IContentReader reader = GetReader(type);
            List<object> result = new List<object>();
            for(int i = 0; i < values.Count; i++)
            {
                ReaderContext context = null;
                if (contextArr != null)
                    context = contextArr[i];

                result.Add(reader.Read(values[i], context));
            }

            return result.ToArray();
        }

        public T[] ReadMultiples<T>(IList<string> values, ReaderContext[] contextArr = null)
        {
            object[] result = ReadMultiples(typeof (T), values, contextArr);

            return Array.ConvertAll(result, c => (T) c);
        }

        public IContentReader GetReader(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            IContentReader reader;
            if (!_readersMap.TryGetValue(type, out reader))
            {
                reader = CreateReader(type);
                _readersMap.Add(type, reader);
            }

            return reader;
        }

        private IContentReader CreateReader(Type type)
        {
            if (type.IsArray)
            {
                if(type.GetArrayRank() != 1)
                    throw new RankException("");

                Type newType = typeof (ArrayReader<>).MakeGenericType(type.GetElementType());

                return (IContentReader) Activator.CreateInstance(newType);
            }

            throw new Exception("");
        }

        #endregion
    }
}
