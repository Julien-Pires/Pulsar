using System;

using Microsoft.Xna.Framework;

namespace PulsarRuntime.Graphics
{
    /// <summary>
    /// Struct containing bounding volume data(AABB, sphere, ...)
    /// </summary>
    public struct BoundingData
    {
        #region Fields

        /// <summary>
        /// Axis Aligned Bounding Box
        /// </summary>
        public BoundingBox BoundingBox;

        /// <summary>
        /// Bounding sphere
        /// </summary>
        public BoundingSphere BoundingSphere;

        #endregion
    }
}
