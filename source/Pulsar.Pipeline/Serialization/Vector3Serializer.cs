using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Vector3Serializer : ContentSerializer<Vector3>
    {
        #region Constructors

        internal Vector3Serializer()
        {
        }

        #endregion

        #region Methods

        public override Vector3 Read(string value, SerializerContext context)
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
