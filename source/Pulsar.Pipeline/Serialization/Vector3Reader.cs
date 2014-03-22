using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Vector3Reader : ContentReader<Vector3>
    {
        #region Constructors

        internal Vector3Reader()
        {
        }

        #endregion

        #region Methods

        public override Vector3 Read(string value, ReaderContext context)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Vector3
            {
                X = Single.Parse(splitVal[0]),
                Y = Single.Parse(splitVal[1]),
                Z = Single.Parse(splitVal[2])
            };
        }

        #endregion
    }
}
