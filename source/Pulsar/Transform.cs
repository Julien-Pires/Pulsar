using Microsoft.Xna.Framework;

using Pulsar.Mathematic;

namespace Pulsar
{
    /// <summary>
    /// Represents the methods that be called when a transform changed
    /// </summary>
    /// <param name="transform"></param>
    public delegate void TransformChangedEventHandler(Transform transform);

    /// <summary>
    /// Represents a transformation in a 3D space
    /// A transformation is defined by a position - rotation - scale
    /// Each transformation can have a parent transformation but when a parent changed it doesn't notify
    /// theirs childs. When a child updates itself it will try to update its parent if necessary.
    /// Transform class can be inherited to implements specific update behaviour between childs and parents
    /// </summary>
    public class Transform
    {
        #region Fields

        protected internal Matrix WorldTransform = Matrix.Identity;
        protected internal Matrix InverseWorldTransform = Matrix.Identity;
        protected internal Quaternion WorldTransformRotation = Quaternion.Identity;
        protected internal Quaternion LocalTransformRotation = Quaternion.Identity;
        protected internal Vector3 WorldTransformPosition;
        protected internal Vector3 WorldTransformScale = Vector3.One;
        protected internal Vector3 LocalTransformPosition;
        protected internal Vector3 LocalTransformScale = Vector3.One;
        protected internal Transform ParentTransform;

        private bool _transformChanged;
        private bool _matrixDirty;
        private readonly Vector3[] _directionAxis = new Vector3[3];

        #endregion

        #region Events

        /// <summary>
        /// Occurred when the transform changed
        /// </summary>
        public event TransformChangedEventHandler TransformChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Transform class
        /// </summary>
        public Transform()
        {
        }

