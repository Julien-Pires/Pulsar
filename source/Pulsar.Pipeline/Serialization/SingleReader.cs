using System;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class SingleReader : ContentReader<float>
    {
        #region Constructors

        internal SingleReader()
        {
        }

        #endregion

        #region Methods

        public override float Read(string value, ReaderContext context)
        {
            string cleanValue = value.Trim();

            return Single.Parse(cleanValue);
        }

        #endregion
    }
}
