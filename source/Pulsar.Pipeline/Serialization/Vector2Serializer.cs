using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for Vector2
    /// </summary>
    [ContentReader]
    public sealed class Vector2Serializer : ContentSerializer<Vector2>
    {
        #region Constructors

        /// <summary>
        /// Constructor of Vector2Serializer class
        /// </summary>
        internal Vector2Serializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a Vector2
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a Vector2</returns>
        public override Vector2 Deserialize(string value, SerializerContext context = null)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Vector2
            {
                X = Single.Parse(splitVal[0]), 
                Y = Single.Parse(splitVal[1])
            };
        }

        /// <summary>
        /// Converts a Vector2 to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the Vector2</returns>
        public override string Serialize(Vector2 value)
        {
            return value.X + " " + value.Y;
        }

        #endregion
    }
}
