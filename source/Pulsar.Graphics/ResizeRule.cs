namespace Pulsar.Graphics
{
    /// <summary>
    /// Defines how the screen resolution is resolved when the game switch in full screen mode
    /// </summary>
    public enum ResizeRule
    {
        /// <summary>
        /// Doesn't change the resolution
        /// </summary>
        None,

        /// <summary>
        /// Use the lowest resolution
        /// </summary>
        Lowest,

        /// <summary>
        /// Use the highest resolution
        /// </summary>
        Highest,

        /// <summary>
        /// Use the nearest resolution compared to the actual 
        /// </summary>
        Nearest
    }
}