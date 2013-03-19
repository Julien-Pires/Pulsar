using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Helpers
{
    /// <summary>
    /// Extension class for the Vector3 class
    /// </summary>
    public static class Vector3Extension
    {
        #region Methods

        /// <summary>
        /// Check if a vector has a length of zero (or extremly near of zero)
        /// </summary>
        /// <param name="v">The vector from which the length is checked</param>
        /// <returns>Return true if the vector has a length of zero, otherwise false</returns>
        public static bool IsZeroLength(this Vector3 v)
        {
            float l = (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z);
            return l < (1e-06 * 1e-06);
        }

        /// <summary>
        /// Compute an arc rotation from two vectors
        /// </summary>
        /// <param name="v">Origin vector</param>
        /// <param name="dest">Destination vector</param>
        /// <param name="fallback"></param>
        /// <param name="rotation">Quaternion containing an arc rotation</param>
        public static void GetArcRotation(this Vector3 v, ref Vector3 dest, ref Vector3 fallback, out Quaternion rotation)
        {
            Vector3 origin = v;
            Vector3 destination = dest;
            origin.Normalize();
            destination.Normalize();

            float dot;
            Vector3.Dot(ref origin, ref destination, out dot);
            if (dot >= 1.0f)
            {
                rotation = Quaternion.Identity;
            }

            if (dot < (1e-06f - 1.0f))
            {
                if (fallback != Vector3.Zero)
                {
                    Quaternion.CreateFromAxisAngle(ref fallback, MathHelper.ToRadians(MathHelper.Pi), out rotation);
                }
                else
                {
                    Vector3 axis = Vector3.Cross(Vector3.UnitZ, v);

                    if (axis.IsZeroLength())
                    {
                        axis = Vector3.Cross(Vector3.UnitY, v);
                    }
                    axis.Normalize();
                    Quaternion.CreateFromAxisAngle(ref axis, MathHelper.ToRadians(MathHelper.Pi), out rotation);
                }
            }
            else
            {
                float s = (float)Math.Sqrt((1.0f + dot) * 2.0f);
                float invS = 1.0f / s;
                Vector3 cross = Vector3.Cross(origin, destination);

                rotation = new Quaternion();
                rotation.X = cross.X * invS;
                rotation.Y = cross.Y * invS;
                rotation.Z = cross.Z * invS;
                rotation.W = s * 0.5f;
                rotation.Normalize();
            }
        }

        /// <summary>
        /// Compute an arc rotation from two vectors
        /// </summary>
        /// <param name="v">Origin vector</param>
        /// <param name="dest">Destination vector</param>
        /// <param name="fallback"></param>
        /// <returns>Return a quaternion containing an arc rotation</returns>
        public static Quaternion GetArcRotation(this Vector3 v, Vector3 dest, Vector3 fallback)
        {
            Quaternion q;
            Vector3 origin = v;
            origin.Normalize();
            dest.Normalize();

            float dot;
            Vector3.Dot(ref origin, ref dest, out dot);
            if (dot >= 1.0f)
            {
                return Quaternion.Identity;
            }

            if (dot < (1e-6f - 1.0f))
            {
                if (fallback != Vector3.Zero)
                {
                    Quaternion.CreateFromAxisAngle(ref fallback, MathHelper.ToRadians(MathHelper.Pi), out q);
                }
                else
                {
                    Vector3 axis = Vector3.Cross(Vector3.UnitZ, v);

                    if (axis.IsZeroLength())
                    {
                        axis = Vector3.Cross(Vector3.UnitY, v);
                    }
                    axis.Normalize();
                    Quaternion.CreateFromAxisAngle(ref axis, MathHelper.ToRadians(MathHelper.Pi), out q);
                }
            }
            else
            {
                float s = (float)Math.Sqrt((1.0f + dot) * 2.0f);
                float invS = 1.0f / s;
                Vector3 cross = Vector3.Cross(origin, dest);

                q = new Quaternion();
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
