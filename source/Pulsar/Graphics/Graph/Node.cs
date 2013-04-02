using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Graph
{
    /// <summary>
    /// Enum defining space used to perform transform operation relative to
    /// </summary>
    public enum TransformSpace { World, Parent, Local };

    public abstract class Node
    {
        #region Fields

        protected string name = string.Empty;
        protected bool needUpdateTransform = true;
        protected bool needParentUpdate = true;
        protected bool needUpdateChild = false;
        protected bool parentAskedForUpdate = false;
        protected Matrix fullTransform = Matrix.Identity;
        protected Quaternion orientation = Quaternion.Identity;
        protected Vector3 scale = Vector3.One;
        protected Vector3 position = Vector3.Zero;
        protected Quaternion fullOrientation = Quaternion.Identity;
        protected Vector3 fullScale = Vector3.One;
        protected Vector3 fullPosition = Vector3.Zero;
        protected Node parent = null;
        protected Dictionary<string, Node> childrensMap = new Dictionary<string, Node>();
        protected List<Node> childrensList = new List<Node>();
        protected List<Node> childldrensToUpdate = new List<Node>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Node class
        /// </summary>
        /// <param name="name">Name of the node</param>
        public Node(string name)
        {
            this.name = name;

            this.NeedUpdate(false);
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
            this.childrensMap.TryGetValue(name, out child);
            if(child != null)
            {
                throw new Exception(string.Format("A child with the name {0} already exist", name));
            }

            child = this.CreateChildIntern(name);
            child.parent = this;
            this.childrensMap.Add(name, child);
            this.childrensList.Add(child);

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
            this.childrensMap.TryGetValue(name, out child);
            if (child == null)
            {
                return false;
            }
            if (!this.RemoveChildIntern(name))
            {
                return false;
            }

            child.parent = null;
            this.childrensMap.Remove(name);
            this.childrensList.Remove(child);

            return true;
        }

        /// <summary>
        /// Internal method to create a child node
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns>Return a new child node</returns>
        protected abstract Node CreateChildIntern(string name);

        protected abstract bool RemoveChildIntern(string name);

        /// <summary>
        /// Set a new position for this scene node
        /// </summary>
        /// <param name="newPos">Vector for the new position</param>
        public virtual void SetPosition(Vector3 newPos)
        {
            this.position = newPos;

            this.NeedUpdate(false);
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
                    this.position += Vector3.Transform(v, this.orientation);
                    break;
                case TransformSpace.Parent:
                    if (this.parent != null)
                    {
                        this.position += Vector3.Transform(v, Quaternion.Inverse(this.parent.FullOrientation))
                            / this.parent.FullScale;
                    }
                    else
                    {
                        this.position += v;
                    }
                    break;
            }

            this.NeedUpdate(false);
        }

        /// <summary>
        /// Execute a yaw operation
        /// </summary>
        /// <param name="angle">Angle of the yaw</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Yaw(float angle, TransformSpace space)
        {
            this.Rotate(angle, Vector3.UnitY, space);
        }

        /// <summary>
        /// Execute a pitch operation
        /// </summary>
        /// <param name="angle">Angle of the pitch</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Pitch(float angle, TransformSpace space)
        {
            this.Rotate(angle, Vector3.UnitX, space);
        }

        /// <summary>
        /// Execute a roll operation
        /// </summary>
        /// <param name="angle">Angle of the roll</param>
        /// <param name="space">Space used to perform the operartion relative to</param>
        public virtual void Roll(float angle, TransformSpace space)
        {
            this.Rotate(angle, Vector3.UnitZ, space);
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

            this.Rotate(q, space);
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
                    Quaternion.Multiply(ref q, ref this.orientation, out this.orientation);
                    break;
                case TransformSpace.Parent:
                    this.orientation = this.orientation * Quaternion.Inverse(this.FullOrientation) * q * this.FullOrientation;
                    break;
                case TransformSpace.Local:
                    Quaternion.Multiply(ref this.orientation, ref q, out this.orientation);
                    break;
            }

            this.NeedUpdate(false);
        }

        /// <summary>
        /// Scale this scene node using the same factor for all axis
        /// </summary>
        /// <param name="factor">Factor used to perform the scale operation</param>
        public virtual void DoScale(float factor)
        {
            this.DoScale(factor, factor, factor);
        }

        /// <summary>
        /// Scale this scene node using one specific factor for each axis
        /// </summary>
        /// <param name="x">X axis scale factor</param>
        /// <param name="y">Y axis scale factor</param>
        /// <param name="z">Z axis scale factor</param>
        public virtual void DoScale(float x, float y, float z)
        {
            this.scale.X *= x;
            this.scale.Y *= y;
            this.scale.Z *= z;
            this.NeedUpdate(false);
        }

        /// <summary>
        /// Scale this scene node with a scale vector
        /// </summary>
        /// <param name="v">Vector containing scale factor for each axis</param>
        public virtual void DoScale(Vector3 v)
        {
            this.scale *= v;
            this.NeedUpdate(false);
        }

        /// <summary>
        /// Indicate that this node need to be updated
        /// </summary>
        /// <param name="forceParentUpdate">If true the parent have to update this node</param>
        protected virtual void NeedUpdate(bool forceParentUpdate)
        {
            this.needUpdateTransform = true;
            this.needUpdateChild = true;
            this.needParentUpdate = true;

            if ((this.parent != null) && (!this.parentAskedForUpdate || forceParentUpdate))
            {
                this.parent.RequestUpdate(this, forceParentUpdate);
                this.parentAskedForUpdate = true;
            }

            this.childldrensToUpdate.Clear();
        }

        /// <summary>
        /// Receive a request for updating a specific child
        /// </summary>
        /// <param name="child">Child node to update</param>
        /// <param name="forceParentUpdate">If true the parent have to update this node</param>
        protected virtual void RequestUpdate(Node child, bool forceParentUpdate)
        {
            this.childldrensToUpdate.Add(child);

            if ((this.parent != null) && (!this.parentAskedForUpdate || forceParentUpdate))
            {
                this.parent.RequestUpdate(this, forceParentUpdate);
                this.parentAskedForUpdate = true;
            }
        }

        /// <summary>
        /// Update node child and all values (rotation, scale, ...) relative to the parent
        /// </summary>
        /// <param name="updateChild">If true forced a child update, false child will not update</param>
        protected internal virtual void Update(bool updateChild, bool parentHasChanged)
        {
            this.parentAskedForUpdate = false;

            if (this.needParentUpdate || parentHasChanged)
            {
                this.UpdateWithParent();
            }

            if (updateChild)
            {
                if (this.needUpdateChild)
                {
                    for (int i = 0; i < this.childrensList.Count; i++)
                    {
                        this.childrensList[i].Update(true, true);
                    }
                }
                else
                {
                    for (int i = 0; i < this.childldrensToUpdate.Count; i++)
                    {
                        this.childldrensToUpdate[i].Update(true, false);
                    }
                }

                this.childldrensToUpdate.Clear();
                this.needUpdateChild = false;
            }
        }

        /// <summary>
        /// Update values(rotation, scale, ...) relative to the parent
        /// </summary>
        protected void UpdateWithParent()
        {
            if (this.parent != null)
            {
                Vector3 parentPos = this.parent.FullPosition;
                Vector3 parentScale = this.parent.FullScale;
                Quaternion parentOri = this.parent.FullOrientation;

                this.fullScale = parentScale * this.scale;
                this.fullOrientation = parentOri * this.orientation;
                this.fullPosition = Vector3.Transform((parentScale * this.position), parentOri) + parentPos;
            }
            else
            {
                this.fullOrientation = this.orientation;
                this.fullPosition = this.position;
                this.fullScale = this.scale;
            }

            this.needUpdateTransform = true;
            this.needParentUpdate = false;
        }

        /// <summary>
        /// Apply the transform matrix of this node to a vector
        /// </summary>
        /// <param name="pos">Origin vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyTransform(ref Vector3 pos, out Vector3 result)
        {
            Vector3.Multiply(ref pos, ref this.fullScale, out result);
            Vector3.Transform(ref result, ref this.fullOrientation, out result);
            Vector3.Add(ref result, ref this.fullPosition, out result);
        }

        /// <summary>
        /// Apply the scale and position of this node to a vector
        /// </summary>
        /// <param name="pos">Origin vector</param>
        /// <param name="result">Result vector</param>
        public void ApplyScaleTrans(ref Vector3 pos, out Vector3 result)
        {
            Vector3.Multiply(ref pos, ref this.fullScale, out result);
            Vector3.Add(ref result, ref this.fullPosition, out result);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the parent node
        /// </summary>
        public Node Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Get the name of this scene node
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Get the the full transform matrix
        /// </summary>
        public virtual Matrix FullTransform
        {
            get
            {
                if (this.needUpdateTransform)
                {
                    this.fullTransform = Matrix.CreateScale(this.fullScale) * Matrix.CreateFromQuaternion(this.fullOrientation) *
                        Matrix.CreateTranslation(this.fullPosition);

                    this.needUpdateTransform = false;
                }

                return this.fullTransform;
            }
        }

        /// <summary>
        /// Get or set the position of the node
        /// </summary>
        public virtual Vector3 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the scale of the node
        /// </summary>
        public virtual Vector3 Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                this.NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get or set the orientation of the node
        /// </summary>
        public virtual Quaternion Orientation
        {
            get { return this.orientation; }
            set
            {
                this.orientation = value;
                this.NeedUpdate(false);
            }
        }

        /// <summary>
        /// Get the full translation
        /// </summary>
        public virtual Vector3 FullPosition
        {
            get
            {
                if (this.needParentUpdate)
                {
                    this.UpdateWithParent();
                }

                return this.fullPosition;
            }
        }

        /// <summary>
        /// Get the full scale
        /// </summary>
        public virtual Vector3 FullScale
        {
            get
            {
                if (this.needParentUpdate)
                {
                    this.UpdateWithParent();
                }

                return this.fullScale;
            }
        }

        /// <summary>
        /// Get the full rotation
        /// </summary>
        public virtual Quaternion FullOrientation
        {
            get
            {
                if (this.needParentUpdate)
                {
                    this.UpdateWithParent();
                }

                return this.fullOrientation;
            }
        }

        #endregion
    }
}
