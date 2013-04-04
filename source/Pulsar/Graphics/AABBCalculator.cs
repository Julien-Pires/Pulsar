using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    public class AABBCalculator
    {
        #region Fields

        private BoundingBox initialBox;
        private BoundingBox aabb;
        private Vector3[] tempCorners = new Vector3[8];

        #endregion

        #region Constructor

        public AABBCalculator()
        {
        }

        public AABBCalculator(BoundingBox initBox)
        {
            this.initialBox = initBox;
        }

        #endregion

        #region Methods

        public void UpdateBox(ref Matrix transformMatrix)
        {
            this.initialBox.GetCorners(this.tempCorners);
            Vector3.Transform(this.tempCorners, ref transformMatrix, this.tempCorners);

            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;
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

            this.aabb.Min = min;
            this.aabb.Max = max;
        }

        #endregion

        #region Properties

        public BoundingBox InitialBox
        {
            get { return this.initialBox; }
            set { this.initialBox = value; }
        }

        public BoundingBox AABB
        {
            get { return this.aabb; }
        }

        #endregion
    }
}
