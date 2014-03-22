using System;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Int32Serializer : ContentSerializer<Int32>
    {
        #region Constructors

        internal Int32Serializer()
        {
        }

        #endregion

        #region Methods

        public override int Read(string value, SerializerContext context)
        {
            string cleanValue = value.Trim();

            return Int32.Parse(cleanValue);
        }

        #endregion
    }
}
