using Microsoft.Xna.Framework;

using Pulsar.Core;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a frustum
    /// </summary>
    public sealed class Frustum
    {
        #region Fields

        private readonly Plane[] _planes = new Plane[6];
        private readonly Vector3[] _negativeNormals = new Vector3[6];
        //private readonly Vector3[] _signPlane = new Vector3[6]; // Used for Intersects2
        private readonly BoundingFrustum _boundingFrustum = new BoundingFrustum(Matrix.Identity);

        #endregion

        #region Methods

        /// <summary>
        /// Extracts plane from BoundingFrustum instance
        /// </summary>
        private void ExtractsPlanes()
        {
            _planes[0] = _boundingFrustum.Near;
            _planes[1] = _boundingFrustum.Far;
            _planes[2] = _boundingFrustum.Top;
            _planes[3] = _boundingFrustum.Bottom;
            _planes[4] = _boundingFrustum.Left;
            _planes[5] = _boundingFrustum.Right;
        }

        /// <summary>
        /// Updates the frustum
        /// </summary>
        /// <param name="viewProj">View-Projection matrix</param>
        internal void Update(ref Matrix viewProj)
        {
            _boundingFrustum.Matrix = viewProj;
            ExtractsPlanes();

            for (int i = 0; i < _planes.Length; i++)
            {
                Vector3.Negate(ref _planes[i].Normal, out _negativeNormals[i]);
                //Vector3Extension.Sign(ref _planes[i].Normal, out _signPlane[i]);
            }
        }

        /// Doesn't work, need to be fixed
        /*public bool Intersect2(AxisAlignedBox aabb)
        {
            Vector3 v;
            float d;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[0], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[0].Normal, out d);
            if (d > _planes[0].D) return true;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[1], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[1].Normal, out d);
            if (d > _planes[1].D) return true;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[2], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[2].Normal, out d);
            if (d > _planes[2].D) return true;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[3], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[3].Normal, out d);
            if (d > _planes[3].D) return true;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[4], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[4].Normal, out d);
            if (d > _planes[4].D) return true;

            Vector3.Multiply(ref aabb.InternHalfSize, ref _signPlane[5], out v);
            Vector3.Add(ref aabb.InternCenter, ref v, out v);
            Vector3.Dot(ref v, ref _planes[5].Normal, out d);
            if (d > _planes[5].D) return true;

            return false;
        }*/

        /// <summary>
        /// Detects if an AABB intersects with the frustum
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Returns true if the AABB intersects otherwise false</returns>
        public bool Intersect(AxisAlignedBox aabb)
        {
            Vector3 max = aabb.Maximum;
            Vector3 min = aabb.Minimum;
            Vector3 positive;
            float d;
            bool isOutside = false;

            GetPositiveVertex(ref max, ref min, ref _planes[0].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[0], ref positive, out d);
            isOutside |= (-_planes[0].D + d < 0);

            GetPositiveVertex(ref max, ref min, ref _planes[1].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[1], ref positive, out d);
            isOutside |= (-_planes[1].D + d < 0);

            GetPositiveVertex(ref max, ref min, ref _planes[2].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[2], ref positive, out d);
            isOutside |= (-_planes[2].D + d < 0);

            GetPositiveVertex(ref max, ref min, ref _planes[3].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[3], ref positive, out d);
            isOutside |= (-_planes[3].D + d < 0);

            GetPositiveVertex(ref max, ref min, ref _planes[4].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[4], ref positive, out d);
            isOutside |= (-_planes[4].D + d < 0);

            GetPositiveVertex(ref max, ref min, ref _planes[5].Normal, out positive);
            Vector3.Dot(ref _negativeNormals[5], ref positive, out d);
            isOutside |= (-_planes[5].D + d < 0);

            return !isOutside;
        }

        /// <summary>
        /// Gets the positive vertex
        /// </summary>
        /// <param name="aabbMax">Maximum point of AABB</param>
        /// <param name="aabbMin">Minimum point of AABB</param>
        /// <param name="plane">Plane</param>
        /// <param name="result">Destination vector</param>
        private void GetPositiveVertex(ref Vector3 aabbMax, ref Vector3 aabbMin, ref Vector3 plane, out Vector3 result)
        {
            result = aabbMax;
            if (plane.X >= 0)
                result.X = aabbMin.X;
            if (plane.Y >= 0)
                result.Y = aabbMin.Y;
            if (plane.Z >= 0)
                result.Z = aabbMin.Z;
        }

        /// <summary>
        /// Gets the six planes of the frustum
        /// </summary>
        /// <param name="planes">Destination array</param>
        public void GetPlanes(Plane[] planes)
        {
            for (int i = 0; i < _planes.Length; i++)
                planes[i] = _planes[i];
        }

        #endregion
    }
}
