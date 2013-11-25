using System;

using Microsoft.Xna.Framework;

using Pulsar.Mathematic;

namespace Pulsar.Core
{
    /// <summary>
    /// Defines an Axis Aligned Bounding Box
    /// </summary>
    public sealed class AxisAlignedBox
    {
        #region Fields

        internal Vector3 InternCenter;
        internal Vector3 InternHalfSize;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AxisAlignedBox
        /// </summary>
        public AxisAlignedBox()
        {
        }

        /// <summary>
        /// Constructor of AxisAlignedBox
        /// </summary>
        /// <param name="aabb">XNA Bounding box</param>
        public AxisAlignedBox(BoundingBox aabb)
        {
            SetFromAabb(ref aabb);
        }

        /// <summary>
        /// Constructor of AxisAlignedBox
        /// </summary>
        /// <param name="aabb">Source AxisAlignedBox</param>
        public AxisAlignedBox(AxisAlignedBox aabb)
        {
            SetFromAabb(aabb);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the AABB
        /// </summary>
        public void Reset()
        {
            InternCenter = Vector3.Zero;
            InternHalfSize = Vector3.Zero;
        }

        /// <summary>
        /// Sets this AABB from another one
        /// </summary>
        /// <param name="aabb">Source aabb</param>
        public void SetFromAabb(BoundingBox aabb)
        {
            SetFromAabb(ref aabb);
        }

        /// <summary>
        /// Sets this AABB from another one
        /// </summary>
        /// <param name="aabb">Source aabb</param>
        public void SetFromAabb(ref BoundingBox aabb)
        {
            Vector3 halfSize;
            Vector3.Subtract(ref aabb.Max, ref aabb.Min, out halfSize);
            Vector3Extension.Abs(ref halfSize, out halfSize);
            Vector3.Multiply(ref halfSize, 0.5f, out InternHalfSize);
            Vector3.Add(ref aabb.Min, ref InternHalfSize, out InternCenter);
        }

        /// <summary>
        /// Sets this AABB from another one
        /// </summary>
        /// <param name="aabb">Source aabb</param>
        public void SetFromAabb(AxisAlignedBox aabb)
        {
            InternCenter = aabb.InternCenter;
            InternHalfSize = aabb.InternHalfSize;
        }

        /// <summary>
        /// Detects if this AABB intersects with another one
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Returns true if the two AABB intersects otherwise false</returns>
        public bool Intersects(AxisAlignedBox aabb)
        {
            Vector3 distance, sumHalfSize;
            Vector3.Subtract(ref InternCenter, ref aabb.InternCenter, out distance);
            Vector3.Add(ref InternHalfSize, ref aabb.InternHalfSize, out sumHalfSize);

            return (Math.Abs(distance.X) <= sumHalfSize.X) & (Math.Abs(distance.Y) <= sumHalfSize.Y)
                   & (Math.Abs(distance.Z) <= sumHalfSize.Z);
        }

        /// <summary>
        /// Detects if this AABB contains another one
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Returns true if the specified AABB is contained otherwise false</returns>
        public bool Contains(AxisAlignedBox aabb)
        {
            Vector3 distance;
            Vector3.Subtract(ref InternCenter, ref aabb.InternCenter, out distance);

            return (Math.Abs(distance.X) + aabb.InternHalfSize.X <= InternHalfSize.X)
                & (Math.Abs(distance.Y) + aabb.InternHalfSize.Y <= InternHalfSize.Y)
                & (Math.Abs(distance.Z) + aabb.InternHalfSize.Z <= InternHalfSize.Z);
        }

        /// <summary>
        /// Detects if a specified point is inside
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Returns true if the point is inside otherwise false</returns>
        public bool Contains(Vector3 point)
        {
            return Contains(ref point);
        }

        /// <summary>
        /// Detects if a specified point is inside
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Returns true if the point is inside otherwise false</returns>
        public bool Contains(ref Vector3 point)
        {
            Vector3 distance;
            Vector3.Subtract(ref InternCenter, ref point, out distance);

            return (Math.Abs(distance.X) <= InternHalfSize.X) & (Math.Abs(distance.Y) <= InternHalfSize.Y)
                   & (Math.Abs(distance.Z) <= InternHalfSize.Z);
        }

        /// <summary>
        /// Merge the AABB with a specified AABB
        /// </summary>
        /// <param name="aabb">Source AABB</param>
        public void Merge(AxisAlignedBox aabb)
        {
            Vector3 max, min, otherMax, otherMin;
            Vector3.Add(ref InternCenter, ref InternHalfSize, out max);
            Vector3.Add(ref aabb.InternCenter, ref aabb.InternHalfSize, out otherMax);
            Vector3.Subtract(ref InternCenter, ref InternHalfSize, out min);
            Vector3.Subtract(ref aabb.InternCenter, ref aabb.InternHalfSize, out otherMin);

            Vector3.Max(ref max, ref otherMax, out max);
            Vector3.Min(ref min, ref otherMin, out min);
            if ((!float.IsPositiveInfinity(max.X)) && (!float.IsPositiveInfinity(max.Y))
                && (!float.IsPositiveInfinity(max.Z)))
            {
                Vector3 maxPlusMin;
                Vector3.Add(ref max, ref min, out maxPlusMin);
                Vector3.Multiply(ref maxPlusMin, 0.5f, out InternCenter);
            }

            Vector3 maxMinusMin;
            Vector3.Subtract(ref max, ref min, out maxMinusMin);
            Vector3.Multiply(ref maxMinusMin, 0.5f, out InternCenter);
        }

        /// <summary>
        /// Extends the AABB to contains a specified point
        /// </summary>
        /// <param name="point">Source point</param>
        public void Merge(Vector3 point)
        {
            Merge(ref point);
        }

        /// <summary>
        /// Extends the AABB to contains a specified point
        /// </summary>
        /// <param name="point">Source point</param>
        public void Merge(ref Vector3 point)
        {
            Vector3 max, min;
            Vector3.Add(ref InternCenter, ref InternHalfSize, out max);
            Vector3.Subtract(ref InternCenter, ref InternHalfSize, out min);

            Vector3.Max(ref max, ref point, out max);
            Vector3.Min(ref min, ref point, out min);
            if ((!float.IsPositiveInfinity(max.X)) && (!float.IsPositiveInfinity(max.Y))
                && (!float.IsPositiveInfinity(max.Z)))
            {
                Vector3 maxPlusMin;
                Vector3.Add(ref max, ref min, out maxPlusMin);
                Vector3.Multiply(ref maxPlusMin, 0.5f, out InternCenter);
            }

            Vector3 maxMinusMin;
            Vector3.Subtract(ref max, ref min, out maxMinusMin);
            Vector3.Multiply(ref maxMinusMin, 0.5f, out InternCenter);
        }

        /// <summary>
        /// Transforms the AABB with a specified matrix
        /// </summary>
        /// <param name="matrix">Matrix</param>
        public void Transform(Matrix matrix)
        {
            Transform(ref matrix);
        }

        /// <summary>
        /// Transforms the AABB with a specified matrix
        /// </summary>
        /// <param name="matrix">Matrix</param>
        public void Transform(ref Matrix matrix)
        {
            Vector3.Transform(ref InternCenter, ref matrix, out InternCenter);

            float xHalfSize = (Math.Abs(matrix.M11)*InternHalfSize.X) + (Math.Abs(matrix.M21)*InternHalfSize.Y) 
                + (Math.Abs(matrix.M31)*InternHalfSize.Z);
            float yHalfSize = (Math.Abs(matrix.M12) * InternHalfSize.X) + (Math.Abs(matrix.M22) * InternHalfSize.Y) 
                + (Math.Abs(matrix.M32) * InternHalfSize.Z);
            float zHalfSize = (Math.Abs(matrix.M13)*InternHalfSize.X) + (Math.Abs(matrix.M23)*InternHalfSize.Y) 
                + (Math.Abs(matrix.M33)*InternHalfSize.Z);
            InternHalfSize = new Vector3(xHalfSize, yHalfSize, zHalfSize);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the center
        /// </summary>
        public Vector3 Center
        {
            get { return InternCenter; }
        }

        /// <summary>
        /// Gets the half-size
        /// </summary>
        public Vector3 HalfSize
        {
            get { return InternHalfSize; }
        }

        /// <summary>
        /// Gets the minimum point
        /// </summary>
        public Vector3 Minimum
        {
            get
            {
                Vector3 min;
                Vector3.Subtract(ref InternCenter, ref InternHalfSize, out min);

                return min;
            }
        }

        /// <summary>
        /// Gets the maximum point
        /// </summary>
        public Vector3 Maximum
        {
            get
            {
                Vector3 max;
                Vector3.Add(ref InternCenter, ref InternHalfSize, out max);

                return max;
            }
        }

        #endregion
    }
}
