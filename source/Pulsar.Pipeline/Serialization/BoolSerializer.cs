namespace Pulsar.Pipeline.Serialization
{
    [ContentReader]
    public sealed class BoolSerializer : ContentSerializer<bool>
    {
        #region Constructors

        internal BoolSerializer()
        {
        }

        #endregion

        #region Methods

        public override bool Read(string value, SerializerContext context)
        {
            string cleanValue = value.Trim();

            return bool.Parse(cleanValue);
        }

        #endregion
    }
}
