using Microsoft.Xna.Framework;

using Pulsar.Mathematic;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a frustum
    /// </summary>
    public sealed class Frustum
    {
        #region Fields

        private readonly Plane[] _planes = new Plane[6];
        private readonly Vector3[] _absPlanes = new Vector3[6];
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
                Vector3Extension.Abs(ref _planes[i].Normal, out _absPlanes[i]);
        }

        /// <summary>
        /// Checks if an AABB intersects with the frustum
        /// </summary>
        /// <param name="aabb">AABB</param>
        /// <returns>Returns true if the AABB intersects otherwise false</returns>
        public bool Intersect(AxisAlignedBox aabb)
        {
            bool result = true;
            float s, e;

            Vector3.Dot(ref aabb.Center, ref _planes[0].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[0], out e);
            result &= (s - e) < -_planes[0].D;

            Vector3.Dot(ref aabb.Center, ref _planes[1].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[1], out e);
            result &= (s - e) < -_planes[1].D;

            Vector3.Dot(ref aabb.Center, ref _planes[2].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[2], out e);
            result &= (s - e) < -_planes[2].D;

            Vector3.Dot(ref aabb.Center, ref _planes[3].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[3], out e);
            result &= (s - e) < -_planes[3].D;

            Vector3.Dot(ref aabb.Center, ref _planes[4].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[4], out e);
            result &= (s - e) < -_planes[4].D;

            Vector3.Dot(ref aabb.Center, ref _planes[5].Normal, out s);
            Vector3.Dot(ref aabb.HalfSize, ref _absPlanes[5], out e);
            result &= (s - e) < -_planes[5].D;

            return result;
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

        #region Properties

        internal BoundingFrustum BoundingFrustum
        {
            get { return _boundingFrustum; }
        }

        #endregion
    }
}
