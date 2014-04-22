namespace Pulsar.Input
{
    /// <summary>
    /// Interface for all input command
    /// </summary>
    internal interface IInputCommand
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates if the command is triggered
        /// </summary>
        bool IsTriggered { get; }

        #endregion
    }
}
