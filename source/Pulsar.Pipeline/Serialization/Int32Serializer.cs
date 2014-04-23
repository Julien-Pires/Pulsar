using System;
using System.Globalization;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for int32
    /// </summary>
    [ContentReader]
    public sealed class Int32Serializer : ContentSerializer<Int32>
    {
        #region Constructors

        /// <summary>
        /// Constructor of Int32Serializer class
        /// </summary>
        internal Int32Serializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to an int
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns an int</returns>
        public override int Read(string value, SerializerContext context = null)
        {
            string cleanValue = value.Trim();

            return Int32.Parse(cleanValue);
        }

        /// <summary>
        /// Converts an int to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the int value</returns>
        public override string Write(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
