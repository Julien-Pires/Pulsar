namespace Pulsar.Graphics
{
    internal interface IMaterialDataCollection<T> : IMaterialDataCollection
    {
        #region Operators

        T this[string key] { get; set; }

        #endregion

        #region Methods

        bool TryGetValue(string key, out T value);

        #endregion
    }
}
