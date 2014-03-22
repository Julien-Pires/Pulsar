using System;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class SingleSerializer : ContentSerializer<float>
    {
        #region Constructors

        internal SingleSerializer()
        {
        }

        #endregion

        #region Methods

        public override float Read(string value, SerializerContext context)
        {
            string cleanValue = value.Trim();

            return Single.Parse(cleanValue);
        }

        #endregion
    }
}
