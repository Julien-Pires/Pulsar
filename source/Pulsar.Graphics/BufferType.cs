namespace Pulsar.Graphics
{
    /// <summary>
    /// Enumerates type of buffer
    /// </summary>
    public enum BufferType
    {
        /// <summary>
        /// A buffer used for static vertices
        /// </summary>
        Static,
        /// <summary>
        /// A buffer used for static vertices in write only mode
        /// </summary>
        StaticWriteOnly,
        /// <summary>
        /// A buffer used when vertices are often replaced
        /// </summary>
        Dynamic,
        /// <summary>
        /// A buffer used when vertices are often replaced and is used only in write mode
        /// </summary>
        DynamicWriteOnly
    }
}