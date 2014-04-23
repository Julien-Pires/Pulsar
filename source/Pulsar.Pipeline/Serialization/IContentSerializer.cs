using System;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Describes a data serializer for a specific type
    /// </summary>
    public interface IContentSerializer
    {
        #region Methods

        /// <summary>
        /// Initialize the serializer
        /// </summary>
        /// <param name="manager">Reader manager that owns this serializer</param>
        void Initialize(ReaderManager manager);

        /// <summary>
        /// Converts a string to an object
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns an object</returns>
        object Read(string value, SerializerContext context = null);

        /// <summary>
        /// Converts an object to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the object</returns>
        string Write(object value);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type associated to the serializer
        /// </summary>
        Type TargetType { get; }

        #endregion
    }
}