        /// <summary>
        /// Constructor of Transform class
        /// </summary>
        /// <param name="matrix">Matrix used to initialize the transform</param>
        public Transform(Matrix matrix)
        {
            UpdateFromMatrix(ref matrix);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates a new transform with identity matrix
        /// </summary>
        /// <returns>Returns a new transform instance</returns>
        public static Transform CreateIdentity()
        {
            return new Transform(Matrix.Identity);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs a yaw rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Yaw(float angle, TransformSpace space = TransformSpace.Local)
        {
            Vector3 yAxis = Vector3.UnitY;
            Rotate(ref yAxis, angle, space);
        }

        /// <summary>
        /// Performs a roll rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Roll(float angle, TransformSpace space = TransformSpace.Local)
        {
            Vector3 zAxis = Vector3.UnitZ;
            Rotate(ref zAxis, angle, space);
        }

        /// <summary>
        /// Performs a pitch rotation
        /// </summary>
        /// <param name="angle">Angle of rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Pitch(float angle, TransformSpace space = TransformSpace.Local)
        {
            Vector3 xAxis = Vector3.UnitX;
            Rotate(ref xAxis, angle, space);
        }

        /// <summary>
        /// Performs a rotation around an axis
        /// </summary>
        /// <param name="axis">Axis to rotate around</param>
        /// <param name="angle">Angle of rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Rotate(Vector3 axis, float angle, TransformSpace space = TransformSpace.Local)
        {
            Rotate(ref axis, angle, space);
        }

        /// <summary>
        /// Performs a rotation around an axis
        /// </summary>
        /// <param name="axis">Axis to rotate around</param>
        /// <param name="angle">Angle of rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Rotate(ref Vector3 axis, float angle, TransformSpace space = TransformSpace.Local)
        {
            Quaternion rotation;
            Quaternion.CreateFromAxisAngle(ref axis, angle, out rotation);
            Rotate(ref rotation, space);
        }

        /// <summary>
        /// Performs a rotation
        /// </summary>
        /// <param name="rotation">Quaternion representing the rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Rotate(Quaternion rotation, TransformSpace space = TransformSpace.Local)
        {
            Rotate(ref rotation, space);
        }

        /// <summary>
        /// Performs a rotation
        /// </summary>
        /// <param name="rotation">Quaternion representing the rotation</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Rotate(ref Quaternion rotation, TransformSpace space = TransformSpace.Local)
        {
            Quaternion normRotation;
            Quaternion.Normalize(ref rotation, out normRotation);
            switch (space)
            {
                case TransformSpace.Local:
                    Quaternion.Multiply(ref LocalTransformRotation, ref normRotation, out LocalTransformRotation);
                    break;
                case TransformSpace.Parent:
                    Quaternion.Multiply(ref normRotation, ref LocalTransformRotation, out LocalTransformRotation);
                    break;
                case TransformSpace.World:
                    if(_transformChanged) 
                        UpdateWithParent();

                    Quaternion invertedWorld;
                    Quaternion.Inverse(ref WorldTransformRotation, out invertedWorld);
                    Quaternion.Multiply(ref LocalTransformRotation, ref invertedWorld, out LocalTransformRotation);
                    Quaternion.Multiply(ref LocalTransformRotation, ref normRotation, out LocalTransformRotation);
                    Quaternion.Multiply(ref LocalTransformRotation, ref WorldTransformRotation, out LocalTransformRotation);
                    break;
            }

            RequireUpdate();
        }

        /// <summary>
        /// Sets the transform to point toward a specific target
        /// </summary>
        /// <param name="target">Target to look at</param>
        public void LookAt(Vector3 target)
        {
            LookAt(ref target);
        }

        /// <summary>
        /// Sets the transform to point toward a specific target
        /// </summary>
        /// <param name="target">Target to look at</param>
        public void LookAt(ref Vector3 target)
        {
            if(_transformChanged) 
                UpdateWithParent();

            Vector3 direction;
            Vector3.Subtract(ref target, ref WorldTransformPosition, out direction);
            SetDirection(ref direction);
        }

        /// <summary>
        /// Sets the direction of the transform
        /// </summary>
        /// <param name="direction">Direction to point toward</param>
        public void SetDirection(Vector3 direction)
        {
            SetDirection(ref direction);
        }

        /// <summary>
        /// Sets the direction of the transform
        /// </summary>
        /// <param name="direction">Direction to point toward</param>
        public virtual void SetDirection(ref Vector3 direction)
        {
            Vector3 adjustZ;
            Vector3.Negate(ref direction, out adjustZ);
            adjustZ.Normalize();

            if(_transformChanged) 
                UpdateWithParent();
            WorldTransformRotation.GetAxis(_directionAxis);
            
            Vector3 zDiff;
            Vector3.Add(ref _directionAxis[2], ref adjustZ, out zDiff);
            Quaternion rotationDiff;
            if (zDiff.LengthSquared() >= 0.00005f)
            {
                Vector3 fallback = new Vector3();
                QuaternionExtension.GetArcRotation(ref _directionAxis[2], ref adjustZ, ref fallback, out rotationDiff);
            }
            else 
                Quaternion.CreateFromAxisAngle(ref _directionAxis[1], MathHelper.ToRadians(MathHelper.Pi), out rotationDiff);

            Quaternion targetRotation;
            Quaternion.Multiply(ref rotationDiff, ref WorldTransformRotation, out targetRotation);
            if (ParentTransform != null)
            {
                Quaternion parentInvert;
                Quaternion.Inverse(ref ParentTransform.LocalTransformRotation, out parentInvert);
                Quaternion.Multiply(ref parentInvert, ref targetRotation, out LocalTransformRotation);
            }
            else 
                LocalTransformRotation = targetRotation;

            RequireUpdate();
        }

        /// <summary>
        /// Translates the transform
        /// </summary>
        /// <param name="move">Vector used to translate</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Translate(Vector3 move, TransformSpace space = TransformSpace.Local)
        {
            Translate(ref move, space);
        }

        /// <summary>
        /// Translates the transform
        /// </summary>
        /// <param name="move">Vector used to translate</param>
        /// <param name="space">Space in which the operation is performed</param>
        public void Translate(ref Vector3 move, TransformSpace space = TransformSpace.Local)
        {
            switch (space)
            {
                case TransformSpace.Local:
                    Vector3 orientedMove;
                    Vector3.Transform(ref move, ref LocalTransformRotation, out orientedMove);
                    Vector3.Add(ref LocalTransformPosition, ref orientedMove, out LocalTransformPosition);
                    break;
                case TransformSpace.Parent:
                    Vector3.Add(ref LocalTransformPosition, ref move, out LocalTransformPosition);
                    break;
                case TransformSpace.World:
                    if (ParentTransform != null)
                    {
                        if (_transformChanged) 
                            UpdateWithParent();

                        Quaternion invertedRotation;
                        Vector3 transformedMove;
                        Quaternion.Inverse(ref ParentTransform.WorldTransformRotation, out invertedRotation);
                        Vector3.Transform(ref move, ref invertedRotation, out transformedMove);
                        Vector3.Divide(ref transformedMove, ref ParentTransform.WorldTransformScale, out transformedMove);
                        Vector3.Add(ref LocalTransformPosition, ref transformedMove, out LocalTransformPosition);
                    }
                    else 
                        Vector3.Add(ref LocalTransformPosition, ref move, out LocalTransformPosition);
                    break;
            }

            RequireUpdate();
        }

        /// <summary>
        /// Performs an uniformed scale
        /// </summary>
        /// <param name="factor">Scaling factor</param>
        public void DoScale(float factor)
        {
            DoScale(factor, factor, factor);
        }

        /// <summary>
        /// Performs a scale
        /// </summary>
        /// <param name="x">Scaling factor on X axis</param>
        /// <param name="y">Scaling factor on Y axis</param>
        /// <param name="z">Scaling factor on Z axis</param>
        public void DoScale(float x, float y, float z)
        {
            LocalTransformScale.X *= x;
            LocalTransformScale.Y *= y;
            LocalTransformScale.Z *= z;
            RequireUpdate();
        }

        /// <summary>
        /// Performs a scale
        /// </summary>
        /// <param name="scale">Vector representing the scale factor</param>
        public void DoScale(Vector3 scale)
        {
            Vector3.Multiply(ref LocalTransformScale, ref scale, out LocalTransformScale);
            RequireUpdate();
        }

        /// <summary>
        /// Performs a scale
        /// </summary>
        /// <param name="scale">Vector representing the scale factor</param>
        public void DoScale(ref Vector3 scale)
        {
            Vector3.Multiply(ref LocalTransformScale, ref scale, out LocalTransformScale);
            RequireUpdate();
        }

        /// <summary>
        /// Gets the local to world matrix
        /// </summary>
        /// <param name="result">Matrix containing the local to world transform</param>
        public void GetLocalToWorld(out Matrix result)
        {
            result = WorldTransform;
        }

        /// <summary>
        /// Gets the world to local matrix
        /// </summary>
        /// <param name="result">Matrix containing the world to local transform</param>
        public void GetWorldToLocal(out Matrix result)
        {
            result = InverseWorldTransform;
        }

        /// <summary>
        /// Gets the local axis
        /// </summary>
        /// <param name="result">Matrix containing the local axes that formed the coordinate system of this transform</param>
        public void GetLocalAxis(out Matrix result)
        {
            Matrix.CreateFromQuaternion(ref LocalTransformRotation, out result);
        }

        /// <summary>
        /// Transforms a point from local space to world space
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <returns>Returns a new point in world space</returns>
        public Vector3 TransformToWorld(Vector3 point)
        {
            Vector3 result;
            Vector3.Transform(ref point, ref WorldTransform, out result);

            return result;
        }

        /// <summary>
        /// Transforms a point from local space to world space
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <param name="result">Vector receiving the transformed point</param>
        public void TransformToWorld(ref Vector3 point, out Vector3 result)
        {
            Vector3.Transform(ref point, ref WorldTransform, out result);
        }

        /// <summary>
        /// Transforms a set of points from local space to world space
        /// </summary>
        /// <param name="points">Points to transform</param>
        /// <returns>Returns an array that contains transformed points</returns>
        public Vector3[] TransformToWorld(Vector3[] points)
        {
            Vector3[] results = new Vector3[points.Length];
            Vector3.Transform(points, ref WorldTransform, results);

            return results;
        }

        /// <summary>
        /// Transforms a set of points from local space to world space to a destination array
        /// </summary>
        /// <param name="points">Points to transform</param>
        /// <param name="results">Destination array</param>
        public void TransformToWorld(Vector3[] points, Vector3[] results)
        {
            Vector3.Transform(points, ref WorldTransform, results);
        }

        /// <summary>
        /// Transforms a point from world space to local space
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <returns>Returns a new point in local space</returns>
        public Vector3 TransformToLocal(Vector3 point)
        {
            Vector3 result;
            Vector3.Transform(ref point, ref InverseWorldTransform, out result);

            return result;
        }

        /// <summary>
        /// Transforms a point from world space to local space
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <param name="result">Vector receiving the transformed point</param>
        public void TransformToLocal(ref Vector3 point, out Vector3 result)
        {
            Vector3.Transform(ref point, ref InverseWorldTransform, out result);
        }

        /// <summary>
        /// Transforms a set of points from world space to local space
        /// </summary>
        /// <param name="points">Points to transform</param>
        /// <returns>Return an array that contains transformed points</returns>
        public Vector3[] TransformToLocal(Vector3[] points)
        {
            Vector3[] results = new Vector3[points.Length];
            Vector3.Transform(points, ref InverseWorldTransform, results);

            return results;
        }

        /// <summary>
        /// Transforms a set of points from world space to local space
        /// </summary>
        /// <param name="points">Points to transform</param>
        /// <param name="results">Destination array</param>
        public void TransformToLocal(Vector3[] points, Vector3[] results)
        {
            Vector3.Transform(points, ref InverseWorldTransform, results);
        }

        /// <summary>
        /// Update the transform by extracting the scale - rotation - position from a matrix
        /// </summary>
        /// <param name="matrix">Matrix used to update the transform</param>
        public void UpdateFromMatrix(Matrix matrix)
        {
            matrix.Decompose(out LocalTransformScale, out LocalTransformRotation, out LocalTransformPosition);
            RequireUpdate();
        }

        /// <summary>
        /// Update the transform by extracting the scale - rotation - position from a matrix
        /// </summary>
        /// <param name="matrix">Matrix used to update the transform</param>
        public void UpdateFromMatrix(ref Matrix matrix)
        {
            matrix.Decompose(out LocalTransformScale, out LocalTransformRotation, out LocalTransformPosition);
            RequireUpdate();
        }

        /// <summary>
        /// Updates this transform
        /// </summary>
        public virtual void Update()
        {
            UpdateWithParent();
        }

        /// <summary>
        /// Sets the transform as dirty
        /// </summary>
        protected void RequireUpdate()
        {
            _transformChanged = true;
            _matrixDirty = true;
            OnTransformChanged();

            RaiseTransformChanged(this);
        }

        /// <summary>
        /// Called when the transform changed
        /// </summary>
        protected virtual void OnTransformChanged()
        {
        }

        /// <summary>
        /// Raises the change event
        /// </summary>
        /// <param name="transform">Transform that changed</param>
        private void RaiseTransformChanged(Transform transform)
        {
            TransformChangedEventHandler handler = TransformChanged;
            if (handler != null) 
                handler(transform);
        }

        /// <summary>
        /// Updates all world properties of this transform
        /// </summary>
        /// <returns>Returns true if the transform has been updated otherwise false</returns>
        protected bool UpdateWithParent()
        {
            if(!_transformChanged) 
                return false;

            if (ParentTransform != null)
            {
                if(ParentTransform._transformChanged) 
                    ParentTransform.UpdateWithParent();

                Vector3 transformedPosition;
                Vector3.Multiply(ref ParentTransform.WorldTransformScale, ref LocalTransformPosition, out transformedPosition);
                Vector3.Transform(ref transformedPosition, ref ParentTransform.WorldTransformRotation, out transformedPosition);
                Vector3.Add(ref transformedPosition, ref ParentTransform.WorldTransformPosition, out WorldTransformPosition);
                Vector3.Multiply(ref ParentTransform.WorldTransformScale, ref LocalTransformScale, out WorldTransformScale);
                Quaternion.Multiply(ref ParentTransform.WorldTransformRotation, ref LocalTransformRotation, out WorldTransformRotation);
            }
            else
            {
                WorldTransformPosition = LocalTransformPosition;
                WorldTransformScale = LocalTransformScale;
                WorldTransformRotation = LocalTransformRotation;
            }

            _transformChanged = false;
            _matrixDirty = true;

            return true;
        }

        /// <summary>
        /// Updates the transform matrix
        /// </summary>
        /// <returns>Return true if matrices has been updated otherwise false</returns>
        protected bool UpdateTransform()
        {
            if(!_matrixDirty) 
                return false;

            if (_transformChanged) 
                UpdateWithParent();

            MatrixExtension.CreateWorld(ref LocalTransformPosition, ref LocalTransformRotation, ref LocalTransformScale, out WorldTransform);
            Matrix.Invert(ref WorldTransform, out InverseWorldTransform);
            _matrixDirty = false;

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the local to world matrix
        /// </summary>
        public Matrix LocalToWorld
        {
            get
            {
                UpdateTransform();

                return WorldTransform;
            }
        }

        /// <summary>
        /// Gets the world to local matrix
        /// </summary>
        public Matrix WorldToLocal
        {
            get
            {
                UpdateTransform();

                return InverseWorldTransform;
            }
        }

        /// <summary>
        /// Gets the local axes that described the coordinate system of this transform
        /// </summary>
        public Matrix LocalAxis
        {
            get
            {
                Matrix result;
                Matrix.CreateFromQuaternion(ref LocalTransformRotation, out result);

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the parent transform
        /// </summary>
        public virtual Transform Parent
        {
            get { return ParentTransform; }
            set
            {
                ParentTransform = value;
                RequireUpdate();
            }
        }

        /// <summary>
        /// Gets the forward vector
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                UpdateTransform();

                return WorldTransform.Forward;
            }
        }

        /// <summary>
        /// Gets the up vector
        /// </summary>
        public Vector3 Up
        {
            get
            {
                UpdateTransform();

                return WorldTransform.Up;
            }
        }

        /// <summary>
        /// Gets the right vector
        /// </summary>
        public Vector3 Right
        {
            get
            {
                UpdateTransform();

                return WorldTransform.Right;
            }
        }

        /// <summary>
        /// Gets or sets the local position
        /// </summary>
        public Vector3 LocalPosition
        {
            get { return LocalTransformPosition; }
            set
            {
                LocalTransformPosition = value;
                RequireUpdate();
            }
        }

        /// <summary>
        /// Gets the world position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                UpdateWithParent();

                return WorldTransformPosition;
            }
        }

        /// <summary>
        /// Gets or sets the local scale
        /// </summary>
        public Vector3 LocalScale
        {
            get { return LocalTransformScale; }
            set
            {
                LocalTransformScale = value;
                RequireUpdate();
            }
        }

        /// <summary>
        /// Gets the world scale
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                UpdateWithParent();

                return WorldTransformScale;
            }
        }

        /// <summary>
        /// Gets or sets the local rotation
        /// </summary>
        public Quaternion LocalRotation
        {
            get { return LocalTransformRotation; }
            set
            {
                LocalTransformRotation = value;
                RequireUpdate();
            }
        }

        /// <summary>
        /// Gets the world rotation
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                UpdateWithParent();

                return WorldTransformRotation;
            }
        }

        #endregion
    }
}