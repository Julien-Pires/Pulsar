using Microsoft.Xna.Framework;

namespace Pulsar.Mathematic
{
    /// <summary>
    /// Extends the Matrix class
    /// </summary>
    public sealed class MatrixExtension
    {
        #region Methods

        /// <summary>
        /// Creates a matrix from three axis(x,y,z)
        /// </summary>
        /// <param name="xAxe">X axis</param>
        /// <param name="yAxe">Y axis</param>
        /// <param name="zAxe">Z axis</param>
        /// <returns>Return a new matrix made of the three axis</returns>
        public static Matrix CreateFromAxes(Vector3 xAxe, Vector3 yAxe, Vector3 zAxe)
        {
#if WINDOWS
            Matrix result;
#elif XBOX || XBOX360
            Matrix result = new Matrix();
#endif
            result.M11 = xAxe.X;
            result.M21 = xAxe.Y;
            result.M31 = xAxe.Z;
            result.M41 = 0.0f;

            result.M12 = yAxe.X;
            result.M22 = yAxe.Y;
            result.M32 = yAxe.Z;
            result.M42 = 0.0f;

            result.M13 = zAxe.X;
            result.M23 = zAxe.Y;
            result.M33 = zAxe.Z;
            result.M43 = 0.0f;

            result.M14 = 0f;
            result.M24 = 0f;
            result.M34 = 0f;
            result.M44 = 1f;

            return result;
        }

