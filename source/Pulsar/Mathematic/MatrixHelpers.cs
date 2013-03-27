using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Mathematic
{
    /// <summary>
    /// Extension class for the Matrix class
    /// </summary>
    public sealed class MatrixExtension
    {
        #region Methods

        /// <summary>
        /// Create a matrix from three axis(x,y,z)
        /// </summary>
        /// <param name="xAxe">X axis</param>
        /// <param name="yAxe">Y axis</param>
        /// <param name="zAxe">Z axis</param>
        /// <returns>Return a new matrix made of the three axis</returns>
        public static Matrix CreateFromAxes(Vector3 xAxe, Vector3 yAxe, Vector3 zAxe)
        {
            Matrix m = new Matrix();

            m.M11 = xAxe.X;
            m.M21 = xAxe.Y;
            m.M31 = xAxe.Z;

            m.M12 = yAxe.X;
            m.M22 = yAxe.Y;
            m.M32 = yAxe.Z;

            m.M13 = zAxe.X;
            m.M23 = zAxe.Y;
            m.M33 = zAxe.Z;

            return m;
        }

        /// <summary>
        /// Create a matrix from three axis(x,y,z)
        /// </summary>
        /// <param name="xAxe">X axis</param>
        /// <param name="yAxe">Y axis</param>
        /// <param name="zAxe">Z axis</param>
        /// <param name="result">The new matrix</param>
        public static void CreateFromAxes(ref Vector3 xAxe, ref Vector3 yAxe, ref Vector3 zAxe, out Matrix result)
        {
            result = new Matrix();

            result.M11 = xAxe.X;
            result.M21 = xAxe.Y;
            result.M31 = xAxe.Z;
            result.M41 = 0f;
            
            result.M12 = yAxe.X;
            result.M22 = yAxe.Y;
            result.M32 = yAxe.Z;
            result.M42 = 0f;

            result.M13 = zAxe.X;
            result.M23 = zAxe.Y;
            result.M33 = zAxe.Z;
            result.M43 = 0f;

            result.M14 = 0f;
            result.M24 = 0f;
            result.M34 = 0f;
            result.M44 = 1f;
        }

        #endregion
    }
}
