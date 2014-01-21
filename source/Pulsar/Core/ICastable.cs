namespace Pulsar.Core
{
    /// <summary>
    /// Provides a mechanism to cast an object to a specific type
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public interface ICastable<out T>
    {
        #region Methods

        /// <summary>
        /// Casts this instance to a specified type
        /// </summary>
        /// <returns>Returns an instance of T</returns>
        T Cast();

        #endregion
    }
}
