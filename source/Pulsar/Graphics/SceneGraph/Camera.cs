using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Mathematic;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Determine the type of projection to be used by a camera
    /// </summary>
    public enum ProjectionType { Perspective, Orthographic }

    /// <summary>
    /// Base class for a camera
    /// </summary>
    public class Camera : IMovable
    {
        #region Fields

        protected string name = string.Empty;
        protected SceneNode parent;
        protected bool isDirtyView = true;
        protected bool isDirtyFrustum = true;
        protected bool isDirtyBoundingFrustum = true;
        protected bool isViewportChanged = true;
        protected bool hasParentChanged = false;
        protected bool useFixedYaw = true;
        protected ProjectionType projType = ProjectionType.Perspective;
        protected Matrix transform = Matrix.Identity;
        protected Matrix viewMatrix = Matrix.Identity;
        protected Matrix projectionMatrix = Matrix.Identity;
        protected float near = 1.0f;
        protected float far = 1000.0f;
        protected float aspectRatio = 16.0f/9.0f;
        protected float fieldOfView = MathHelper.PiOver4;
        protected Vector3 yawFixedAxis = Vector3.UnitY;
        protected Vector3 direction = Vector3.Forward;
        protected Vector3 defaultDirection = Vector3.Forward;
        protected Quaternion orientation = Quaternion.Identity;
        protected Vector3 position = Vector3.Zero;
        protected Quaternion lastNodeOrientation = Quaternion.Identity;
        protected Vector3 lastNodePosition = Vector3.Zero;
        protected Quaternion fullOrientation = Quaternion.Identity;
        protected Vector3 fullPosition = Vector3.Zero;
        protected Viewport vp;
        protected BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
        protected SpeedFrustum spdFrustum;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the BaseCamera class
        /// </summary>
        /// <param name="name">Name of the camera</param>
        public Camera(string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <remarks>Not implemented for the BaseCamera class</remarks>
        /// </summary>
        /// <param name="cam">Current camera</param>
        public virtual void NotifyCurrentCamera(Camera cam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <remarks>Not implemented for the BaseCamera class </remarks>
        /// </summary>
        /// <param name="queue">Current render queue</param>
        public virtual void UpdateRenderQueue(RenderQueue queue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use a fixed vector for yaw
        /// </summary>
        /// <param name="useFixed">Boolean indicating the uses or not of a fixed vector</param>
        /// <param name="fixedAxis">Fixed vector for yaw</param>
        public virtual void UseFixedYaw(bool useFixed, Vector3? fixedAxis)
        {
            fixedAxis = fixedAxis ?? Vector3.UnitY;
            this.useFixedYaw = useFixed;
            this.yawFixedAxis = fixedAxis.Value;
        }

        /// <summary>
        /// Move the camera's position by a specific vector in the world coordinate
        /// </summary>
        /// <param name="v">Vector to add for translate</param>
        public virtual void Translate(Vector3 v)
        {
            Vector3.Add(ref this.position, ref v, out this.position);

            this.InvalidateView();
        }

        /// <summary>
        /// Move the camera's position by a specific vector in the local coordinate
        /// </summary>
        /// <param name="v">Vector to add for translate</param>
        public virtual void TranslateRelative(Vector3 v)
        {
            Vector3 mov = Vector3.Transform(v, this.orientation);

            Vector3.Add(ref this.position, ref mov, out this.position);
        }

        /// <summary>
        /// Execute a yaw operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public virtual void Yaw(float angle)
        {
            Vector3 axis;

            if (this.useFixedYaw)
            {
                axis = this.yawFixedAxis;
            }
            else
            {
                axis = Vector3.Transform(Vector3.UnitY, this.orientation);
            }

            this.Rotate(axis, angle);

            this.InvalidateView();
        }

        /// <summary>
        /// Execute a pitch operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public virtual void Pitch(float angle)
        {
            Vector3 axis = Vector3.Transform(Vector3.UnitX, this.orientation);
            
            this.Rotate(axis, angle);

            this.InvalidateView();
        }

        /// <summary>
        /// Execute a roll operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public virtual void Roll(float angle)
        {
            Vector3 axis = Vector3.Transform(Vector3.UnitZ, this.orientation);

            this.Rotate(axis, angle);

            this.InvalidateView();
        }

        /// <summary>
        /// Rotate around an axis
        /// </summary>
        /// <param name="axis">Axis to rotate around</param>
        /// <param name="angle">Angle to rotate</param>
        public virtual void Rotate(Vector3 axis, float angle)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
            
            this.Rotate(q);
        }

        /// <summary>
        /// Rotate around an axis with a quaternion
        /// </summary>
        /// <param name="q">Quaternion used for rotation</param>
        public virtual void Rotate(Quaternion q)
        {
            q.Normalize();
            Quaternion.Multiply(ref q, ref this.orientation, out this.orientation);

            this.InvalidateView();
        }

        /// <summary>
        /// Set a direction on wich the camera is looking
        /// </summary>
        /// <param name="v">Direction to look for the camera</param>
        public virtual void LookAt(Vector3 target)
        {
            this.UpdateView();
            this.SetDirection(target - this.fullPosition);
        }

        /// <summary>
        /// Set the direction where the camera is watching
        /// </summary>
        /// <param name="v">Direction vector</param>
        public virtual void SetDirection(Vector3 v)
        {
            Vector3 adjustZ = -v;
            adjustZ.Normalize();

            Quaternion targetOrientation;

            if (this.useFixedYaw)
            {
                Vector3 vecX = Vector3.Cross(this.yawFixedAxis, adjustZ);
                vecX.Normalize();

                Vector3 vecY = Vector3.Cross(adjustZ, vecX);
                vecY.Normalize();

                Matrix rotation;
                MatrixExtension.CreateFromAxes(ref vecX, ref vecY, ref adjustZ, out rotation);
                Quaternion.CreateFromRotationMatrix(ref rotation, out targetOrientation); 
            }
            else
            {
                Vector3[] axes;
                this.UpdateView();
                this.fullOrientation.GetAxes(out axes);
                Quaternion rotationQuat;

                if ((axes[2] + adjustZ).LengthSquared() < 0.00005f)
                {
                    Quaternion.CreateFromAxisAngle(ref axes[1], MathHelper.ToRadians(MathHelper.Pi), out rotationQuat);
                }
                else
                {
                    Vector3 fallB = Vector3.Zero;
                    axes[2].GetArcRotation(ref adjustZ, ref fallB, out rotationQuat);
                }

                Quaternion.Multiply(ref rotationQuat, ref fullOrientation, out targetOrientation);
            }

            if (this.parent != null)
            {
                this.orientation = Quaternion.Inverse(this.parent.Orientation) * targetOrientation;
            }
            else
            {
                this.orientation = targetOrientation;
            }

            this.InvalidateView();
        }

        /// <summary>
        /// Check if the frustum is out of date and update values accordingly
        /// </summary>
        /// <returns>Return true if the frustum is out of date, else false</returns>
        protected virtual bool IsProjectionOutOfDate()
        {
            if (this.IsViewOutOfDate())
            {
                this.isDirtyFrustum = true;
            }

            return this.isDirtyFrustum;
        }

        /// <summary>
        /// Check if view is out of date and update values accordingly
        /// </summary>
        protected virtual bool IsViewOutOfDate()
        {
            if (this.parent != null)
            {
                if((isDirtyView) ||
                    ((this.lastNodeOrientation != this.parent.FullOrientation) ||
                    (this.lastNodePosition != this.parent.FullPosition)))
                {
                    this.lastNodeOrientation = this.parent.FullOrientation;
                    this.lastNodePosition = this.parent.FullPosition;
                    Quaternion.Multiply(ref this.lastNodeOrientation, ref this.orientation, out this.fullOrientation);
                    this.fullPosition = Vector3.Add(Vector3.Transform(this.position, this.lastNodeOrientation), this.lastNodePosition);

                    this.InvalidateView();
                }
            }
            else
            {
                this.fullOrientation = this.orientation;
                this.fullPosition = this.position;
            }

            return this.isDirtyView;
        }

        /// <summary>
        /// Invalidate the projection
        /// </summary>
        protected virtual void InvalidateFrustum()
        {
            this.isDirtyFrustum = true;
            this.isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Invalidate the view
        /// </summary>
        protected virtual void InvalidateView()
        {
            this.isDirtyView = true;
            this.isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Check if view matrix need to be updated
        /// </summary>
        protected virtual void UpdateView()
        {
            if (this.IsViewOutOfDate())
            {
                this.ComputeView();
            }
        }

        /// <summary>
        /// Check if projection matrix need to be updated
        /// </summary>
        protected virtual void UpdateFrustum()
        {
            if (this.IsProjectionOutOfDate())
            {
                this.ComputeProjection();
            }
        }

        /// <summary>
        /// Check if the bounding frustum need to be updated
        /// </summary>
        protected virtual void UpdateBoundingFrustum()
        {
            this.UpdateView();
            this.UpdateFrustum();

            if (this.isDirtyBoundingFrustum)
            {
                this.ComputeBoundingFrustum();
            }
        }

        /// <summary>
        /// Compute the bounding frustum
        /// </summary>
        protected virtual void ComputeBoundingFrustum()
        {
            this.frustum.Matrix = this.viewMatrix * this.projectionMatrix;
            this.spdFrustum = new SpeedFrustum(ref this.frustum);

            this.isDirtyBoundingFrustum = false;
        }

        /// <summary>
        /// Compute the projection matrix depending on the projection type
        /// </summary>
        protected virtual void ComputeProjection()
        {
            switch (this.projType)
            {
                case ProjectionType.Perspective:
                    Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, this.near, 
                        this.far, out this.projectionMatrix);
                    break;
                case ProjectionType.Orthographic:
                    Matrix.CreateOrthographic(vp.Width, vp.Height, this.near, this.far, out this.projectionMatrix);
                    break;
            }

            this.isDirtyFrustum = false;
        }

        /// <summary>
        /// Compute the view matrix
        /// </summary>
        protected virtual void ComputeView()
        {
            if (this.isDirtyView)
            {
                this.transform = Matrix.CreateFromQuaternion(this.fullOrientation) * Matrix.CreateTranslation(this.fullPosition);
                Matrix.Invert(ref this.transform, out this.viewMatrix);
            }

            this.isDirtyView = false;
            this.isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Attach this object to a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        /// <param name="parent">Parent scene node</param>
        public void AttachParent(SceneNode parent)
        {
            this.parent = parent;

            this.InvalidateView();
        }

        /// <summary>
        /// Detach this object of a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        public void DetachParent()
        {
            this.parent = null;

            this.InvalidateView();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set a boolean indicating if the camera is visible (frustum plane)
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Get or set the viewport in wich this camera will render
        /// </summary>
        public Viewport Viewport
        {
            get { return this.vp; }
            set
            {
                this.vp = value;
                this.aspectRatio = this.vp.AspectRatio;

                this.InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the projection type of this camera
        /// </summary>
        public virtual ProjectionType Projection
        {
            get { return this.projType; }
            set
            {
                this.projType = value;

                this.InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the field of view of this camera
        /// </summary>
        public virtual float FOV
        {
            get { return this.fieldOfView; }
            set
            {
                this.fieldOfView = value;

                this.InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the near plane of this camera
        /// </summary>
        public virtual float NearPlane
        {
            get { return this.near; }
            set
            {
                this.near = value;

                this.InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the far plane of this camera
        /// </summary>
        public virtual float FarPlane
        {
            get { return this.far; }
            set
            {
                this.far = value;

                this.InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the position of the camera
        /// </summary>
        public virtual Vector3 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;

                this.InvalidateView();
            }
        }

        /// <summary>
        /// Get the bounding frustum of the camera
        /// </summary>
        public virtual BoundingFrustum Frustum
        {
            get
            {
                this.UpdateBoundingFrustum();

                return this.frustum;
            }
        }

        /// <summary>
        /// Get a SpeedFrustum instance to compute fast frustum interesection
        /// </summary>
        public virtual SpeedFrustum FastFrustum
        {
            get
            {
                this.UpdateBoundingFrustum();

                return this.spdFrustum;
            }
        }

        /// <summary>
        /// Get the projection matrix of this camera
        /// </summary>
        public virtual Matrix ProjectionTransform
        {
            get
            {
                this.UpdateFrustum();

                return this.projectionMatrix;
            }
        }

        /// <summary>
        /// Get the view matrix of this camera
        /// </summary>
        public virtual Matrix ViewTransform
        {
            get
            {
                this.UpdateView();

                return this.viewMatrix;
            }
        }

        /// <summary>
        /// Get the AABB of the camera (Not implemented...)
        /// </summary>
        public virtual BoundingBox WorldBoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get a boolean indicating if this object is attached to a scene node
        /// </summary>
        public virtual bool IsAttached
        {
            get { return this.parent != null; }
        }

        /// <summary>
        /// Get the name of the camera
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Get a boolean indicating if this object is visible
        /// </summary>
        public virtual bool IsRendered
        {
            get { return false; }
        }

        /// <summary>
        /// Get or set a boolean indicating if the parent node has changed
        /// </summary>
        public virtual bool HasParentChanged
        {
            get { return this.hasParentChanged; }
            set { this.hasParentChanged = value; }
        }

        /// <summary>
        /// Get the parent scene node of this object
        /// </summary>
        public virtual SceneNode Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Get the transform matrix of this object
        /// </summary>
        public virtual Matrix Transform
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Get the direction vector
        /// </summary>
        public virtual Vector3 Direction
        {
            get { return this.transform.Forward; }
        }

        /// <summary>
        /// Get the right vector
        /// </summary>
        public virtual Vector3 Right
        {
            get { return this.transform.Right; }
        }

        /// <summary>
        /// Get the up vector
        /// </summary>
        public virtual Vector3 Up
        {
            get { return this.transform.Up; }
        }

        #endregion
    }
}
