using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Represents a node in a scene graph
    /// </summary>
    public class SceneNode : Node
    {
        #region Fields

        protected readonly SceneTree Owner;

        private BoundingBox _boundingBox;
        private readonly Dictionary<string, IMovable> _movablesByName = new Dictionary<string, IMovable>();
        private readonly List<IMovable> _movablesList = new List<IMovable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of SceneNode class
        /// </summary>
        /// <param name="owner">SceneTree instance wich create this node</param>
        internal SceneNode(SceneTree owner)
        {
            Owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new child node
        /// </summary>
        /// <returns>Returns a new child node</returns>
        protected override Node CreateChildIntern()
        {
            return Owner.CreateNode();
        }

        /// <summary>
        /// Destroys a child node
        /// </summary>
        /// <param name="child">Child to destroy</param>
        protected override void DestroyChildIntern(Node child)
        {
            SceneNode node = child as SceneNode;
            if(node == null) throw new InvalidCastException("");

            node.Owner.DestroyNode(node);
        }

        /// <summary>
        /// Attaches a movable object to this scene node
        /// </summary>
        /// <param name="movObj">IMovable instance</param>
        public void AttachObject(IMovable movObj)
        {
            if (_movablesByName.ContainsKey(movObj.Name))
            {
                _movablesByName.Remove(movObj.Name);
                _movablesList.Remove(movObj);
            }

            _movablesByName.Add(movObj.Name, movObj);
            _movablesList.Add(movObj);
            movObj.AttachParent(this);

            RequireUpdate();
        }

        /// <summary>
        /// Detaches a movable from this scene node
        /// </summary>
        /// <param name="name">Name of the movable object</param>
        public void DetachObject(string name)
        {
            if (!_movablesByName.ContainsKey(name)) return;

            IMovable movObj = _movablesByName[name];
            movObj.DetachParent();
            _movablesByName.Remove(name);
            _movablesList.Remove(movObj);

            RequireUpdate();
        }

        /// <summary>
        /// Finds all visible objects attached to this scene node
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <param name="queue">Current render queue</param>
        /// <param name="addChildren">Boolean indicating if the search goes through childrens</param>
        internal void FindVisibleObjects(Camera cam, RenderQueue queue, bool addChildren)
        {
            for (int i = 0; i < _movablesList.Count; i++)
                queue.ProcessVisibleObject(cam, _movablesList[i]);

            if (addChildren)
            {
                for (int j = 0; j < Childrens.Count; j++)
                    ((SceneNode)Childrens[j]).FindVisibleObjects(cam, queue, true);
            }
        }
        
        /// <summary>
        /// Updates this node and all his child
        /// </summary>
        /// <param name="updateChild">Boolean indicating if child has to be updated</param>
        protected internal override void Update(bool updateChild)
        {
            base.Update(updateChild);

            UpdateBounds();
        }

        /// <summary>
        /// Updates the bounding volumes
        /// </summary>
        private void UpdateBounds()
        {
            _boundingBox = new BoundingBox();

            for (int i = 0; i < _movablesList.Count; i++)
            {
                IMovable obj = _movablesList[i];
                BoundingBox objBox = obj.WorldBoundingBox;
                BoundingBox.CreateMerged(ref _boundingBox, ref objBox, out _boundingBox);
            }

            for (int i = 0; i < Childrens.Count; i++)
            {
                SceneNode node = (SceneNode)Childrens[i];
                BoundingBox.CreateMerged(ref _boundingBox, ref node._boundingBox, out _boundingBox);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the scene tree which created this node
        /// </summary>
        public SceneTree Scene
        {
            get { return Owner; }
        }

        #endregion
    }
}
