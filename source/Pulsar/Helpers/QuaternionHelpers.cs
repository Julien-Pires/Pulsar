using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Helpers
{
    /// <summary>
    /// Extension class for the Quaternion class
    /// </summary>
    public static class QuaternionExtension
    {
        #region Methods

        /// <summary>
        /// Extract the three coordinate axis from a quaternion
        /// </summary>
        /// <param name="q">Quaternion instance</param>
        /// <returns>Return an array containing the three axis e</returns>
        public static Vector3[] GetAxes(this Quaternion q)
        {
            Matrix rotation;
            Matrix.CreateFromQuaternion(ref q, out rotation);

            Vector3[] axes = new Vector3[3];
            
            axes[0].X = rotation.M11;
            axes[0].Y = rotation.M21;
            axes[0].Z = rotation.M31;

            axes[1].X = rotation.M12;
            axes[1].Y = rotation.M22;
            axes[1].Z = rotation.M32;

            axes[2].X = rotation.M13;
            axes[2].Y = rotation.M23;
            axes[2].Z = rotation.M33;

            return axes;
        }

        /// <summary>
        /// Extract the three coordinate axis from a quaternion
        /// </summary>
        /// <param name="q">Quaternion instance</param>
        /// <param name="axes">An array containing the three axis</param>
        public static void GetAxes(this Quaternion q, out Vector3[] axes)
        {
            Matrix rotation;
            Matrix.CreateFromQuaternion(ref q, out rotation);

            axes = new Vector3[3];

            axes[0].X = rotation.M11;
            axes[0].Y = rotation.M21;
            axes[0].Z = rotation.M31;

            axes[1].X = rotation.M12;
            axes[1].Y = rotation.M22;
            axes[1].Z = rotation.M32;

            axes[2].X = rotation.M13;
            axes[2].Y = rotation.M23;
            axes[2].Z = rotation.M33;
        }

        #endregion
    }
}
