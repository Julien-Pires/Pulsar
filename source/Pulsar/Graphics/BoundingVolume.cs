using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Enum for bounding volume type
    /// </summary>
    public enum BoundingType { AABB, Sphere };

    /// <summary>
    /// Manage various type of bounding volume
    /// </summary>
    public sealed class BoundingVolume
    {
        #region Fields

        private BoundingType boundType = BoundingType.AABB;
        private BoundingBox initialBox;
        private BoundingSphere initialSphere;
        private BoundingSphere realSphere;
        private BoundingBox realBox;
        private Vector3[] tempCorners = new Vector3[8];

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
            this.initialBox = initBox;
            this.initialSphere = initSphere;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update bounding volumes
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        public void Update(ref Matrix transform)
        {
            switch (this.boundType)
            {
                case BoundingType.AABB: this.UpdateAABB(ref transform);
                    break;
                case BoundingType.Sphere: this.UpdateSphere(ref transform);
                    break;
            }
        }

        /// <summary>
        /// Performs intersection with a frustum
        /// </summary>
        /// <param name="frustum">Frustum</param>
        /// <returns>Returns true if the bounding volume intersect with the frustum otherwise false</returns>
        public bool FrustumIntersect(ref SpeedFrustum frustum)
        {
            bool result = false;
            switch (this.boundType)
            {
                case BoundingType.AABB: result = frustum.Intersects(ref this.realBox);
                    break;
                case BoundingType.Sphere: result = frustum.Intersects(ref this.realSphere);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Update the AABB
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        private void UpdateAABB(ref Matrix transform)
        {
            this.initialBox.GetCorners(this.tempCorners);
            Vector3.Transform(this.tempCorners, ref transform, this.tempCorners);

            Vector3 min = this.tempCorners[0];
            Vector3 max = this.tempCorners[0];
            for (int i = 0; i < this.tempCorners.Length; i++)
            {
                Vector3 vec = this.tempCorners[i];
                if (vec.X > max.X) max.X = vec.X;
                if (vec.Y > max.Y) max.Y = vec.Y;
                if (vec.Z > max.Z) max.Z = vec.Z;
                if (vec.X < min.X) min.X = vec.X;
                if (vec.Y < min.Y) min.Y = vec.Y;
                if (vec.Z < min.Z) min.Z = vec.Z;
            }

            this.realBox.Min = min;
            this.realBox.Max = max;
        }

        /// <summary>
        /// Update the bounding sphere
        /// </summary>
        /// <param name="transform">Matrix transform</param>
        private void UpdateSphere(ref Matrix transform)
        {
            this.initialSphere.Transform(ref transform, out this.realSphere);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the type of bounding volume used
        /// </summary>
        public BoundingType Type
        {
            get { return this.boundType; }
            set { this.boundType = value; }
        }

        /// <summary>
        /// Get the AABB
        /// </summary>
        public BoundingBox Box
        {
            get { return this.realBox; }
        }

        /// <summary>
        /// Get the bounding sphere
        /// </summary>
        public BoundingSphere Sphere
        {
            get { return this.realSphere; }
        }

        /// <summary>
        /// Get the initial AABB
        /// </summary>
        public BoundingBox InitialBox
        {
            get { return this.initialBox; }
            set { this.initialBox = value; }
        }

        /// <summary>
        /// Get the initial bounding sphere
        /// </summary>
        public BoundingSphere InitialSphere
        {
            get { return this.initialSphere; }
            set { this.initialSphere = value; }
        }

        #endregion
    }
}
