namespace Pulsar.Input
{
    /// <summary>
    /// Represents a player index
    /// </summary>
    internal sealed class PlayerIndex
    {
        #region Properties

        /// <summary>
        /// Gets the player index
        /// </summary>
        public short Index { get; internal set; }

        /// <summary>
        /// Gets the gamepad index
        /// </summary>
        public short GamePadIndex { get; internal set; }

        #endregion
    }
}
