namespace Pulsar
{
    /// <summary>
    /// Interface for message listener
    /// </summary>
    public interface IEventHandler
    {
        #region Methods

        /// <summary>
        /// Process a message
        /// </summary>
        /// <param name="msg">Message</param>
        void HandleEvent(Message msg);

        #endregion
    }
}
