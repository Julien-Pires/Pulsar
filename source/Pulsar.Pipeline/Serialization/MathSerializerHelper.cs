namespace Pulsar.Pipeline.Serialization
{
    public static class MathSerializerHelper
    {
        #region Fields

        private static readonly char[] Separators = { ' ', '\t', '\n', '\r' };

        #endregion

        #region Static methods

        public static int GetValueCount(string value)
        {
            return Split(value).Length;
        }

        public static string[] Split(string value)
        {
            return Split(value, Separators);
        }

        public static string[] Split(string value, char[] separators)
        {
            return value.Split(separators);
        }

        #endregion
    }
}
