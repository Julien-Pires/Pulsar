namespace Pulsar.Input
{
    /// <summary>
    /// Delegate used to check the state of a command
    /// </summary>
    /// <returns>Return true if the command is triggered otherwise false</returns>
    internal delegate bool CheckCommandState();

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
