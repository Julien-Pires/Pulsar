namespace Pulsar.Input
{
    /// <summary>
    /// Interface for input service provider
    /// </summary>
    public interface IInputService
    {
        #region Properties

        /// <summary>
        /// Gets the InputManager instance
        /// </summary>
        InputManager Input { get; }

        #endregion
    }
}
