using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Vector2Reader : ContentReader<Vector2>
    {
        #region Constructors

        internal Vector2Reader()
        {
        }

        #endregion

        #region Methods

        public override Vector2 Read(string value, ReaderContext context)
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
