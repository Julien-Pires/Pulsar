using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class MatrixReader : ContentReader<Matrix>
    {
        #region Constructors

        internal MatrixReader()
        {
        }

        #endregion

        #region Methods

        public override Matrix Read(string value, ReaderContext context)
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

        #endregion
    }
}
