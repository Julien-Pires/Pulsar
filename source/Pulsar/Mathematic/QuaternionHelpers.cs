using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Mathematic
{
    /// <summary>
    /// Extension class for the Quaternion class
    /// </summary>
    public static class QuaternionExtension
    {
        #region Methods

        /// <summary>
        /// Extracts the three coordinates axis from a quaternion
        /// </summary>
        /// <param name="q">Rotation</param>
        /// <returns>Return an array containing the three axis</returns>
        public static Vector3[] GetAxis(this Quaternion q)
        {
            float xx = q.X * q.X;
            float yy = q.Y * q.Y;
            float zz = q.Z * q.Z;
            float xy = q.X * q.Y;
            float xz = q.X * q.Z;
            float xw = q.X * q.W;
            float yz = q.Y * q.Z;
            float yw = q.Y * q.W;
            float zw = q.Z * q.W;

            Vector3[] axis = new Vector3[3];
            axis[0].X = 1f - 2f * (yy + zz);
            axis[0].Y = 2 * (xy + zw);
            axis[0].Z = 2 * (xz - yw);

            axis[1].X = 2f * (xy - zw);
            axis[1].Y = 1f - 2f * (xx + zz);
            axis[1].Z = 2f * (yz + xw);

            axis[2].X = -2f * (xz + yw);
            axis[2].Y = -2f * (yz - xw);
            axis[2].Z = -1f + 2f * (xx + yy);

            return axis;
        }

        /// <summary>
        /// Extracts the three coordinates axis from a quaternion
        /// </summary>
        /// <param name="q">Rotation</param>
        /// <param name="axis">Destination array</param>
        public static void GetAxis(this Quaternion q, Vector3[] axis)
        {
            float xx = q.X * q.X;
            float yy = q.Y * q.Y;
            float zz = q.Z * q.Z;
            float xy = q.X * q.Y;
            float xz = q.X * q.Z;
            float xw = q.X * q.W;
            float yz = q.Y * q.Z;
            float yw = q.Y * q.W;
            float zw = q.Z * q.W;

            axis[0].X = 1f - 2f * (yy + zz);
            axis[0].Y = 2 * (xy + zw);
            axis[0].Z = 2 * (xz - yw);

            axis[1].X = 2f * (xy - zw);
            axis[1].Y = 1f - 2f * (xx + zz);
            axis[1].Z = 2f * (yz + xw);

            axis[2].X = -2f * (xz + yw);
            axis[2].Y = -2f * (yz - xw);
            axis[2].Z = -1f + 2f * (xx + yy);
        }

        /// <summary>
        /// Computes an arc rotation from two vectors
        /// </summary>
        /// <param name="origin">Origin</param>
        /// <param name="destination">Destination</param>
        /// <param name="fallback"></param>
        /// <param name="rotation">Quaternion containing an arc rotation</param>
        public static void GetArcRotation(ref Vector3 origin, ref Vector3 destination, ref Vector3 fallback, out Quaternion rotation)
        {
            Vector3 start = origin;
            Vector3 end = destination;
            start.Normalize();
            end.Normalize();

            float dot;
            Vector3.Dot(ref start, ref end, out dot);
            if (dot >= 1.0f)
            {
                rotation = Quaternion.Identity;
                return;
            }

            if (dot < (1e-06f - 1.0f))
            {
                if (fallback != Vector3.Zero)
                    Quaternion.CreateFromAxisAngle(ref fallback, MathHelper.ToRadians(MathHelper.Pi), out rotation);
                else
                {
                    Vector3 axis = Vector3.Cross(Vector3.UnitZ, start);
                    if (axis.IsZeroLength()) axis = Vector3.Cross(Vector3.UnitY, start);
                    axis.Normalize();
                    Quaternion.CreateFromAxisAngle(ref axis, MathHelper.ToRadians(MathHelper.Pi), out rotation);
                }
            }
            else
            {
                float s = (float)Math.Sqrt((1.0f + dot) * 2.0f);
                float invS = 1.0f / s;
                Vector3 cross;
                Vector3.Cross(ref start, ref end, out cross);
#if XBOX || XBOX360
                rotation = new Quaternion();
#endif
                rotation.X = cross.X * invS;
                rotation.Y = cross.Y * invS;
                rotation.Z = cross.Z * invS;
                rotation.W = s * 0.5f;
                rotation.Normalize();
            }
        }

        /// <summary>
        /// Computes an arc rotation from two vectors
        /// </summary>
        /// <param name="origin">Origin</param>
        /// <param name="destination">Destination</param>
        /// <param name="fallback"></param>
        /// <returns>Return a quaternion containing an arc rotation</returns>
        public static Quaternion GetArcRotation(Vector3 origin, Vector3 destination, Vector3 fallback)
        {
            origin.Normalize();
            destination.Normalize();

            float dot;
            Vector3.Dot(ref origin, ref destination, out dot);
            if (dot >= 1.0f) return Quaternion.Identity;

            Quaternion q;
            if (dot < (1e-6f - 1.0f))
            {
                if (fallback != Vector3.Zero)
                    Quaternion.CreateFromAxisAngle(ref fallback, MathHelper.ToRadians(MathHelper.Pi), out q);
                else
                {
                    Vector3 axis = Vector3.Cross(Vector3.UnitZ, origin);
                    if (axis.IsZeroLength()) axis = Vector3.Cross(Vector3.UnitY, origin);
                    axis.Normalize();
                    Quaternion.CreateFromAxisAngle(ref axis, MathHelper.ToRadians(MathHelper.Pi), out q);
                }
            }
            else
            {
                float s = (float)Math.Sqrt((1.0f + dot) * 2.0f);
                float invS = 1.0f / s;
                Vector3 cross;
                Vector3.Cross(ref origin, ref destination, out cross);
#if XBOX || XBOX360
                q = new Quaternion();
#endif
                q.X = cross.X * invS;
                q.Y = cross.Y * invS;
                q.Z = cross.Z * invS;
                q.W = s * 0.5f;
                q.Normalize();
            }

            return q;
        }

        #endregion
    }
}
