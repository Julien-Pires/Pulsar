using System;
using System.Globalization;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for float
    /// </summary>
    [ContentReader]
    public sealed class SingleSerializer : ContentSerializer<float>
    {
        #region Constructors

        /// <summary>
        /// Constructor of SingleSerializer class
        /// </summary>
        internal SingleSerializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a float
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a float</returns>
        public override float Deserialize(string value, SerializerContext context = null)
        {
            string cleanValue = value.Trim();

            return Single.Parse(cleanValue);
        }

        /// <summary>
        /// Converts a float to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the float value</returns>
        public override string Serialize(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
