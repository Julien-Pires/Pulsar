using System;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Rendering;
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
    public sealed class Camera : IMovable
    {
        #region Fields

        internal Matrix ViewMatrix = Matrix.Identity;
        internal Matrix ProjectionMatrix = Matrix.Identity;

        private readonly string _name = string.Empty;
        private readonly SceneTree _owner;
        private SceneNode _parent;
        private bool _isDirtyView = true;
        private bool _isDirtyFrustum = true;
        private bool _isDirtyBoundingFrustum = true;
        private bool _useFixedYaw = true;
        private ProjectionType _projectionType = ProjectionType.Perspective;
        private Matrix _transform = Matrix.Identity;
        private float _near = 1.0f;
        private float _far = 1000.0f;
        private float _aspectRatio = 16.0f/9.0f;
        private float _fieldOfView = MathHelper.PiOver4;
        private Vector3 _yawFixedAxis = Vector3.UnitY;
        private Quaternion _orientation = Quaternion.Identity;
        private Vector3 _position = Vector3.Zero;
        private Quaternion _lastNodeOrientation = Quaternion.Identity;
        private Vector3 _lastNodePosition = Vector3.Zero;
        private Quaternion _fullOrientation = Quaternion.Identity;
        private Vector3 _fullPosition = Vector3.Zero;
        private Viewport _viewport;
        private BoundingFrustum _frustum = new BoundingFrustum(Matrix.Identity);
        private SpeedFrustum _spdFrustum;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the BaseCamera class
        /// </summary>
        /// <param name="name">Name of the camera</param>
        /// <param name="owner">SceneTree in which the camera was created</param>
        internal Camera(string name, SceneTree owner)
        {
            HasParentChanged = false;
            _name = name;
            _owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Render the scene in a specific viewport
        /// </summary>
        /// <param name="vp">Viewport in which to render the scene</param>
        public void Render(Viewport vp)
        {
            if (vp != _viewport) Viewport = vp;
            _owner.RenderScene(vp, this);
        }

        /// <summary>
        /// <remarks>Not implemented for the BaseCamera class</remarks>
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <exception cref="NotImplementedException">Camera doesn't need to be notified about other cameras</exception>
        public void NotifyCurrentCamera(Camera cam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <remarks>Not implemented for the BaseCamera class </remarks>
        /// </summary>
        /// <param name="queue">Current render queue</param>
        /// <exception cref="NotImplementedException">Camera doesn't neeed to be added in a queue</exception>
        public void UpdateRenderQueue(RenderQueue queue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use a fixed vector for yaw
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
        /// Move the camera's position by a specific vector in the world coordinate
        /// </summary>
        /// <param name="v">Vector to add for translate</param>
        public void Translate(Vector3 v)
        {
            Vector3.Add(ref _position, ref v, out _position);

            InvalidateView();
        }

        /// <summary>
        /// Move the camera's position by a specific vector in the local coordinate
        /// </summary>
        /// <param name="v">Vector to add for translate</param>
        public void TranslateRelative(Vector3 v)
        {
            Vector3 mov = Vector3.Transform(v, _orientation);
            Vector3.Add(ref _position, ref mov, out _position);
        }

        /// <summary>
        /// Execute a yaw operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public void Yaw(float angle)
        {
            Vector3 axis = _useFixedYaw ? _yawFixedAxis : Vector3.Transform(Vector3.UnitY, _orientation);
            Rotate(axis, angle);

            InvalidateView();
        }

        /// <summary>
        /// Execute a pitch operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public void Pitch(float angle)
        {
            Vector3 axis = Vector3.Transform(Vector3.UnitX, _orientation);
            Rotate(axis, angle);

            InvalidateView();
        }

        /// <summary>
        /// Execute a roll operation
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public void Roll(float angle)
        {
            Vector3 axis = Vector3.Transform(Vector3.UnitZ, _orientation);
            Rotate(axis, angle);

            InvalidateView();
        }

        /// <summary>
        /// Rotate around an axis
        /// </summary>
        /// <param name="axis">Axis to rotate around</param>
        /// <param name="angle">Angle to rotate</param>
        public void Rotate(Vector3 axis, float angle)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
            Rotate(q);
        }

        /// <summary>
        /// Rotate around an axis with a quaternion
        /// </summary>
        /// <param name="q">Quaternion used for rotation</param>
        public void Rotate(Quaternion q)
        {
            q.Normalize();
            Quaternion.Multiply(ref q, ref _orientation, out _orientation);

            InvalidateView();
        }

        /// <summary>
        /// Set a direction on wich the camera is looking
        /// </summary>
        /// <param name="target">Direction to look for</param>
        public void LookAt(Vector3 target)
        {
            UpdateView();
            SetDirection(target - _fullPosition);
        }

        /// <summary>
        /// Set the direction where the camera is watching
        /// </summary>
        /// <param name="v">Direction vector</param>
        public void SetDirection(Vector3 v)
        {
            Quaternion targetOrientation;
            Vector3 adjustZ = -v;
            adjustZ.Normalize();

            if (_useFixedYaw)
            {
                Vector3 vecX = Vector3.Cross(_yawFixedAxis, adjustZ);
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
                UpdateView();
                _fullOrientation.GetAxes(out axes);

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

                Quaternion.Multiply(ref rotationQuat, ref _fullOrientation, out targetOrientation);
            }

            if (_parent != null)
            {
                _orientation = Quaternion.Inverse(_parent.Orientation) * targetOrientation;
            }
            else
            {
                _orientation = targetOrientation;
            }

            InvalidateView();
        }

        /// <summary>
        /// Check if the frustum is out of date and update values accordingly
        /// </summary>
        /// <returns>Return true if the frustum is out of date, else false</returns>
        private bool IsProjectionOutOfDate()
        {
            if (IsViewOutOfDate()) _isDirtyFrustum = true;

            return _isDirtyFrustum;
        }

        /// <summary>
        /// Check if view is out of date and update values accordingly
        /// </summary>
        private bool IsViewOutOfDate()
        {
            if (_parent != null)
            {
                if((_isDirtyView) || ((_lastNodeOrientation != _parent.FullOrientation) ||
                    (_lastNodePosition != _parent.FullPosition)))
                {
                    _lastNodeOrientation = _parent.FullOrientation;
                    _lastNodePosition = _parent.FullPosition;
                    Quaternion.Multiply(ref _lastNodeOrientation, ref _orientation, out _fullOrientation);
                    _fullPosition = Vector3.Add(Vector3.Transform(_position, _lastNodeOrientation), _lastNodePosition);

                    InvalidateView();
                }
            }
            else
            {
                _fullOrientation = _orientation;
                _fullPosition = _position;
            }

            return _isDirtyView;
        }

        /// <summary>
        /// Invalidate the projection
        /// </summary>
        private void InvalidateFrustum()
        {
            _isDirtyFrustum = true;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Invalidate the view
        /// </summary>
        private void InvalidateView()
        {
            _isDirtyView = true;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Check if view matrix need to be updated
        /// </summary>
        private void UpdateView()
        {
            if (IsViewOutOfDate()) ComputeView();
        }

        /// <summary>
        /// Check if projection matrix need to be updated
        /// </summary>
        private void UpdateFrustum()
        {
            if (IsProjectionOutOfDate()) ComputeProjection();
        }

        /// <summary>
        /// Check if the bounding frustum need to be updated
        /// </summary>
        private void UpdateBoundingFrustum()
        {
            UpdateView();
            UpdateFrustum();

            if (_isDirtyBoundingFrustum) ComputeBoundingFrustum();
        }

        /// <summary>
        /// Compute the bounding frustum
        /// </summary>
        private void ComputeBoundingFrustum()
        {
            _frustum.Matrix = ViewMatrix * ProjectionMatrix;
            _spdFrustum = new SpeedFrustum(ref _frustum);
            _isDirtyBoundingFrustum = false;
        }

        /// <summary>
        /// Compute the projection matrix depending on the projection type
        /// </summary>
        private void ComputeProjection()
        {
            switch (_projectionType)
            {
                case ProjectionType.Perspective:
                    Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _near, 
                        _far, out ProjectionMatrix);
                    break;
                case ProjectionType.Orthographic:
                    Matrix.CreateOrthographic(_viewport.Width, _viewport.Height, _near, _far, out ProjectionMatrix);
                    break;
            }

            _isDirtyFrustum = false;
        }

        /// <summary>
        /// Compute the view matrix
        /// </summary>
        private void ComputeView()
        {
            if (_isDirtyView)
            {
                _transform = Matrix.CreateFromQuaternion(_fullOrientation) * Matrix.CreateTranslation(_fullPosition);
                Matrix.Invert(ref _transform, out ViewMatrix);
            }
            _isDirtyView = false;
            _isDirtyBoundingFrustum = true;
        }

        /// <summary>
        /// Attach this object to a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        /// <param name="parent">Parent scene node</param>
        public void AttachParent(SceneNode parent)
        {
            _parent = parent;

            InvalidateView();
        }

        /// <summary>
        /// Detach this object of a scene node<br />
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        public void DetachParent()
        {
            _parent = null;

            InvalidateView();
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
            get { return _viewport; }
            internal set
            {
                _viewport = value;
                _aspectRatio = _viewport.AspectRatio;
                InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the projection type of this camera
        /// </summary>
        public ProjectionType Projection
        {
            get { return _projectionType; }
            set
            {
                _projectionType = value;
                InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the field of view of this camera
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                _fieldOfView = value;
                InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the near plane of this camera
        /// </summary>
        public float NearPlane
        {
            get { return _near; }
            set
            {
                _near = value;
                InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the far plane of this camera
        /// </summary>
        public float FarPlane
        {
            get { return _far; }
            set
            {
                _far = value;
                InvalidateFrustum();
            }
        }

        /// <summary>
        /// Get or set the position of the camera
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                InvalidateView();
            }
        }

        /// <summary>
        /// Get the bounding frustum of the camera
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
        /// Get a SpeedFrustum instance to compute fast frustum interesection
        /// </summary>
        public SpeedFrustum FastFrustum
        {
            get
            {
                UpdateBoundingFrustum();

                return _spdFrustum;
            }
        }

        /// <summary>
        /// Get the projection matrix of this camera
        /// </summary>
        public Matrix ProjectionTransform
        {
            get
            {
                UpdateFrustum();

                return ProjectionMatrix;
            }
        }

        /// <summary>
        /// Get the view matrix of this camera
        /// </summary>
        public Matrix ViewTransform
        {
            get
            {
                UpdateView();

                return ViewMatrix;
            }
        }

        /// <summary>
        /// Get the AABB of the camera (Not implemented...)
        /// </summary>
        public BoundingBox WorldBoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get a boolean indicating if this object is attached to a scene node
        /// </summary>
        public bool IsAttached
        {
            get { return _parent != null; }
        }

        /// <summary>
        /// Get the name of the camera
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get a boolean indicating if this object is visible
        /// </summary>
        public bool IsRendered
        {
            get { return false; }
        }

        /// <summary>
        /// Get or set a boolean indicating if the parent node has changed
        /// </summary>
        public bool HasParentChanged { get; set; }

        /// <summary>
        /// Get the parent scene node of this object
        /// </summary>
        public SceneNode Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Get the transform matrix of this object
        /// </summary>
        public Matrix Transform
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Get the direction vector
        /// </summary>
        public Vector3 Direction
        {
            get { return _transform.Forward; }
        }

        /// <summary>
        /// Get the right vector
        /// </summary>
        public Vector3 Right
        {
            get { return _transform.Right; }
        }

        /// <summary>
        /// Get the up vector
        /// </summary>
        public Vector3 Up
        {
            get { return _transform.Up; }
        }

        #endregion
    }
}
