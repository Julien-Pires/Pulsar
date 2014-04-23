using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for Vector4
    /// </summary>
    [ContentReader]
    public sealed class Vector4Serializer : ContentSerializer<Vector4>
    {
        #region Constructors

        /// <summary>
        /// Constructor of Vector4Serializer class
        /// </summary>
        internal Vector4Serializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a Vector4
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a Vector4</returns>
        public override Vector4 Read(string value, SerializerContext context = null)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Vector4
            {
                X = Single.Parse(splitVal[0]),
                Y = Single.Parse(splitVal[1]),
                Z = Single.Parse(splitVal[2]),
                W = Single.Parse(splitVal[3])
            };
        }

        /// <summary>
        /// Converts a Vector4 to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the Vector4</returns>
        public override string Write(Vector4 value)
        {
            return value.X + " " + value.Y + " " + value.Z + " " + value.W; 
        }

        #endregion
    }
}
