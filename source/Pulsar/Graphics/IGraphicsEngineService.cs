using System;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Interface describing a service managing a graphic engine
    /// </summary>
    interface IGraphicsEngineService
    {
        #region Fields

        /// <summary>
        /// Get the graphic engine
        /// </summary>
        GraphicsEngine Engine { get; }

        #endregion
    }
}
