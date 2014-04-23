using System;
using System.Reflection;
using System.Collections.Generic;

using Pulsar.System;

namespace Pulsar.Pipeline.Serialization
{
    public sealed class SerializerManager
    {
        #region Fields

        private const BindingFlags CtorFlag = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly MethodInfo DeserializeInfo = typeof (SerializerManager).GetMethod("Deserialize",
            new[] {typeof(string), typeof(SerializerContext)});
        private static readonly MethodInfo DeserializeArrayInfo = typeof(SerializerManager).GetMethod("Deserialize",
            new[] { typeof(IList<string>), typeof(SerializerContext[]) });

        private readonly Dictionary<Type, IContentSerializer> _serializerMap =
            new Dictionary<Type, IContentSerializer>();

        #endregion

        #region Constructors

        public SerializerManager()
        {
            TypeDetector detector = new TypeDetector
            {
                BaseType = typeof (IContentSerializer),
                Exclude = (TypeDetectorRule.Abstract | TypeDetectorRule.Interface | TypeDetectorRule.Private |
                           TypeDetectorRule.ValueType | TypeDetectorRule.NoParameterLessCtor | TypeDetectorRule.Nested)
            };
            detector.Attributes.Add(typeof(ContentReaderAttribute));

            foreach (Type type in detector.GetTypes())
            {
                ConstructorInfo ctorInfo = type.GetConstructor(CtorFlag, null, Type.EmptyTypes, null);
                if(ctorInfo == null)
                    throw new Exception("Unable to find a suitable parameterless constructor");

                IContentSerializer serializer = (IContentSerializer)ctorInfo.Invoke(null);
                serializer.Initialize(this);
                _serializerMap.Add(serializer.TargetType, serializer);
            }
        }

        #endregion

        #region Static methods

        private static IContentSerializer CreateSerializer(Type type)
        {
            if (!type.IsArray) 
                throw new Exception(string.Format("Failed to create a serializer for type {0}", type));

            if (type.GetArrayRank() != 1)
                throw new RankException("Cannot create a serializer for array with rank superior to 1");

            Type newType = typeof(ArraySerializer<>).MakeGenericType(type.GetElementType());

            return (IContentSerializer)Activator.CreateInstance(newType);
        }

        #endregion

        #region Methods

        public object Deserialize(Type type, string value, SerializerContext context = null)
        {
            MethodInfo genericMethod = DeserializeInfo.MakeGenericMethod(new[] {type});

            return genericMethod.Invoke(this, new object[]{value, context});
        }

        public T Deserialize<T>(string value, SerializerContext context = null)
        {
            IContentSerializer serializer = GetSerializer(typeof (T));

            return InvokeDeserialize<T>(value, serializer, context);
        }

        public object Deserialize(Type type, IList<string> values, SerializerContext[] contextArr = null)
        {
            MethodInfo genericMethod = DeserializeArrayInfo.MakeGenericMethod(new[] {type});

            return genericMethod.Invoke(this, new object[] {values, contextArr});
        }

        public T[] Deserialize<T>(IList<string> values, SerializerContext[] contextArr = null)
        {
            T[] result = new T[values.Count];
            IContentSerializer serializer = GetSerializer(typeof(T));
            for (int i = 0; i < values.Count; i++)
            {
                SerializerContext context = null;
                if ((contextArr != null) && (contextArr.Length > 0))
                {
                    int index = Math.Min(i, contextArr.Length - 1);
                    context = contextArr[index];
                }

                result[i] = InvokeDeserialize<T>(values[i], serializer, context);
            }

            return result;
        }

        private T InvokeDeserialize<T>(string value, IContentSerializer serializer, SerializerContext context)
        {
            ContentSerializer<T> typeSerializer = serializer as ContentSerializer<T>;
            T result;
            if (typeSerializer == null)
            {
                object obj = serializer.Deserialize(value, context) ?? default(T);
                result = (T)obj;
            }
            else
                result = typeSerializer.Deserialize(value, context);

            return result;
        }

        private string InvokeSerialize<T>(T value, IContentSerializer serializer, SerializerContext context)
        {
            ContentSerializer<T> typeSerializer = serializer as ContentSerializer<T>;

            return (typeSerializer == null) ? serializer.Serialize(value) : typeSerializer.Serialize(value);
        }

        public IContentSerializer GetSerializer(Type type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            IContentSerializer serializer;
            if (_serializerMap.TryGetValue(type, out serializer)) 
                return serializer;

            serializer = CreateSerializer(type);
            _serializerMap.Add(type, serializer);

            return serializer;
        }

        #endregion
    }
}
