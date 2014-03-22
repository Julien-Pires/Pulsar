using System;

namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class Int32Reader : ContentReader<Int32>
    {
        #region Constructors

        internal Int32Reader()
        {
        }

        #endregion

        #region Methods

        public override int Read(string value, ReaderContext context)
        {
            string cleanValue = value.Trim();

            return Int32.Parse(cleanValue);
        }

        #endregion
    }
}
