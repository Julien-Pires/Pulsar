using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Enum for bounding volume type
    /// </summary>
    public enum BoundingType
    {
        Aabb, 
        Sphere
    };

    /// <summary>
    /// Manages various type of bounding volume
    /// </summary>
    public sealed class BoundingVolume
    {
        #region Fields

        private BoundingBox _initialBox;
        private BoundingSphere _initialSphere;
        private BoundingSphere _realSphere;
        private BoundingBox _realBox;
        private readonly Vector3[] _tempCorners = new Vector3[8];

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BoundingVolume class
        /// </summary>
        public BoundingVolume()
        {
        }

        /// <summary>
        /// Constructor of BoundingVolume class
        /// </summary>
        /// <param name="initBox">Initial bounding box</param>
        /// <param name="initSphere">Initial bounding sphere</param>
        public BoundingVolume(BoundingBox initBox, BoundingSphere initSphere)
        {
            _initialBox = initBox;
            _initialSphere = initSphere;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates bounding volumes
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        public void Update(Matrix transform)
        {
            Update(ref transform);
        }

        /// <summary>
        /// Updates bounding volumes
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        public void Update(ref Matrix transform)
        {
            UpdateAabb(ref transform);
            UpdateSphere(ref transform);
        }

        /// <summary>
        /// Performs intersection between a frustum and the AABB
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <returns>Returns true if the bounding volume intersect with the frustum otherwise false</returns>
        public bool FrustumToAabbIntersect(SpeedFrustum frustum)
        {
            return frustum.Intersects(ref _realBox);
        }

        /// <summary>
        /// Performs intersection between a frustum and the bouding sphere
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <returns>Returns true if the bounding volume intersect with the frustum otherwise false</returns>
        public bool FrustumToSphereIntersect(SpeedFrustum frustum)
        {
            return frustum.Intersects(ref _realSphere);
        }

        /// <summary>
        /// Updates the AABB
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        private void UpdateAabb(ref Matrix transform)
        {
            _initialBox.GetCorners(_tempCorners);
            Vector3.Transform(_tempCorners, ref transform, _tempCorners);

            Vector3 min = _tempCorners[0];
            Vector3 max = _tempCorners[0];
            for (int i = 0; i < _tempCorners.Length; i++)
            {
                Vector3 vec = _tempCorners[i];
                if (vec.X > max.X) max.X = vec.X;
                if (vec.Y > max.Y) max.Y = vec.Y;
                if (vec.Z > max.Z) max.Z = vec.Z;
                if (vec.X < min.X) min.X = vec.X;
                if (vec.Y < min.Y) min.Y = vec.Y;
                if (vec.Z < min.Z) min.Z = vec.Z;
            }

            _realBox.Min = min;
            _realBox.Max = max;
        }

        /// <summary>
        /// Updates the bounding sphere
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        private void UpdateSphere(ref Matrix transform)
        {
            _initialSphere.Transform(ref transform, out _realSphere);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AABB
        /// </summary>
        public BoundingBox Box
        {
            get { return _realBox; }
        }

        /// <summary>
        /// Gets the bounding sphere
        /// </summary>
        public BoundingSphere Sphere
        {
            get { return _realSphere; }
        }

        /// <summary>
        /// Gets the initial AABB
        /// </summary>
        public BoundingBox InitialBox
        {
            get { return _initialBox; }
            set { _initialBox = value; }
        }

        /// <summary>
        /// Gets the initial bounding sphere
        /// </summary>
        public BoundingSphere InitialSphere
        {
            get { return _initialSphere; }
            set { _initialSphere = value; }
        }

        #endregion
    }
}