namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for boolean
    /// </summary>
    [ContentReader]
    public sealed class BoolSerializer : ContentSerializer<bool>
    {
        #region Constructors

        /// <summary>
        /// Constructor of BoolSerializer class
        /// </summary>
        internal BoolSerializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a bool
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a bool</returns>
        public override bool Deserialize(string value, SerializerContext context = null)
        {
            string cleanValue = value.Trim();

            return bool.Parse(cleanValue);
        }

        /// <summary>
        /// Converts a bool to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the bool value</returns>
        public override string Serialize(bool value)
        {
            return value.ToString();
        }

        #endregion
    }
}
