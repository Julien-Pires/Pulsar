using System;
using System.Collections.Generic;
using System.Reflection;

using Pulsar.System;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class ReaderManager
    {
        #region Fields

        private const BindingFlags CtorFlag = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private readonly Dictionary<Type, IContentSerializer> _readersMap = new Dictionary<Type, IContentSerializer>();

        #endregion

        #region Constructors

        public ReaderManager()
        {
            TypeDetector detector = new TypeDetector
            {
                BaseType = typeof (ContentSerializer<>),
                Exclude = (TypeDetectorRule.Abstract | TypeDetectorRule.Interface | TypeDetectorRule.Private |
                           TypeDetectorRule.ValueType | TypeDetectorRule.NoParameterLessCtor | TypeDetectorRule.Nested)
            };
            detector.Attributes.Add(typeof(ContentReaderAttribute));

            foreach (Type type in detector.GetTypes())
            {
                ConstructorInfo ctorInfo = type.GetConstructor(CtorFlag, null, Type.EmptyTypes, null);
                if(ctorInfo == null)
                    throw new Exception("");

                IContentSerializer serializer = (IContentSerializer)ctorInfo.Invoke(null);
                serializer.Initialize(this);
                _readersMap.Add(serializer.TargetType, serializer);
            }
        }

        #endregion

        #region Methods

        public object Read(Type type, string value, SerializerContext context = null)
        {
            IContentSerializer serializer = GetReader(type);

            return serializer.Read(value, context);
        }

        public T Read<T>(string value, SerializerContext context = null)
        {
            return (T)Read(typeof(T), value, context);
        }

        public object[] ReadMultiples(Type type, IList<string> values, SerializerContext[] contextArr = null)
        {
            IContentSerializer serializer = GetReader(type);
            List<object> result = new List<object>();
            for(int i = 0; i < values.Count; i++)
            {
                SerializerContext context = null;
                if (contextArr != null)
                    context = contextArr[i];

                result.Add(serializer.Read(values[i], context));
            }

            return result.ToArray();
        }

        public T[] ReadMultiples<T>(IList<string> values, SerializerContext[] contextArr = null)
        {
            object[] result = ReadMultiples(typeof (T), values, contextArr);

            return Array.ConvertAll(result, c => (T) c);
        }

        public IContentSerializer GetReader(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            IContentSerializer serializer;
            if (!_readersMap.TryGetValue(type, out serializer))
            {
                serializer = CreateReader(type);
                _readersMap.Add(type, serializer);
            }

            return serializer;
        }

        private IContentSerializer CreateReader(Type type)
        {
            if (type.IsArray)
            {
                if(type.GetArrayRank() != 1)
                    throw new RankException("");

                Type newType = typeof (ArraySerializer<>).MakeGenericType(type.GetElementType());

                return (IContentSerializer) Activator.CreateInstance(newType);
            }

            throw new Exception("");
        }

        #endregion
    }
}
