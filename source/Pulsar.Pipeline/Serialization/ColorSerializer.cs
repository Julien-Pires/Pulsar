using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    /// <summary>
    /// Represents a serializer for Color
    /// </summary>
    [ContentReader]
    public sealed class ColorSerializer : ContentSerializer<Color>
    {
        #region Fields

        private const string HexPattern = "^#{0,1}(([a-fA-F0-9]{6})|([a-fA-F0-9]{8})){1}$";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ColorSerializer class
        /// </summary>
        internal ColorSerializer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a string to a Color
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="context">Current context</param>
        /// <returns>Returns a Color instance</returns>
        public override Color Read(string value, SerializerContext context = null)
        {
            if (Regex.IsMatch(value, HexPattern))
            {
                Color color = new Color
                {
                    PackedValue = uint.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
                };
                
                return color;
            }

            string[] splitValue = MathSerializerHelper.Split(value);
            if ((splitValue.Length <= 0) && (splitValue.Length >= 5)) 
                throw new Exception("Invalid color format");

            float[] parsedValues = new float[4];
            for (int i = 0; i < splitValue.Length; i++)
                parsedValues[i] = Single.Parse(splitValue[i]);

            Vector4 vec4 = new Vector4(parsedValues[0], parsedValues[1], parsedValues[2], parsedValues[3]);

            return new Color(vec4);
        }

        /// <summary>
        /// Converts a Color instance to a string representation
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a string that represents the Color instance</returns>
        public override string Write(Color value)
        {
            return "#" + value.A.ToString("X2") + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2");
        }

        #endregion
    }
}
