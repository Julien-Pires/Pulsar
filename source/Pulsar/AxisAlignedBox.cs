using System;

using Microsoft.Xna.Framework;

using Pulsar.Mathematic;

namespace Pulsar
{
    /// <summary>
    /// Defines an Axis Aligned Bounding Box describes by a center and a half size
    /// </summary>
    public sealed class AxisAlignedBox
    {
        #region Fields

        /// <summary>
        /// Center of the AABB
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// Extend of the AABB
        /// </summary>
        public Vector3 HalfSize;

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
            Center = Vector3.Zero;
            HalfSize = Vector3.Zero;
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
            Vector3.Multiply(ref halfSize, 0.5f, out HalfSize);
            Vector3.Add(ref aabb.Min, ref HalfSize, out Center);
        }

        /// <summary>
        /// Sets this AABB from another one
        /// </summary>
        /// <param name="aabb">Source aabb</param>
        public void SetFromAabb(AxisAlignedBox aabb)
        {
            Center = aabb.Center;
            HalfSize = aabb.HalfSize;
        }

        /// <summary>
        /// Checks intersection with a plane
        /// </summary>
        /// <param name="plane">Plane to test against</param>
        /// <returns>Return the relationship between the aabb and the plane</returns>
        public PlaneIntersectionType Intersects(Plane plane)
        {
            Vector3 absPlane;
            Vector3Extension.Abs(ref plane.Normal, out absPlane);

            float e, s;
            Vector3.Dot(ref HalfSize, ref absPlane, out e);
            Vector3.Dot(ref Center, ref plane.Normal, out s);
            s += plane.D;

            if (s - e > 0) 
                return PlaneIntersectionType.Back;

            return (s + e < 0) ? PlaneIntersectionType.Front : PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks intersection with a plane
        /// </summary>
        /// <param name="plane">Plane to test against</param>
        /// <param name="result">Result relationship between the aabb and the plane</param>
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            Vector3 absPlane;
            Vector3Extension.Abs(ref plane.Normal, out absPlane);

            float e, s;
            Vector3.Dot(ref HalfSize, ref absPlane, out e);
            Vector3.Dot(ref Center, ref plane.Normal, out s);
            s += plane.D;

            if (s - e > 0)
            {
                result = PlaneIntersectionType.Back;
                return;
            }
            if (s + e < 0)
            {
                result = PlaneIntersectionType.Front;
                return;
            }

            result = PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Detects if this AABB intersects with another one
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Returns true if the two AABB intersects otherwise false</returns>
        public bool Intersects(AxisAlignedBox aabb)
        {
            Vector3 distance, sumHalfSize;
            Vector3.Subtract(ref Center, ref aabb.Center, out distance);
            Vector3.Add(ref HalfSize, ref aabb.HalfSize, out sumHalfSize);

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
            Vector3.Subtract(ref Center, ref aabb.Center, out distance);

            return (Math.Abs(distance.X) + aabb.HalfSize.X <= HalfSize.X)
                & (Math.Abs(distance.Y) + aabb.HalfSize.Y <= HalfSize.Y)
                & (Math.Abs(distance.Z) + aabb.HalfSize.Z <= HalfSize.Z);
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
            Vector3.Subtract(ref Center, ref point, out distance);

            return (Math.Abs(distance.X) <= HalfSize.X) & (Math.Abs(distance.Y) <= HalfSize.Y)
                   & (Math.Abs(distance.Z) <= HalfSize.Z);
        }

        /// <summary>
        /// Merge the AABB with a specified AABB
        /// </summary>
        /// <param name="aabb">Source AABB</param>
        public void Merge(AxisAlignedBox aabb)
        {
            Vector3 max, min, otherMax, otherMin;
            Vector3.Add(ref Center, ref HalfSize, out max);
            Vector3.Add(ref aabb.Center, ref aabb.HalfSize, out otherMax);
            Vector3.Subtract(ref Center, ref HalfSize, out min);
            Vector3.Subtract(ref aabb.Center, ref aabb.HalfSize, out otherMin);

            Vector3.Max(ref max, ref otherMax, out max);
            Vector3.Min(ref min, ref otherMin, out min);
            if ((!float.IsPositiveInfinity(max.X)) && (!float.IsPositiveInfinity(max.Y))
                && (!float.IsPositiveInfinity(max.Z)))
            {
                Vector3 maxPlusMin;
                Vector3.Add(ref max, ref min, out maxPlusMin);
                Vector3.Multiply(ref maxPlusMin, 0.5f, out Center);
            }

            Vector3 maxMinusMin;
            Vector3.Subtract(ref max, ref min, out maxMinusMin);
            Vector3.Multiply(ref maxMinusMin, 0.5f, out Center);
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
            Vector3.Add(ref Center, ref HalfSize, out max);
            Vector3.Subtract(ref Center, ref HalfSize, out min);

            Vector3.Max(ref max, ref point, out max);
            Vector3.Min(ref min, ref point, out min);
            if ((!float.IsPositiveInfinity(max.X)) && (!float.IsPositiveInfinity(max.Y))
                && (!float.IsPositiveInfinity(max.Z)))
            {
                Vector3 maxPlusMin;
                Vector3.Add(ref max, ref min, out maxPlusMin);
                Vector3.Multiply(ref maxPlusMin, 0.5f, out Center);
            }

            Vector3 maxMinusMin;
            Vector3.Subtract(ref max, ref min, out maxMinusMin);
            Vector3.Multiply(ref maxMinusMin, 0.5f, out Center);
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
            Vector3.Transform(ref Center, ref matrix, out Center);

            float xHalfSize = (Math.Abs(matrix.M11)*HalfSize.X) + (Math.Abs(matrix.M21)*HalfSize.Y) 
                + (Math.Abs(matrix.M31)*HalfSize.Z);
            float yHalfSize = (Math.Abs(matrix.M12) * HalfSize.X) + (Math.Abs(matrix.M22) * HalfSize.Y) 
                + (Math.Abs(matrix.M32) * HalfSize.Z);
            float zHalfSize = (Math.Abs(matrix.M13)*HalfSize.X) + (Math.Abs(matrix.M23)*HalfSize.Y) 
                + (Math.Abs(matrix.M33)*HalfSize.Z);
            HalfSize = new Vector3(xHalfSize, yHalfSize, zHalfSize);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the minimum point
        /// </summary>
        public Vector3 Minimum
        {
            get
            {
                Vector3 min;
                Vector3.Subtract(ref Center, ref HalfSize, out min);

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
                Vector3.Add(ref Center, ref HalfSize, out max);

                return max;
            }
        }

        #endregion
    }
}
