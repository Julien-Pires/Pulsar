namespace Pulsar.Components
{
    /// <summary>
    /// A component defines game object behaviors
    /// </summary>
    public abstract class Component
    {
        #region Properties

        /// <summary>
        /// Get or set the owner of this component
        /// </summary>
        public GameObject Parent { get; internal set; }

        #endregion
    }
}
