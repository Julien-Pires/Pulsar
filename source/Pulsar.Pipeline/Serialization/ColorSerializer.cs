using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class ColorSerializer : ContentSerializer<Color>
    {
        #region Fields

        private const string HexPattern = "^[a-fA-F0-9]{6}$";

        #endregion

        #region Constructors

        internal ColorSerializer()
        {
        }

        #endregion

        #region Methods

        public override Color Read(string value, SerializerContext context)
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

        #endregion
    }
}
