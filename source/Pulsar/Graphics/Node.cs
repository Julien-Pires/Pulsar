using System;
using System.Collections.Generic;

using Pulsar.Core;

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
    /// Represents a node in a tree
    /// Each node can have one parent and multiple childrens
    /// </summary>
    public abstract class Node : Transform
    {
        #region Fields

        protected readonly List<Node> Childrens = new List<Node>();
        protected readonly List<Node> ChildldrensToUpdate = new List<Node>();

        private bool _needUpdateChild;
        private bool _parentAskedForUpdate;
        private Node _parentNode;
        private int _depth;

        #endregion

        #region Methods

        /// <summary>
        /// Creates and attaches a child to this node
        /// </summary>
        /// <returns>Returns a new child node</returns>
        public Node CreateChild()
        {
            Node child = CreateChildIntern();
            AddChild(child);

            return child;
        }

        /// <summary>
        /// Performs internal operation when a child node is about to be created.
        /// Used to create specific child when this class is inherited
        /// </summary>
        /// <returns>Returns a new child node</returns>
        protected abstract Node CreateChildIntern();

        /// <summary>
        /// Performs internal operation when a child node is about to be destroyed.
        /// Used when this class is inherited
        /// </summary>
        /// <param name="child">Child to destroy</param>
        protected abstract void DestroyChildIntern(Node child);

        /// <summary>
        /// Destroys a child node
        /// </summary>
        /// <param name="child">Child to destroy</param>
        public void DestroyChild(Node child)
        {
            if (child._parentNode != this) throw new ArgumentException("", "child");

            child.DestroyAllChild();
            RemoveChild(child);
            DestroyChildIntern(child);
        }

        /// <summary>
        /// Destroys all child node
        /// </summary>
        public void DestroyAllChild()
        {
            for (int i = 0; i < Childrens.Count; i++)
            {
                Node child = Childrens[i];
                child.DestroyAllChild();
                child.SetParent(null);
                DestroyChildIntern(child);
            }
            Childrens.Clear();
        }

        /// <summary>
        /// Add a child
        /// </summary>
        /// <param name="child">Node to add as a child</param>
        public void AddChild(Node child)
        {
            if (child == null) throw new ArgumentNullException("child");
            if (child._parentNode != null)
            {
                if (child._parentNode != this) throw new ArgumentException("", "child");

                return;
            }

            Childrens.Add(child);
            child.SetParent(this);
        }

        /// <summary>
        /// Remove a child
        /// </summary>
        /// <param name="child">Child to remove</param>
        /// <returns></returns>
        public bool RemoveChild(Node child)
        {
            if(child._parentNode != this) throw new ArgumentException("", "child");

            Childrens.Remove(child);
            child.SetParent(null);

            return true;
        }

        /// <summary>
        /// Sets the parent of this node
        /// </summary>
        /// <param name="parent">Parent node</param>
        private void SetParent(Node parent)
        {
            bool changed = (parent != _parentNode);
            _parentNode = parent;
            ParentTransform = parent;
            if (!changed) return;

            if (_parentNode != null) _depth = _parentNode._depth + 1;
            else _depth = 0;

            for(int i = 0; i < Childrens.Count; i++) Childrens[i].UpdateDepthLevel();
        }

        /// <summary>
        /// Update the depth of this node and all its childs
        /// </summary>
        private void UpdateDepthLevel()
        {
            _depth = _parentNode._depth + 1;
            for (int i = 0; i < Childrens.Count; i++) Childrens[i].UpdateDepthLevel();
        }

        /// <summary>
        /// Called when the transform changed
        /// </summary>
        protected override void OnTransformChanged()
        {
            _needUpdateChild = true;
            if ((ParentTransform != null) && !_parentAskedForUpdate)
            {
                _parentNode.RequestUpdate(this);
                _parentAskedForUpdate = true;
            }

            ChildldrensToUpdate.Clear();
        }

        /// <summary>
        /// Receives a request for updating a specific child
        /// </summary>
        /// <param name="child">Child node to update</param>
        protected virtual void RequestUpdate(Node child)
        {
            ChildldrensToUpdate.Add(child);

            if ((ParentTransform == null) || _parentAskedForUpdate) return;

            _parentNode.RequestUpdate(this);
            _parentAskedForUpdate = true;
        }

        /// <summary>
        /// Updates childs
        /// </summary>
        /// <param name="updateChild">If true forced a child update, false child will not update</param>
        protected internal virtual void Update(bool updateChild)
        {
            _parentAskedForUpdate = false;
            UpdateWithParent();

            if (!updateChild) return;

            if (_needUpdateChild)
            {
                for (int i = 0; i < Childrens.Count; i++) 
                    Childrens[i].Update(true);
            }
            else
            {
                for (int i = 0; i < ChildldrensToUpdate.Count; i++)
                    ChildldrensToUpdate[i].Update(true);
            }

            ChildldrensToUpdate.Clear();
            _needUpdateChild = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the parent transform
        /// </summary>
        public override Transform Parent
        {
            get { return base.Parent; }
            set { throw new NotSupportedException("Cannot directly set Parent on Node instance, use AddChild instead"); }
        }

        /// <summary>
        /// Gets the parent node
        /// </summary>
        public Node ParentNode
        {
            get { return _parentNode; }
        }

        #endregion
    }
}
