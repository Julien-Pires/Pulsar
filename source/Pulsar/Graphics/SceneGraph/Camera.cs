using System;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Rendering;
using Pulsar.Mathematic;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Determine the type of projection to be used by a camera
    /// </summary>
    public enum ProjectionType
    {
        Perspective, 
        Orthographic
    }

    /// <summary>
    /// Base class for a camera
    /// </summary>
    public sealed class Camera : IMovable
    {
        #region Fields

        private readonly string _name = string.Empty;
        private readonly SceneTree _owner;
        private SceneNode _parent;
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
        private Matrix _transform = Matrix.Identity;
        private Matrix _positionTransform = Matrix.Identity;
        private Matrix _orientationTransform = Matrix.Identity;
        private Matrix _viewMatrix = Matrix.Identity;
        private Matrix _projectionMatrix = Matrix.Identity;
        private Matrix _viewProjMatrix = Matrix.Identity;
        private Quaternion _orientation = Quaternion.Identity;
        private Quaternion _lastNodeOrientation = Quaternion.Identity;
        private Quaternion _fullOrientation = Quaternion.Identity;
        private Vector3 _position = Vector3.Zero;
        private Vector3 _yawFixedAxis = Vector3.UnitY;
        private Vector3 _lastNodePosition = Vector3.Zero;
        private Vector3 _fullPosition = Vector3.Zero;
        private Viewport _viewport;
        private BoundingFrustum _frustum = new BoundingFrustum(Matrix.Identity);
        private SpeedFrustum _spdFrustum;
        private readonly Vector3[] _tempDirAxis = new Vector3[3];

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
            if (vp != _viewport) CurrentViewport = vp;
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
        /// Translate the camera in a specific transform space
        /// </summary>
        /// <param name="v">Translation vector</param>
        /// <param name="space">Transform space used to translate</param>
        public void Translate(Vector3 v, TransformSpace space)
        {
            Translate(ref v, space);
        }

        /// <summary>
        /// Translate the camera in a specific transform space
        /// </summary>
        /// <param name="v">Translation vector</param>
        /// <param name="space">Transform space used to translate</param>
        public void Translate(ref Vector3 v, TransformSpace space)
        {
            switch (space)
            {
                case TransformSpace.Local:
                    Vector3 move;
                    Vector3.Transform(ref v, ref _orientation, out move);
                    Vector3.Add(ref _position, ref move, out _position);
                    break;
                case TransformSpace.Parent:
                    Vector3.Add(ref _position, ref v, out _position);
                    break;
                case TransformSpace.World:
                    if (_parent != null)
                    {
                        Quaternion invertedOrientation = Quaternion.Inverse(_parent.AbsoluteOrientation);
                        Vector3 transformedMove;
                        Vector3.Transform(ref v, ref invertedOrientation, out transformedMove);
                        transformedMove = Vector3.Divide(transformedMove, _parent.AbsoluteScale);
                        Vector3.Add(ref _position, ref transformedMove, out _position);
                    }
                    else
                        Vector3.Add(ref _position, ref v, out _position);
                    break;
            }
            InvalidateView();
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
            Rotate(ref axis, angle);
        }

        /// <summary>
        /// Rotate around an axis
        /// </summary>
        /// <param name="axis">Axis to rotate around</param>
        /// <param name="angle">Angle to rotate</param>
        public void Rotate(ref Vector3 axis, float angle)
        {
            Quaternion q;
            Quaternion.CreateFromAxisAngle(ref axis, angle, out q);
            Rotate(q);
        }

        /// <summary>
        /// Rotate around an axis with a quaternion
        /// </summary>
        /// <param name="rotation">Quaternion used for rotation</param>
        public void Rotate(Quaternion rotation)
        {
            Rotate(ref rotation);
        }

        /// <summary>
        /// Rotate around an axis with a quaternion
        /// </summary>
        /// <param name="rotation">Quaternion used for rotation</param>
        public void Rotate(ref Quaternion rotation)
        {
            Quaternion q;
            Quaternion.Normalize(ref rotation, out q);
            Quaternion.Multiply(ref q, ref _orientation, out _orientation);
            InvalidateView();
        }

        /// <summary>
        /// Set a direction on wich the camera is looking
        /// </summary>
        /// <param name="target">Direction to look for</param>
        public void LookAt(Vector3 target)
        {
            LookAt(ref target);
        }

        /// <summary>
        /// Set a direction on wich the camera is looking
        /// </summary>
        /// <param name="target">Direction to look for</param>
        public void LookAt(ref Vector3 target)
        {
            UpdateView();
            Vector3 direction;
            Vector3.Subtract(ref target, ref _fullPosition, out direction);
            SetDirection(ref direction);
        }

        /// <summary>
        /// Set the direction where the camera is watching
        /// </summary>
        /// <param name="direction">Direction vector</param>
        public void SetDirection(Vector3 direction)
        {
            SetDirection(ref direction);
        }

        /// <summary>
        /// Set the direction where the camera is watching
        /// </summary>
        /// <param name="direction">Direction vector</param>
        public void SetDirection(ref Vector3 direction)
        {
            Quaternion targetOrientation;
            Vector3 adjustZ;
            Vector3.Negate(ref direction, out adjustZ);
            adjustZ.Normalize();

            if (_useFixedYaw)
            {
                Vector3 vecX;
                Vector3.Cross(ref _yawFixedAxis, ref adjustZ, out vecX);
                vecX.Normalize();

                Vector3 vecY;
                Vector3.Cross(ref adjustZ, ref vecX, out vecY);
                vecY.Normalize();

                Matrix rotation;
                MatrixExtension.CreateFromAxes(ref vecX, ref vecY, ref adjustZ, out rotation);
                Quaternion.CreateFromRotationMatrix(ref rotation, out targetOrientation);
            }
            else
            {
                UpdateView();
                _fullOrientation.GetAxes(_tempDirAxis);

                Quaternion rotationQuat;
                if ((_tempDirAxis[2] + adjustZ).LengthSquared() < 0.00005f)
                    Quaternion.CreateFromAxisAngle(ref _tempDirAxis[1], MathHelper.ToRadians(MathHelper.Pi), out rotationQuat);
                else
                {
                    Vector3 fallB = new Vector3();
                    _tempDirAxis[2].GetArcRotation(ref adjustZ, ref fallB, out rotationQuat);
                }
                Quaternion.Multiply(ref rotationQuat, ref _fullOrientation, out targetOrientation);
            }

            if (_parent != null)
            {
                Quaternion parentInvert = Quaternion.Inverse(_parent.Orientation);
                Quaternion.Multiply(ref parentInvert, ref targetOrientation, out _orientation);
            }
            else _orientation = targetOrientation;

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
                Quaternion absOrien = _parent.AbsoluteOrientation;
                Vector3 absPos = _parent.AbsolutePosition;
                if ((_isDirtyView) || ((_lastNodeOrientation != absOrien) || (_lastNodePosition != absPos)))
                {
                    _lastNodeOrientation = absOrien;
                    _lastNodePosition = absPos;
                    Quaternion.Multiply(ref _lastNodeOrientation, ref _orientation, out _fullOrientation);

                    Vector3 orientedPosition;
                    Vector3.Transform(ref _position, ref _lastNodeOrientation, out orientedPosition);
                    Vector3.Add(ref orientedPosition, ref _lastNodePosition, out _fullPosition);

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
            _frustum.Matrix = _viewProjMatrix;
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
                    Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _near, _far, out _projectionMatrix);
                    break;
                case ProjectionType.Orthographic:
                    Matrix.CreateOrthographic(_viewport.Width, _viewport.Height, _near, _far, out _projectionMatrix);
                    break;
            }
            Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out _viewProjMatrix);

            _isDirtyFrustum = false;
        }

        /// <summary>
        /// Compute the view matrix
        /// </summary>
        private void ComputeView()
        {
            if (_isDirtyView)
            {
                Matrix.CreateFromQuaternion(ref _fullOrientation, out _orientationTransform);
                Matrix.CreateTranslation(ref _fullPosition, out _positionTransform);
                Matrix.Multiply(ref _orientationTransform, ref _positionTransform, out _transform);
                Matrix.Invert(ref _transform, out _viewMatrix);
            }
            Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out _viewProjMatrix);

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
        public Viewport CurrentViewport
        {
            get { return _viewport; }
            internal set
            {
                _viewport = value;
                if (!_automaticAspect) return;

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

                return _projectionMatrix;
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

                return _viewMatrix;
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
            get { return _transform; }
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
