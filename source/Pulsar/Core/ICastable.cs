namespace Pulsar.Core
{
    public interface ICastable<T>
    {
        #region Methods

        T Cast();

        #endregion
    }
}
