using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Vector2Serializer : ContentSerializer<Vector2>
    {
        #region Constructors

        internal Vector2Serializer()
        {
        }

        #endregion

        #region Methods

        public override Vector2 Read(string value, SerializerContext context)
        {
            string[] splitVal = MathSerializerHelper.Split(value);

            return new Vector2
            {
                X = Single.Parse(splitVal[0]), 
                Y = Single.Parse(splitVal[1])
            };
        }

        #endregion
    }
}
