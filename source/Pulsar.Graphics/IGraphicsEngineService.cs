﻿namespace Pulsar.Graphics
{
    /// <summary>
    /// Interface describing a service managing a graphic engine
    /// </summary>
    public interface IGraphicsEngineService
    {
        #region Properties

        /// <summary>
        /// Get the graphic engine
        /// </summary>
        GraphicsEngine Engine { get; }

        #endregion
    }
}
