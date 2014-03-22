using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Vector4Reader : ContentReader<Vector4>
    {
        #region Constructors

        internal Vector4Reader()
        {
        }

        #endregion

        #region Methods

        public override Vector4 Read(string value, ReaderContext context)
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

        #endregion
    }
}
