using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Define a Transform node
    /// </summary>
    public sealed class SceneNode : Node
    {
        #region Fields

        private readonly SceneTree _owner;
        private BoundingBox _boundingBox;
        private readonly Dictionary<string, IMovable> _movablesByName = new Dictionary<string, IMovable>();
        private readonly List<IMovable> _movablesList = new List<IMovable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of SceneNode class
        /// </summary>
        /// <param name="scene">SceneGraph which contains this node</param>
        internal SceneNode(SceneTree scene)
            : base(string.Empty)
        {
            _owner = scene;
            NeedUpdate(false);
        }

        /// <summary>
        /// Constructor of the SceneNode class
        /// </summary>
        /// <param name="scene">SceneGraph which contains this node</param>
        /// <param name="name">Name of this node</param>
        internal SceneNode(SceneTree scene, string name) 
            : base(name)
        {
            _owner = scene;
            NeedUpdate(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attach a movable object to this scene node
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

            NeedUpdate(false);
        }

        /// <summary>
        /// Detach a movable from this scene node
        /// </summary>
        /// <param name="name">Name of the movable object</param>
        public void DetachObject(string name)
        {
            if (!_movablesByName.ContainsKey(name)) return;

            IMovable movObj = _movablesByName[name];
            movObj.DetachParent();
            _movablesByName.Remove(name);
            _movablesList.Remove(movObj);

            NeedUpdate(false);
        }

        /// <summary>
        /// Create a child node by calling create node methods of the scene graph
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Return a new child node</returns>
        protected override Node CreateChildIntern(string name)
        {
            return _owner.CreateNode(name);
        }

        /// <summary>
        /// Remove a child by calling remove node methods of the scene graph
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool RemoveChildIntern(string name)
        {
            return _owner.RemoveNode(name);
        }

        /// <summary>
        /// Create a child node
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Return a new child node</returns>
        public SceneNode CreateChildSceneNode(string name)
        {
            return (SceneNode)CreateChild(name);
        }

        /// <summary>
        /// Find all visible objects attached to this scene node
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <param name="queue">Current render queue</param>
        /// <param name="addChildren">Boolean indicating if the search goes through childrens</param>
        internal void FindVisibleObjects(Camera cam, RenderQueue queue, bool addChildren)
        {
            for (int i = 0; i < _movablesList.Count; i++)
            {
                queue.ProcessVisibleObject(cam, _movablesList[i]);
            }

            if (addChildren)
            {
                for (int j = 0; j < ChildrensList.Count; j++)
                {
                    ((SceneNode)ChildrensList[j]).FindVisibleObjects(cam, queue, true);
                }
            }
        }

        /// <summary>
        /// Update this node and all his child
        /// </summary>
        /// <param name="updateChild">Boolean indicating if child has to be updated</param>
        /// <param name="parentHasChanged">Boolean indicating if the parent has been updated</param>
        protected internal override void Update(bool updateChild, bool parentHasChanged)
        {
            base.Update(updateChild, parentHasChanged);
            UpdateBounds();
        }

        /// <summary>
        /// Update the bounding volumes
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

            for (int i = 0; i < ChildrensList.Count; i++)
            {
                SceneNode node = (SceneNode)ChildrensList[i];
                BoundingBox.CreateMerged(ref _boundingBox, ref node._boundingBox, out _boundingBox);
            }
        }

        #endregion
    }
}
