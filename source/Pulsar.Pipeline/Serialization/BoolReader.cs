namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class BoolReader : ContentReader<bool>
    {
        #region Constructors

        internal BoolReader()
        {
        }

        #endregion

        #region Methods

        public override bool Read(string value, ReaderContext context)
        {
            string cleanValue = value.Trim();

            return bool.Parse(cleanValue);
        }

        #endregion
    }
}