        /// <summary>
        /// Creates a matrix from three axis(x,y,z)
        /// </summary>
        /// <param name="xAxe">X axis</param>
        /// <param name="yAxe">Y axis</param>
        /// <param name="zAxe">Z axis</param>
        /// <param name="result">Destination matrix</param>
        public static void CreateFromAxes(ref Vector3 xAxe, ref Vector3 yAxe, ref Vector3 zAxe, out Matrix result)
        {
#if XBOX || XBOX360
            result = new Matrix();
#endif
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

        /// <summary>
        /// Creates a world matrix from three components
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="eulerAngle">Rotation</param>
        /// <param name="scale">Scale</param>
        /// <returns>Returns a world matrix</returns>
        public static Matrix CreateWorld(Vector3 position, Vector3 eulerAngle, Vector3 scale)
        {
            Quaternion rotation;
            Quaternion.CreateFromYawPitchRoll(eulerAngle.X, eulerAngle.Y, eulerAngle.Z, out rotation);

            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float xz = rotation.X * rotation.Z;
            float xw = rotation.X * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float yw = rotation.Y * rotation.W;
            float zw = rotation.Z * rotation.W;
#if WINDOWS
            Matrix result;
#elif XBOX || XBOX360
            Matrix result = new Matrix();
#endif
            // Initialize with rotation matrix
            result.M11 = 1f - 2f * (yy + zz);
            result.M12 = 2f * (xy + zw);
            result.M13 = 2f * (xz - yw);
            result.M14 = 0;
            result.M21 = 2f * (xy - zw);
            result.M22 = 1f - 2f * (xx + zz);
            result.M23 = 2f * (yz + xw);
            result.M24 = 0;
            result.M31 = 2f * (xz + yw);
            result.M32 = 2f * (yz - xw);
            result.M33 = 1f - 2f * (xx + yy);
            result.M34 = 0;
            result.M44 = 1f;

            // Multiply : scale * rotation
            result.M11 *= scale.X;
            result.M12 *= scale.X;
            result.M13 *= scale.X;
            result.M21 *= scale.Y;
            result.M22 *= scale.Y;
            result.M23 *= scale.Y;
            result.M31 *= scale.Z;
            result.M32 *= scale.Z;
            result.M33 *= scale.Z;

            // Add position
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;

            return result;
        }

        /// <summary>
        /// Creates a world matrix from three components
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="eulerAngle">Rotation</param>
        /// <param name="scale">Scale</param>
        /// <param name="result">Destination matrix</param>
        public static void CreateWorld(ref Vector3 position, ref Vector3 eulerAngle, ref Vector3 scale, out Matrix result)
        {
            Quaternion rotation;
            Quaternion.CreateFromYawPitchRoll(eulerAngle.X, eulerAngle.Y, eulerAngle.Z, out rotation);

#if XBOX || XBOX360
            result = new Matrix();
#endif
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float xz = rotation.X * rotation.Z;
            float xw = rotation.X * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float yw = rotation.Y * rotation.W;
            float zw = rotation.Z * rotation.W;

            // Initialize with rotation matrix
            result.M11 = 1f - 2f * (yy + zz);
            result.M12 = 2f * (xy + zw);
            result.M13 = 2f * (xz - yw);
            result.M14 = 0;
            result.M21 = 2f * (xy - zw);
            result.M22 = 1f - 2f * (xx + zz);
            result.M23 = 2f * (yz + xw);
            result.M24 = 0;
            result.M31 = 2f * (xz + yw);
            result.M32 = 2f * (yz - xw);
            result.M33 = 1f - 2f * (xx + yy);
            result.M34 = 0;
            result.M44 = 1f;

            // Multiply : scale * rotation
            result.M11 *= scale.X;
            result.M12 *= scale.X;
            result.M13 *= scale.X;
            result.M21 *= scale.Y;
            result.M22 *= scale.Y;
            result.M23 *= scale.Y;
            result.M31 *= scale.Z;
            result.M32 *= scale.Z;
            result.M33 *= scale.Z;

            // Add position
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
        }

        /// <summary>
        /// Creates a world matrix from three components 
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="scale">Scale</param>
        /// <returns>Returns a world matrix</returns>
        public static Matrix CreateWorld(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float xz = rotation.X * rotation.Z;
            float xw = rotation.X * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float yw = rotation.Y * rotation.W;
            float zw = rotation.Z * rotation.W;
#if WINDOWS
            Matrix result;
#elif XBOX || XBOX360
            Matrix result = new Matrix();
#endif
            // Initialize with rotation matrix
            result.M11 = 1f - 2f * (yy + zz);
            result.M12 = 2f * (xy + zw);
            result.M13 = 2f * (xz - yw);
            result.M14 = 0;
            result.M21 = 2f * (xy - zw);
            result.M22 = 1f - 2f * (xx + zz);
            result.M23 = 2f * (yz + xw);
            result.M24 = 0;
            result.M31 = 2f * (xz + yw);
            result.M32 = 2f * (yz - xw);
            result.M33 = 1f - 2f * (xx + yy);
            result.M34 = 0;
            result.M44 = 1f;

            // Multiply : scale * rotation
            result.M11 *= scale.X;
            result.M12 *= scale.X;
            result.M13 *= scale.X;
            result.M21 *= scale.Y;
            result.M22 *= scale.Y;
            result.M23 *= scale.Y;
            result.M31 *= scale.Z;
            result.M32 *= scale.Z;
            result.M33 *= scale.Z;

            // Add position
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;

            return result;
        }

        /// <summary>
        /// Creates a world matrix from three components 
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="scale">Scale</param>
        /// <param name="result">Destination matrix</param>
        public static void CreateWorld(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale, out Matrix result)
        {
#if XBOX || XBOX360
            result = new Matrix();
#endif
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float xz = rotation.X * rotation.Z;
            float xw = rotation.X * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float yw = rotation.Y * rotation.W;
            float zw = rotation.Z * rotation.W;

            // Initialize with rotation matrix
            result.M11 = 1f - 2f * (yy + zz);
            result.M12 = 2f * (xy + zw);
            result.M13 = 2f * (xz - yw);
            result.M14 = 0;
            result.M21 = 2f * (xy - zw);
            result.M22 = 1f - 2f * (xx + zz);
            result.M23 = 2f * (yz + xw);
            result.M24 = 0;
            result.M31 = 2f * (xz + yw);
            result.M32 = 2f * (yz - xw);
            result.M33 = 1f - 2f * (xx + yy);
            result.M34 = 0;
            result.M44 = 1f;

            // Multiply : scale * rotation
            result.M11 *= scale.X;
            result.M12 *= scale.X;
            result.M13 *= scale.X;
            result.M21 *= scale.Y;
            result.M22 *= scale.Y;
            result.M23 *= scale.Y;
            result.M31 *= scale.Z;
            result.M32 *= scale.Z;
            result.M33 *= scale.Z;

            // Add position
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
        }

        #endregion
    }
}
