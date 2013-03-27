using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Graph
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
        public BoundingBox AxisAlignedBoundingBox;

        /// <summary>
        /// Bounding sphere
        /// </summary>
        public BoundingSphere BoundingSphere;

        #endregion
    }
}
