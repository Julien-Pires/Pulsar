using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for Matrix
    /// </summary>
    [ContentReader]
    public sealed class MatrixSerializer : ContentSerializer<Matrix>
    {
        #region Constructors

        /// <summary>
        /// Constructor of MatrixSerializer class
        /// </summary>
        internal MatrixSerializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a Matrix
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a Matrix</returns>
        public override Matrix Deserialize(string value, SerializerContext context = null)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Matrix
            {
                M11 = Single.Parse(splitVal[0]),
                M12 = Single.Parse(splitVal[1]),
                M13 = Single.Parse(splitVal[2]),
                M14 = Single.Parse(splitVal[3]),

                M21 = Single.Parse(splitVal[4]),
                M22 = Single.Parse(splitVal[5]),
                M23 = Single.Parse(splitVal[6]),
                M24 = Single.Parse(splitVal[7]),

                M31 = Single.Parse(splitVal[8]),
                M32 = Single.Parse(splitVal[9]),
                M33 = Single.Parse(splitVal[10]),
                M34 = Single.Parse(splitVal[11]),

                M41 = Single.Parse(splitVal[12]),
                M42 = Single.Parse(splitVal[13]),
                M43 = Single.Parse(splitVal[14]),
                M44 = Single.Parse(splitVal[15]),
            };
        }

        /// <summary>
        /// Converts a Matrix to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the Matrix</returns>
        public override string Serialize(Matrix value)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(value.M11).Append(' ');
            builder.Append(value.M12).Append(' ');
            builder.Append(value.M13).Append(' ');
            builder.Append(value.M14).Append(' ');

            builder.Append(value.M21).Append(' ');
            builder.Append(value.M22).Append(' ');
            builder.Append(value.M23).Append(' ');
            builder.Append(value.M24).Append(' ');

            builder.Append(value.M31).Append(' ');
            builder.Append(value.M32).Append(' ');
            builder.Append(value.M33).Append(' ');
            builder.Append(value.M34).Append(' ');

            builder.Append(value.M41).Append(' ');
            builder.Append(value.M42).Append(' ');
            builder.Append(value.M43).Append(' ');
            builder.Append(value.M44).Append(' ');

            return builder.ToString();
        }

        #endregion
    }
}
