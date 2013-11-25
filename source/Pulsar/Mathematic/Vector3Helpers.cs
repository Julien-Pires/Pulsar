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

        /// <summary>
        /// Creates an absolute vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <returns>Returns a vector with the absolute value for each components</returns>
        public static Vector3 Abs(Vector3 v)
        {
#if XBOX || XBOX360
            Vector3 result = new Vector3();
#else
            Vector3 result;
#endif
            result.X = Math.Abs(v.X);
            result.Y = Math.Abs(v.Y);
            result.Z = Math.Abs(v.Z);

            return result;
        }

        /// <summary>
        /// Creates an absolute vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <param name="result">Destination vector</param>
        public static void Abs(ref Vector3 v, out Vector3 result)
        {
#if XBOX || XBOX360
            result = new Vector3();
#endif
            result.X = Math.Abs(v.X);
            result.Y = Math.Abs(v.Y);
            result.Z = Math.Abs(v.Z);
        }

        /// <summary>
        /// Creates a signed vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <returns>Returns a vector with a value indicating the sign for each components</returns>
        public static Vector3 Sign(Vector3 v)
        {
#if XBOX || XBOX360
            Vector3 result = new Vector3();
#else
            Vector3 result;
#endif
            result.X = Math.Sign(v.X);
            result.Y = Math.Sign(v.Y);
            result.Z = Math.Sign(v.Z);

            return result;
        }

        /// <summary>
        /// Creates a signed vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <param name="result">Destination vector</param>
        public static void Sign(ref Vector3 v, out Vector3 result)
        {
#if XBOX || XBOX360
            result = new Vector3();
#endif
            result.X = Math.Sign(v.X);
            result.Y = Math.Sign(v.Y);
            result.Z = Math.Sign(v.Z);
        }

        /// <summary>
        /// Creates a flipped signed vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <returns>Returns a vector with a value indicating the opposite sign for each components</returns>
        public static Vector3 FlipSign(Vector3 v)
        {
#if XBOX || XBOX360
            Vector3 result = new Vector3();
#else
            Vector3 result;
#endif
            result.X = -1.0f * Math.Sign(v.X);
            result.Y = -1.0f * Math.Sign(v.Y);
            result.Z = -1.0f * Math.Sign(v.Z);

            return result;
        }

        /// <summary>
        /// Creates a flipped signed vector from the specified vector
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <param name="result">Destination vector</param>
        public static void FlipSign(ref Vector3 v, out Vector3 result)
        {
#if XBOX || XBOX360
            result = new Vector3();
#endif
            result.X = -1.0f * Math.Sign(v.X);
            result.Y = -1.0f * Math.Sign(v.Y);
            result.Z = -1.0f * Math.Sign(v.Z);
        }

        #endregion
    }
}
