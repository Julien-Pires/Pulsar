using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Enum defining space used to perform transform operation relative to
    /// </summary>
    public enum TransformSpace { World, Parent, Local };

    /// <summary>
    /// Base class for a scene tree node
    /// </summary>
    public abstract class Node
    {
        #region Fields

        protected bool NeedUpdateTransform = true;
        protected bool NeedParentUpdate = true;
        protected bool NeedUpdateChild = false;
        protected bool ParentAskedForUpdate = false;
        protected Matrix ScaleOrientTransform = Matrix.Identity;
        protected Matrix FullTransform = Matrix.Identity;
        protected Quaternion NodeOrientation = Quaternion.Identity;
        protected Vector3 NodeScale = Vector3.One;
        protected Vector3 NodePosition = Vector3.Zero;
        protected Quaternion FullOrientation = Quaternion.Identity;
        protected Vector3 FullScale = Vector3.One;
        protected Vector3 FullPosition = Vector3.Zero;
        protected Node ParentNode = null;
        protected Dictionary<string, Node> ChildrensMap = new Dictionary<string, Node>();
        protected List<Node> ChildrensList = new List<Node>();
        protected List<Node> ChildldrensToUpdate = new List<Node>();

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
            {
                throw new Exception(string.Format("A child with the name {0} already exist", name));
            }

            child = CreateChildIntern(name);
            child.ParentNode = this;
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
            if (child == null)
            {
                return false;
            }
            if (!RemoveChildIntern(name))
            {
                return false;
            }

            child.ParentNode = null;
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
        /// <param name="newPos">Vector for the new position</param>
        public virtual void SetPosition(Vector3 newPos)
        {
            NodePosition = newPos;

            NeedUpdate(false);
        }

        /// <summary>
        /// Translate this scene node
        /// </summary>
        /// <param name="v">Translation vector</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Translate(Vector3 v, TransformSpace space)
        {
            switch (space)
            {
                case TransformSpace.Local:
                    NodePosition += Vector3.Transform(v, NodeOrientation);
                    break;
                case TransformSpace.Parent:
                    if (ParentNode != null)
                    {
                        NodePosition += Vector3.Transform(v, Quaternion.Inverse(ParentNode.AbsoluteOrientation))
                            / ParentNode.AbsoluteScale;
                    }
                    else
                    {
                        NodePosition += v;
                    }
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
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);

            Rotate(q, space);
        }

        /// <summary>
        /// Rotate this scene node using a quaternion
        /// </summary>
        /// <param name="q">Quaternion containing the rotation information</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        protected virtual void Rotate(Quaternion q, TransformSpace space)
        {
            q.Normalize();

            switch (space)
            {
                case TransformSpace.World:
                    Quaternion.Multiply(ref q, ref NodeOrientation, out NodeOrientation);
                    break;
                case TransformSpace.Parent:
                    NodeOrientation = NodeOrientation * Quaternion.Inverse(AbsoluteOrientation) * q * AbsoluteOrientation;
                    break;
                case TransformSpace.Local:
                    Quaternion.Multiply(ref NodeOrientation, ref q, out NodeOrientation);
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
            NodeScale.X *= x;
            NodeScale.Y *= y;
            NodeScale.Z *= z;
            NeedUpdate(false);
        }

        /// <summary>
        /// Scale this scene node with a scale vector
        /// </summary>
        /// <param name="v">Vector containing scale factor for each axis</param>
        public virtual void DoScale(Vector3 v)
        {
            NodeScale *= v;
            NeedUpdate(false);
        }

        /// <summary>
        /// Indicate that this node need to be updated
        /// </summary>
        /// <param name="forceParentUpdate">If true the parent have to update this node</param>
        protected virtual void NeedUpdate(bool forceParentUpdate)
        {
            NeedUpdateTransform = true;
            NeedUpdateChild = true;
            NeedParentUpdate = true;

            if ((ParentNode != null) && (!ParentAskedForUpdate || forceParentUpdate))
            {
                ParentNode.RequestUpdate(this, forceParentUpdate);
                ParentAskedForUpdate = true;
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

            if ((ParentNode != null) && (!ParentAskedForUpdate || forceParentUpdate))
            {
                ParentNode.RequestUpdate(this, forceParentUpdate);
                ParentAskedForUpdate = true;
            }
        }

        /// <summary>
        /// Update node child and all values (rotation, scale, ...) relative to the parent
        /// </summary>
        /// <param name="updateChild">If true forced a child update, false child will not update</param>
        /// <param name="parentHasChanged">Indicates if the parent has changed</param>
        protected internal virtual void Update(bool updateChild, bool parentHasChanged)
        {
            ParentAskedForUpdate = false;

            if (NeedParentUpdate || parentHasChanged)
            {
                UpdateWithParent();
            }

            if (updateChild)
            {
                if (NeedUpdateChild)
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
                NeedUpdateChild = false;
            }
        }

        /// <summary>
        /// Update values(rotation, scale, ...) relative to the parent
        /// </summary>
        protected void UpdateWithParent()
        {
            if (ParentNode != null)
            {
                Vector3 parentPos = ParentNode.AbsolutePosition;
                Vector3 parentScale = ParentNode.AbsoluteScale;
                Quaternion parentOri = ParentNode.AbsoluteOrientation;

                FullScale = parentScale * NodeScale;
                FullOrientation = parentOri * NodeOrientation;
                FullPosition = Vector3.Transform((parentScale * NodePosition), parentOri) + parentPos;
            }
            else
            {
                FullOrientation = NodeOrientation;
                FullPosition = NodePosition;
                FullScale = NodeScale;
            }

            NeedUpdateTransform = true;
            NeedParentUpdate = false;
        }

        /// <summary>
        /// Update the transform matrix
        /// </summary>
        private void UpdateTransform()
        {
            if (NeedUpdateTransform)
            {
                ScaleOrientTransform = Matrix.CreateScale(FullScale) * Matrix.CreateFromQuaternion(FullOrientation);
                FullTransform = ScaleOrientTransform * Matrix.CreateTranslation(FullPosition);

                NeedUpdateTransform = false;
            }
        }

        /// <summary>
        /// Apply the transform matrix of this node to a vector
        /// </summary>
        /// <param name="pos">Origin vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyTransform(ref Vector3 pos, out Vector3 result)
        {
            Vector3.Multiply(ref pos, ref FullScale, out result);
            Vector3.Transform(ref result, ref FullOrientation, out result);
            Vector3.Add(ref result, ref FullPosition, out result);
        }

        /// <summary>
        /// Apply the scale and position of this node to a vector
        /// </summary>
        /// <param name="pos">Origin vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyScalePos(ref Vector3 pos, out Vector3 result)
        {
            Vector3.Multiply(ref pos, ref FullScale, out result);
            Vector3.Add(ref result, ref FullPosition, out result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the parent node
        /// </summary>
        public Node Parent
        {
            get { return ParentNode; }
        }

        /// <summary>
        /// Get the name of this scene node
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Get the the full transform matrix
        /// </summary>
        public virtual Matrix Transform
        {
            get
            {
                UpdateTransform();

                return FullTransform;
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

                return ScaleOrientTransform;
            }
        }

        /// <summary>
        /// Get or set the position of the node
        /// </summary>
        public virtual Vector3 Position
        {
            get { return NodePosition; }
            set
            {
                NodePosition = value;
                NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the scale of the node
        /// </summary>
        public virtual Vector3 Scale
        {
            get { return NodeScale; }
            set
            {
                NodeScale = value;
                NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the orientation of the node
        /// </summary>
        public virtual Quaternion Orientation
        {
            get { return NodeOrientation; }
            set
            {
                NodeOrientation = value;
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
                if (NeedParentUpdate) UpdateWithParent();

                return FullPosition;
            }
        }

        /// <summary>
        /// Get the absolute scale
        /// </summary>
        public virtual Vector3 AbsoluteScale
        {
            get
            {
                if (NeedParentUpdate) UpdateWithParent();

                return FullScale;
            }
        }

        /// <summary>
        /// Get the absolute rotation
        /// </summary>
        public virtual Quaternion AbsoluteOrientation
        {
            get
            {
                if (NeedParentUpdate) UpdateWithParent();

                return FullOrientation;
            }
        }

        #endregion
    }
}
