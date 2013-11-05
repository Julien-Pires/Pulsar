﻿using System;

using Microsoft.Xna.Framework;

using Pulsar.Core;
using Pulsar.Mathematic;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Enumerates the type of projection used by a camera
    /// </summary>
    public enum ProjectionType
    {
        Perspective, 
        Orthographic
    }

    /// <summary>
    /// Represents a camera
    /// </summary>
    public class Camera : Transform, IMovable
    {
        #region Fields

        private readonly string _name = string.Empty;
        private readonly SceneTree _owner;
        private bool _isDirtyView = true;
        private bool _isDirtyFrustum = true;
        private bool _isDirtyBoundingFrustum = true;
        private bool _useFixedYaw = true;
        private bool _automaticAspect = true;
        private float _near = 1.0f;
        private float _far = 1000.0f;
        private float _aspectRatio = 16.0f/9.0f;
        private float _fieldOfView = MathHelper.PiOver4;
        private ProjectionType _projectionType = ProjectionType.Perspective;
        private Matrix _viewMatrix = Matrix.Identity;
        private Matrix _projectionMatrix = Matrix.Identity;
        private Vector3 _yawFixedAxis = Vector3.UnitY;
        private Viewport _viewport;
        private BoundingFrustum _frustum = new BoundingFrustum(Matrix.Identity);
        private readonly SpeedFrustum _speedFrustum = new SpeedFrustum();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the Camera class
        /// </summary>
        /// <param name="name">Name of the camera</param>
        /// <param name="owner">SceneTree in which the camera was created</param>
        internal Camera(string name, SceneTree owner)
        {
            _name = name;
            _owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the scene in a specific viewport
        /// </summary>
        /// <param name="vp">Viewport in which to render the scene</param>
        public void Render(Viewport vp)
        {
            if (vp != _viewport) CurrentViewport = vp;
            _owner.RenderScene(vp, this);
        }

        /// <summary>
        /// <remarks>Not implemented for the Camera class</remarks>
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <exception cref="NotImplementedException">Camera doesn't need to be notified about other cameras</exception>
        public void CheckVisibilityWithCamera(Camera cam)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <remarks>Not implemented for the BaseCamera class </remarks>
        /// </summary>
        /// <param name="queue">Current render queue</param>
        /// <exception cref="NotImplementedException">Camera doesn't neeed to be added in a queue</exception>
        public void UpdateRenderQueue(RenderQueue queue)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Uses a fixed vector for yaw
        /// </summary>
        /// <param name="useFixed">Boolean indicating the uses or not of a fixed vector</param>
        /// <param name="fixedAxis">Fixed vector for yaw</param>
        public void UseFixedYaw(bool useFixed, Vector3? fixedAxis)
        {
            fixedAxis = fixedAxis ?? Vector3.UnitY;
            _useFixedYaw = useFixed;
            _yawFixedAxis = fixedAxis.Value;
        }

        /// <summary>
        /// Performs a yaw rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        public void Yaw(float angle)
        {
            Vector3 yAxis = _useFixedYaw ? _yawFixedAxis : Vector3.Transform(Vector3.UnitY, LocalTransformRotation);
            Rotate(ref yAxis, angle, TransformSpace.Parent);
        }

        /// <summary>
        /// Performs a pitch rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        public void Pitch(float angle)
        {
            Vector3 xAxis = Vector3.Transform(Vector3.UnitX, LocalTransformRotation);
            Rotate(ref xAxis, angle, TransformSpace.Parent);
        }

        /// <summary>
        /// Performs a roll rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        public void Roll(float angle)
        {
            Vector3 zAxis = Vector3.Transform(Vector3.UnitZ, LocalTransformRotation);
            Rotate(ref zAxis, angle, TransformSpace.Parent);
        }

        /// <summary>
        /// Sets the direction of the transform
        /// </summary>
        /// <param name="direction">Direction to point toward</param>
        public override void SetDirection(ref Vector3 direction)
        {
            if (_useFixedYaw)
            {
                Vector3 adjustZ;
                Vector3.Negate(ref direction, out adjustZ);
                adjustZ.Normalize();

                Vector3 xAxis;
                Vector3.Cross(ref _yawFixedAxis, ref adjustZ, out xAxis);
                xAxis.Normalize();

                Vector3 yAxis;
                Vector3.Cross(ref adjustZ, ref xAxis, out yAxis);
                yAxis.Normalize();

                Matrix rotation;
                Quaternion targetRotation;
                MatrixExtension.CreateFromAxes(ref xAxis, ref yAxis, ref adjustZ, out rotation);
                Quaternion.CreateFromRotationMatrix(ref rotation, out targetRotation);

                if (ParentTransform != null)
                {
                    Quaternion parentInvert = ParentTransform.LocalRotation;
                    Quaternion.Inverse(ref parentInvert, out parentInvert);
                    Quaternion.Multiply(ref parentInvert, ref targetRotation, out LocalTransformRotation);
                }
                else LocalTransformRotation = targetRotation;

                RequireUpdate();
            }
            else base.SetDirection(ref direction);
        }

        /// <summary>
        /// Called when the transform changed
        /// </summary>
        protected override void OnTransformChanged()
        {
            _isDirtyView = true;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Invalidates the projection
        /// </summary>
        private void RequireFrustumUpdate()
        {
            _isDirtyFrustum = true;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Checks if the bounding frustum need to be updated
        /// </summary>
        private void UpdateBoundingFrustum()
        {
            if (!_isDirtyBoundingFrustum) return;
            UpdateView();
            UpdateProjection();

            Matrix viewProj;
            Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out viewProj);
            _frustum.Matrix = viewProj;
            _speedFrustum.Update(ref _frustum);

            _isDirtyBoundingFrustum = false;
        }

        /// <summary>
        /// Computes the projection matrix depending on the projection type
        /// </summary>
        private void UpdateProjection()
        {
            if(!_isDirtyFrustum) return;

            switch (_projectionType)
            {
                case ProjectionType.Perspective:
                    Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _near, _far, out _projectionMatrix);
                    break;
                case ProjectionType.Orthographic:
                    Matrix.CreateOrthographic(_viewport.Width, _viewport.Height, _near, _far, out _projectionMatrix);
                    break;
            }

            _isDirtyFrustum = false;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Computes the view matrix
        /// </summary>
        private void UpdateView()
        {
            if(!_isDirtyView) return;
            UpdateTransform();

            Matrix.Invert(ref WorldTransform, out _viewMatrix);
            _isDirtyView = false;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Attaches this object to a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        /// <param name="parent">Parent scene node</param>
        public void AttachParent(SceneNode parent)
        {
            ParentTransform = parent;
            RequireUpdate();
        }

        /// <summary>
        /// Detaches this object of a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        public void DetachParent()
        {
            ParentTransform = null;
            RequireUpdate();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent node
        /// </summary>
        public new SceneNode Parent
        {
            get { return (SceneNode) ParentTransform; }
        }

        /// <summary>
        /// Gets or sets a boolean indicating if the camera is visible (frustum plane)
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the viewport in wich this camera will render
        /// </summary>
        public Viewport CurrentViewport
        {
            get { return _viewport; }
            internal set
            {
                _viewport = value;
                if (!_automaticAspect) return;

                _aspectRatio = _viewport.AspectRatio;
                RequireFrustumUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the projection type of this camera
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return _projectionType; }
            set
            {
                _projectionType = value;
                RequireFrustumUpdate();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the aspect ratio is automatically computed 
        /// by the viewport associated with this camera
        /// </summary>
        public bool AutomaticAspectRatio
        {
            get { return _automaticAspect; }
            set { _automaticAspect = value; }
        }

        /// <summary>
        /// Gets or sets the aspect ratio
        /// </summary>
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; }
        }

        /// <summary>
        /// Gets or sets the field of view of this camera
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                _fieldOfView = value;
                RequireFrustumUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the near plane of this camera
        /// </summary>
        public float NearPlane
        {
            get { return _near; }
            set
            {
                _near = value;
                RequireFrustumUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the far plane of this camera
        /// </summary>
        public float FarPlane
        {
            get { return _far; }
            set
            {
                _far = value;
                RequireFrustumUpdate();
            }
        }

        /// <summary>
        /// Gets the bounding frustum of the camera
        /// </summary>
        public BoundingFrustum Frustum
        {
            get
            {
                UpdateBoundingFrustum();

                return _frustum;
            }
        }

        /// <summary>
        /// Gets a SpeedFrustum instance to compute fast frustum interesection
        /// </summary>
        public SpeedFrustum FastFrustum
        {
            get
            {
                UpdateBoundingFrustum();

                return _speedFrustum;
            }
        }

        /// <summary>
        /// Gets the projection matrix of this camera
        /// </summary>
        public Matrix Projection
        {
            get
            {
                UpdateProjection();

                return _projectionMatrix;
            }
        }

        /// <summary>
        /// Gets the view matrix of this camera
        /// </summary>
        public Matrix View
        {
            get
            {
                UpdateView();

                return _viewMatrix;
            }
        }

        /// <summary>
        /// Gets the AABB of the camera (Not implemented...)
        /// </summary>
        public BoundingBox WorldBoundingBox
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a boolean indicating if this object is attached to a scene node
        /// </summary>
        public bool IsAttached
        {
            get { return ParentTransform != null; }
        }

        /// <summary>
        /// Gets the name of the camera
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a boolean indicating if this object is visible
        /// </summary>
        public bool IsRendered
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets a boolean indicating if the parent node has changed
        /// </summary>
        public bool HasParentChanged { get; set; }

        /// <summary>
        /// Gets the transform matrix of this object
        /// </summary>
        public Matrix Transform
        {
            get { return LocalToWorld; }
        }

        #endregion
    }
}
