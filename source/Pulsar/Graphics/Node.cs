using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Enumerates space used to perform transform operation
    /// </summary>
    public enum TransformSpace
    {
        World, 
        Parent, 
        Local
    };

    /// <summary>
    /// Base class for a scene tree node
    /// </summary>
    public abstract class Node
    {
        #region Fields

        protected readonly Dictionary<string, Node> ChildrensMap = new Dictionary<string, Node>();
        protected readonly List<Node> ChildrensList = new List<Node>();
        protected readonly List<Node> ChildldrensToUpdate = new List<Node>();

        private bool _needUpdateTransform = true;
        private bool _needParentUpdate = true;
        private bool _needUpdateChild;
        private bool _parentAskedForUpdate;
        private Matrix _scaleTransform = Matrix.Identity;
        private Matrix _orientationTransform = Matrix.Identity;
        private Matrix _positionTransform = Matrix.Identity;
        private Matrix _scaleOrientTransform = Matrix.Identity;
        private Matrix _fullTransform = Matrix.Identity;
        private Quaternion _nodeOrientation = Quaternion.Identity;
        private Quaternion _fullOrientation = Quaternion.Identity;
        private Vector3 _nodeScale = Vector3.One;
        private Vector3 _nodePosition;
        private Vector3 _fullScale = Vector3.One;
        private Vector3 _fullPosition;
        private Node _parentNode;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Node class
        /// </summary>
        /// <param name="name">Name of the node</param>
        protected Node(string name)
        {
            Name = name;
            NeedUpdate(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a child for this scene node
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>Returns an instance of SceneNode class wich is a child of this scene node</returns>
        public virtual Node CreateChild(string name)
        {
            Node child;
            ChildrensMap.TryGetValue(name, out child);
            if(child != null)
                throw new Exception(string.Format("A child with the name {0} already exist", name));

            child = CreateChildIntern(name);
            child._parentNode = this;
            ChildrensMap.Add(name, child);
            ChildrensList.Add(child);

            return child;
        }

        /// <summary>
        /// Remove a child
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>Returns true if the child is removed, otherwise false</returns>
        public virtual bool RemoveChild(string name)
        {
            Node child;
            ChildrensMap.TryGetValue(name, out child);
            if (child == null) return false;
            if (!RemoveChildIntern(name)) return false;

            child._parentNode = null;
            ChildrensMap.Remove(name);
            ChildrensList.Remove(child);

            return true;
        }

        /// <summary>
        /// Internal method to create a child node
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>Return a new child node</returns>
        protected abstract Node CreateChildIntern(string name);

        /// <summary>
        /// Internal method to remove a child node
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>Return true if the child is removed successfully otherwise false</returns>
        protected abstract bool RemoveChildIntern(string name);

        /// <summary>
        /// Set a new position for this scene node
        /// </summary>
        /// <param name="position">Vector for the new position</param>
        public virtual void SetPosition(Vector3 position)
        {
            SetPosition(ref position);
        }

        /// <summary>
        /// Set a new position for this scene node
        /// </summary>
        /// <param name="position">Vector for the new position</param>
        public virtual void SetPosition(ref Vector3 position)
        {
            _nodePosition = position;
            NeedUpdate(false);
        }

        /// <summary>
        /// Translate this scene node
        /// </summary>
        /// <param name="v">Translation vector</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Translate(Vector3 v, TransformSpace space)
        {
            Translate(ref v, space);
        }

        /// <summary>
        /// Translate this scene node
        /// </summary>
        /// <param name="v">Translation vector</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Translate(ref Vector3 v, TransformSpace space)
        {
            switch (space)
            {
                case TransformSpace.Local:
                    Vector3 move;
                    Vector3.Transform(ref v, ref _nodeOrientation, out move);
                    Vector3.Add(ref _nodePosition, ref move, out _nodePosition);
                    break;
                case TransformSpace.Parent:
                    if (_parentNode != null)
                    {
                        Quaternion invertedOrient;
                        Quaternion.Inverse(ref _parentNode._fullOrientation, out invertedOrient);
                        Vector3 transformedMove;
                        Vector3.Transform(ref v, ref invertedOrient, out transformedMove);
                        Vector3.Divide(ref transformedMove, ref _parentNode._fullScale, out transformedMove);
                        Vector3.Add(ref _nodePosition, ref transformedMove, out _nodePosition);
                    }
                    else
                        Vector3.Add(ref _nodePosition, ref v, out _nodePosition);
                    break;
            }

            NeedUpdate(false);
        }

        /// <summary>
        /// Execute a yaw operation
        /// </summary>
        /// <param name="angle">Angle of the yaw</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Yaw(float angle, TransformSpace space)
        {
            Rotate(angle, Vector3.UnitY, space);
        }

        /// <summary>
        /// Execute a pitch operation
        /// </summary>
        /// <param name="angle">Angle of the pitch</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Pitch(float angle, TransformSpace space)
        {
            Rotate(angle, Vector3.UnitX, space);
        }

        /// <summary>
        /// Execute a roll operation
        /// </summary>
        /// <param name="angle">Angle of the roll</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Roll(float angle, TransformSpace space)
        {
            Rotate(angle, Vector3.UnitZ, space);
        }

        /// <summary>
        /// Rotate this scene node about an axis
        /// </summary>
        /// <param name="angle">Angle of the rotation</param>
        /// <param name="axis">Axis used to perform the rotation</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Rotate(float angle, Vector3 axis, TransformSpace space)
        {
            Rotate(angle, ref axis, space);
        }

        /// <summary>
        /// Rotate this scene node about an axis
        /// </summary>
        /// <param name="angle">Angle of the rotation</param>
        /// <param name="axis">Axis used to perform the rotation</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Rotate(float angle, ref Vector3 axis, TransformSpace space)
        {
            Quaternion q;
            Quaternion.CreateFromAxisAngle(ref axis, angle, out q);

            Rotate(ref q, space);
        }

        /// <summary>
        /// Rotate this scene node using a quaternion
        /// </summary>
        /// <param name="q">Quaternion containing the rotation information</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        protected virtual void Rotate(Quaternion q, TransformSpace space)
        {
            Rotate(ref q, space);
        }

        /// <summary>
        /// Rotate this scene node using a quaternion
        /// </summary>
        /// <param name="q">Quaternion containing the rotation information</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        protected virtual void Rotate(ref Quaternion q, TransformSpace space)
        {
            Quaternion inverted;
            Quaternion.Normalize(ref q, out inverted);

            switch (space)
            {
                case TransformSpace.World:
                    Quaternion.Multiply(ref inverted, ref _nodeOrientation, out _nodeOrientation);
                    break;
                case TransformSpace.Parent:
                    Quaternion w;
                    Quaternion.Inverse(ref _fullOrientation, out w);
                    Quaternion.Multiply(ref _nodeOrientation, ref w, out w);
                    Quaternion.Multiply(ref w, ref inverted, out w);
                    Quaternion.Multiply(ref w, ref _fullOrientation, out w);
                    _nodeOrientation = w;
                    break;
                case TransformSpace.Local:
                    Quaternion.Multiply(ref _nodeOrientation, ref inverted, out _nodeOrientation);
                    break;
            }

            NeedUpdate(false);
        }

        /// <summary>
        /// Scale this scene node using the same factor for all axis
        /// </summary>
        /// <param name="factor">Factor used to perform the scale operation</param>
        public virtual void DoScale(float factor)
        {
            DoScale(factor, factor, factor);
        }

        /// <summary>
        /// Scale this scene node using one specific factor for each axis
        /// </summary>
        /// <param name="x">X axis scale factor</param>
        /// <param name="y">Y axis scale factor</param>
        /// <param name="z">Z axis scale factor</param>
        public virtual void DoScale(float x, float y, float z)
        {
            _nodeScale.X *= x;
            _nodeScale.Y *= y;
            _nodeScale.Z *= z;
            NeedUpdate(false);
        }

        /// <summary>
        /// Scale this scene node with a scale vector
        /// </summary>
        /// <param name="v">Vector containing scale factor for each axis</param>
        public virtual void DoScale(Vector3 v)
        {
            DoScale(ref v);
        }

        /// <summary>
        /// Scale this scene node with a scale vector
        /// </summary>
        /// <param name="v">Vector containing scale factor for each axis</param>
        public virtual void DoScale(ref Vector3 v)
        {
            Vector3.Multiply(ref _nodeScale, ref v, out _nodeScale);
            NeedUpdate(false);
        }

        /// <summary>
        /// Indicate that this node need to be updated
        /// </summary>
        /// <param name="forceParentUpdate">If true the parent have to update this node</param>
        protected virtual void NeedUpdate(bool forceParentUpdate)
        {
            _needUpdateTransform = true;
            _needUpdateChild = true;
            _needParentUpdate = true;

            if ((_parentNode != null) && (!_parentAskedForUpdate || forceParentUpdate))
            {
                _parentNode.RequestUpdate(this, forceParentUpdate);
                _parentAskedForUpdate = true;
            }

            ChildldrensToUpdate.Clear();
        }

        /// <summary>
        /// Receive a request for updating a specific child
        /// </summary>
        /// <param name="child">Child node to update</param>
        /// <param name="forceParentUpdate">If true the parent have to update this node</param>
        protected virtual void RequestUpdate(Node child, bool forceParentUpdate)
        {
            ChildldrensToUpdate.Add(child);

            if ((_parentNode != null) && (!_parentAskedForUpdate || forceParentUpdate))
            {
                _parentNode.RequestUpdate(this, forceParentUpdate);
                _parentAskedForUpdate = true;
            }
        }

        /// <summary>
        /// Update node child and all values (rotation, scale, ...) relative to the parent
        /// </summary>
        /// <param name="updateChild">If true forced a child update, false child will not update</param>
        /// <param name="parentHasChanged">Indicates if the parent has changed</param>
        protected internal virtual void Update(bool updateChild, bool parentHasChanged)
        {
            _parentAskedForUpdate = false;

            if (_needParentUpdate || parentHasChanged) UpdateWithParent();

            if (updateChild)
            {
                if (_needUpdateChild)
                {
                    for (int i = 0; i < ChildrensList.Count; i++)
                    {
                        ChildrensList[i].Update(true, true);
                    }
                }
                else
                {
                    for (int i = 0; i < ChildldrensToUpdate.Count; i++)
                    {
                        ChildldrensToUpdate[i].Update(true, false);
                    }
                }

                ChildldrensToUpdate.Clear();
                _needUpdateChild = false;
            }
        }

        /// <summary>
        /// Update values(rotation, scale, ...) relative to the parent
        /// </summary>
        /// <remarks>
        /// ParentNode.AbsoluteScale will trigger UpdateWithParent() of parent, so we can bypass AbsoluteOrientation 
        /// and AbsolutePosition properties and use fields directly because parent will be up to date 
        /// </remarks>
        protected void UpdateWithParent()
        {
            if (_parentNode != null)
            {
                Vector3 parentScale = _parentNode.AbsoluteScale;
                Vector3.Multiply(ref parentScale, ref _nodeScale, out _fullScale);
                Quaternion.Multiply(ref _parentNode._fullOrientation, ref _nodeOrientation, out _fullOrientation);

                Vector3 v;
                Vector3.Multiply(ref parentScale, ref _nodePosition, out v);
                Vector3.Transform(ref v, ref _parentNode._fullOrientation, out v);
                Vector3.Add(ref v, ref _parentNode._fullPosition, out _fullPosition);
            }
            else
            {
                _fullOrientation = _nodeOrientation;
                _fullPosition = _nodePosition;
                _fullScale = _nodeScale;
            }

            _needUpdateTransform = true;
            _needParentUpdate = false;
        }

        /// <summary>
        /// Update the transform matrix
        /// </summary>
        private void UpdateTransform()
        {
            if (!_needUpdateTransform) return;

            Matrix.CreateScale(ref _fullScale, out _scaleTransform);
            Matrix.CreateFromQuaternion(ref _fullOrientation, out _orientationTransform);
            Matrix.CreateTranslation(ref _fullPosition, out _positionTransform);
            Matrix.Multiply(ref _scaleTransform, ref _orientationTransform, out _scaleOrientTransform);
            Matrix.Multiply(ref _scaleOrientTransform, ref _positionTransform, out _fullTransform);
            _needUpdateTransform = false;
        }

        /// <summary>
        /// Apply the transform matrix of this node to a vector
        /// </summary>
        /// <param name="v">Vector</param>
        public Vector3 ApplyTransform(Vector3 v)
        {
            if (_needParentUpdate) UpdateWithParent();
            UpdateTransform();

            Vector3 result;
            Vector3.Transform(ref v, ref _fullTransform, out result);

            return result;
        }

        /// <summary>
        /// Apply the transform matrix of this node to a vector
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyTransform(ref Vector3 v, out Vector3 result)
        {
            if(_needParentUpdate) UpdateWithParent();
            UpdateTransform();

            Vector3.Transform(ref v, ref _fullTransform, out result);
        }

        /// <summary>
        /// Apply the scale and position of this node to a vector
        /// </summary>
        /// <param name="v">Origin vector</param>
        public Vector3 ApplyScalePosition(Vector3 v)
        {
            if (_needParentUpdate) UpdateWithParent();

            Vector3 result;
            Vector3.Multiply(ref v, ref _fullScale, out result);
            Vector3.Add(ref result, ref _fullPosition, out result);

            return result;
        }

        /// <summary>
        /// Apply the scale and position of this node to a vector
        /// </summary>
        /// <param name="v">Origin vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyScalePosition(ref Vector3 v, out Vector3 result)
        {
            if (_needParentUpdate) UpdateWithParent();

            Vector3.Multiply(ref v, ref _fullScale, out result);
            Vector3.Add(ref result, ref _fullPosition, out result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the parent node
        /// </summary>
        public Node Parent
        {
            get { return _parentNode; }
        }

        /// <summary>
        /// Get the name of this scene node
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Get the full transform matrix
        /// </summary>
        public virtual Matrix Transform
        {
            get
            {
                UpdateTransform();

                return _fullTransform;
            }
        }

        /// <summary>
        /// Get the Scale-Orientation transform matrix
        /// </summary>
        public virtual Matrix ScaleOrientationTransform
        {
            get
            {
                UpdateTransform();

                return _scaleOrientTransform;
            }
        }

        /// <summary>
        /// Get or set the position of the node
        /// </summary>
        public virtual Vector3 Position
        {
            get { return _nodePosition; }
            set
            {
                _nodePosition = value;
                NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the scale of the node
        /// </summary>
        public virtual Vector3 Scale
        {
            get { return _nodeScale; }
            set
            {
                _nodeScale = value;
                NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the orientation of the node
        /// </summary>
        public virtual Quaternion Orientation
        {
            get { return _nodeOrientation; }
            set
            {
                _nodeOrientation = value;
                NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get the absolute translation
        /// </summary>
        public virtual Vector3 AbsolutePosition
        {
            get
            {
                if (_needParentUpdate) UpdateWithParent();

                return _fullPosition;
            }
        }

        /// <summary>
        /// Get the absolute scale
        /// </summary>
        public virtual Vector3 AbsoluteScale
        {
            get
            {
                if (_needParentUpdate) UpdateWithParent();

                return _fullScale;
            }
        }

        /// <summary>
        /// Get the absolute rotation
        /// </summary>
        public virtual Quaternion AbsoluteOrientation
        {
            get
            {
                if (_needParentUpdate) UpdateWithParent();

                return _fullOrientation;
            }
        }

        #endregion
    }
}
