using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contain frustum data to compute fast intersection test
    /// </summary>
    public struct SpeedFrustum
    {
        #region Fields

        private readonly Vector3 _nearNormal;
        private readonly Vector3 _farNormal;
        private readonly Vector3 _leftNormal;
        private readonly Vector3 _rightNormal;
        private readonly Vector3 _bottomNormal;
        private readonly Vector3 _topNormal;
        private readonly float _nearDist;
        private readonly float _farDist;
        private readonly float _leftDist;
        private readonly float _rightDist;
        private readonly float _bottomDist;
        private readonly float _topDist;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SpeedFrustum struct
        /// </summary>
        /// <param name="frustum">BoundingFrustum instance</param>
        public SpeedFrustum(ref BoundingFrustum frustum)
        {
            _nearNormal = frustum.Near.Normal;
            _farNormal = frustum.Far.Normal;
            _leftNormal = frustum.Left.Normal;
            _rightNormal = frustum.Right.Normal;
            _bottomNormal = frustum.Bottom.Normal;
            _topNormal = frustum.Top.Normal;

            _nearDist = frustum.Near.D;
            _farDist = frustum.Far.D;
            _leftDist = frustum.Left.D;
            _rightDist = frustum.Right.D;
            _bottomDist = frustum.Bottom.D;
            _topDist = frustum.Top.D;
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

            v.X = (_nearNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_nearNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_nearNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_nearDist + (_nearNormal.X * v.X) + (_nearNormal.Y * v.Y) + (_nearNormal.Z * v.Z) > 0) return false;

            v.X = (_leftNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_leftNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_leftNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_leftDist + (_leftNormal.X * v.X) + (_leftNormal.Y * v.Y) + (_leftNormal.Z * v.Z) > 0) return false;

            v.X = (_rightNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_rightNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_rightNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_rightDist + (_rightNormal.X * v.X) + (_rightNormal.Y * v.Y) + (_rightNormal.Z * v.Z) > 0) return false;

            v.X = (_bottomNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_bottomNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_bottomNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_bottomDist + (_bottomNormal.X * v.X) + (_bottomNormal.Y * v.Y) + (_bottomNormal.Z * v.Z) > 0) return false;

            v.X = (_topNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_topNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_topNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_topDist + (_topNormal.X * v.X) + (_topNormal.Y * v.Y) + (_topNormal.Z * v.Z) > 0) return false;

            v.X = (_farNormal.X >= 0 ? b.Min.X : b.Max.X);
            v.Y = (_farNormal.Y >= 0 ? b.Min.Y : b.Max.Y);
            v.Z = (_farNormal.Z >= 0 ? b.Min.Z : b.Max.Z);
            if (_farDist + (_farNormal.X * v.X) + (_farNormal.Y * v.Y) + (_farNormal.Z * v.Z) > 0) return false;

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

            if (_nearDist + (_nearNormal.X *v.X) + (_nearNormal.Y * v.Y) + (_nearNormal.Z * v.Z) > radius) return false;
            if (_leftDist + (_leftNormal.X * v.X) + (_leftNormal.Y * v.Y) + (_leftNormal.Z * v.Z) > radius) return false;
            if (_rightDist + (_rightNormal.X * v.X) + (_rightNormal.Y * v.Y) + (_rightNormal.Z * v.Z) > radius) return false;
            if (_bottomDist + (_bottomNormal.X * v.X) + (_bottomNormal.Y * v.Y) + (_bottomNormal.Z * v.Z) > radius) return false;
            if (_topDist + (_topNormal.X * v.X) + (_topNormal.Y * v.Y) + (_topNormal.Z * v.Z) > radius) return false;
            if (_farDist + (_farNormal.X * v.X) + (_farNormal.Y * v.Y) + (_farNormal.Z * v.Z) > radius) return false;

            return true;
        }

        #endregion
    }
}
