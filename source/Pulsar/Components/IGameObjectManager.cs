namespace Pulsar.Components
{
    /// <summary>
    /// Provides mechanism to manage game objects
    /// </summary>
    public interface IGameObjectManager
    {
        #region Methods

        /// <summary>
        /// Adds a game object to this manager
        /// </summary>
        /// <param name="gameObject">Game object</param>
        void Added(GameObject gameObject);

        /// <summary>
        /// Removes a game object from this manager
        /// </summary>
        /// <param name="gameObject">Game object</param>
        void Removed(GameObject gameObject);

        #endregion
    }
}
