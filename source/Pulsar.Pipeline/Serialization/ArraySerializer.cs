using System;
using System.Text;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for a generic array
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public sealed class ArraySerializer<T> : ContentSerializer<T[]>
    {
        #region Fields

        private const char ValueSeparator = ',';
        private const string FirstBracket = "[";
        private const string LastBracket = "]";

        private ReaderManager _manager;
        private IContentSerializer _serializer;

        #endregion

        /// <summary>
        /// Initializes the serializer
        /// </summary>
        /// <param name="manager">Reader manager that owns this serializer</param>
        public override void Initialize(ReaderManager manager)
        {
            if(manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
            _serializer = _manager.GetReader(typeof (T));
        }

        /// <summary>
        /// Converts a string to an array of T
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns an array of T</returns>
        public override T[] Read(string value, SerializerContext context = null)
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

        /// <summary>
        /// Converts an array of T to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the array</returns>
        public override string Write(T[] value)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            for (int i = 0; i < value.Length; i++)
                builder.Append("\" ").Append(_serializer.Write(value[i])).Append(" \",");
            builder.Append(']');

            return builder.ToString();
        }
    }
}
