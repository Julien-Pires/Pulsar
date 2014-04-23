namespace Pulsar.Graphics
{
    /// <summary>
    /// Interface describing a service managing a graphic engine
    /// </summary>
    public interface IGraphicsEngineService
    {
        #region Properties

        /// <summary>
        /// Gets the graphic engine
        /// </summary>
        GraphicsEngine Engine { get; }

        #endregion
    }
}
