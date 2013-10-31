using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Mathematic
{
    /// <summary>
    /// Extension class for the Vector3 class
    /// </summary>
    public static class Vector3Extension
    {
        #region Methods

        /// <summary>
        /// Checks if a vector has a length of zero (or extremly near of zero)
        /// </summary>
        /// <param name="v">The vector from which the length is checked</param>
        /// <returns>Return true if the vector has a length of zero, otherwise false</returns>
        public static bool IsZeroLength(this Vector3 v)
        {
            float l = (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z);
            return l < (1e-06 * 1e-06);
        }

        #endregion
    }
}
