using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contain frustum data to compute fast intersection test
    /// </summary>
    public struct SpeedFrustum
    {
        #region Fields

        private readonly Vector3 nearNormal;
        private readonly Vector3 farNormal;
        private readonly Vector3 leftNormal;
        private readonly Vector3 rightNormal;
        private readonly Vector3 bottomNormal;
        private readonly Vector3 topNormal;
        private readonly float nearDist;
        private readonly float farDist;
        private readonly float leftDist;
        private readonly float rightDist;
        private readonly float bottomDist;
        private readonly float topDist;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SpeedFrustum struct
        /// </summary>
        /// <param name="frustum">BoundingFrustum instance</param>
        public SpeedFrustum(ref BoundingFrustum frustum)
        {
            this.nearNormal = frustum.Near.Normal;
            this.farNormal = frustum.Far.Normal;
            this.leftNormal = frustum.Left.Normal;
            this.rightNormal = frustum.Right.Normal;
            this.bottomNormal = frustum.Bottom.Normal;
            this.topNormal = frustum.Top.Normal;

            this.nearDist = frustum.Near.D;
            this.farDist = frustum.Far.D;
            this.leftDist = frustum.Left.D;
            this.rightDist = frustum.Right.D;
            this.bottomDist = frustum.Bottom.D;
            this.topDist = frustum.Top.D;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compute intersection between the frustum and a bounding box
        /// </summary>
        /// <param name="b">Bounding box to test intersection with</param>
        /// <returns>Return true if they interesect, otherwise false</returns>
        public bool Intersects(ref BoundingBox b)
        {
            Vector3 v;

            v.X = (this.nearNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.nearNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.nearNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.nearDist + (this.nearNormal.X * v.X) + (this.nearNormal.Y * v.Y) + (this.nearNormal.Z * v.Z) > 0) return false;

            v.X = (this.leftNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.leftNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.leftNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.leftDist + (this.leftNormal.X * v.X) + (this.leftNormal.Y * v.Y) + (this.leftNormal.Z * v.Z) > 0) return false;

            v.X = (this.rightNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.rightNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.rightNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.rightDist + (this.rightNormal.X * v.X) + (this.rightNormal.Y * v.Y) + (this.rightNormal.Z * v.Z) > 0) return false;

            v.X = (this.bottomNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.bottomNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.bottomNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.bottomDist + (this.bottomNormal.X * v.X) + (this.bottomNormal.Y * v.Y) + (this.bottomNormal.Z * v.Z) > 0) return false;

            v.X = (this.topNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.topNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.topNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.topDist + (this.topNormal.X * v.X) + (this.topNormal.Y * v.Y) + (this.topNormal.Z * v.Z) > 0) return false;

            v.X = (this.farNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (this.farNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (this.farNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (this.farDist + (this.farNormal.X * v.X) + (this.farNormal.Y * v.Y) + (this.farNormal.Z * v.Z) > 0) return false;

            return true;
        }

        /// <summary>
        /// Compute intersection between the frustum and a bounding sphere
        /// </summary>
        /// <param name="s">Bounding sphere to test intersection with</param>
        /// <returns>Return true if they intersect, otherwise false</returns>
        public bool Intersects(ref BoundingSphere s)
        {
            Vector3 v = s.Center;
            float radius = s.Radius;

            if (this.nearDist + (this.nearNormal.X *v.X) + (this.nearNormal.Y * v.Y) + (this.nearNormal.Z * v.Z) > radius) return false;
            if (this.leftDist + (this.leftNormal.X * v.X) + (this.leftNormal.Y * v.Y) + (this.leftNormal.Z * v.Z) > radius) return false;
            if (this.rightDist + (this.rightNormal.X * v.X) + (this.rightNormal.Y * v.Y) + (this.rightNormal.Z * v.Z) > radius) return false;
            if (this.bottomDist + (this.bottomNormal.X * v.X) + (this.bottomNormal.Y * v.Y) + (this.bottomNormal.Z * v.Z) > radius) return false;
            if (this.topDist + (this.topNormal.X * v.X) + (this.topNormal.Y * v.Y) + (this.topNormal.Z * v.Z) > radius) return false;
            if (this.farDist + (this.farNormal.X * v.X) + (this.farNormal.Y * v.Y) + (this.farNormal.Z * v.Z) > radius) return false;

            return true;
        }

        #endregion
    }
}
