using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for Vector3
    /// </summary>
    [ContentReader]
    public sealed class Vector3Serializer : ContentSerializer<Vector3>
    {
        #region Constructors

        /// <summary>
        /// Constructor of Vector3Serializer class
        /// </summary>
        internal Vector3Serializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a Vector3
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a Vector3</returns>
        public override Vector3 Read(string value, SerializerContext context = null)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Vector3
            {
                X = Single.Parse(splitVal[0]),
                Y = Single.Parse(splitVal[1]),
                Z = Single.Parse(splitVal[2])
            };
        }

        /// <summary>
        /// Converts a Vector3 to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the Vector3</returns>
        public override string Write(Vector3 value)
        {
            return value.X + " " + value.Y + " " + value.Z; 
        }

        #endregion
    }
}
