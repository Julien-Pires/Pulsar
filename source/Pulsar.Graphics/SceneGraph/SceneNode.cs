using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Represents a node in a scene graph
    /// </summary>
    public class SceneNode : Node
    {
        #region Fields

        protected readonly BaseScene Owner;

        private readonly AxisAlignedBox _aabb = new AxisAlignedBox();
        private readonly Dictionary<string, IMovable> _movablesByName = new Dictionary<string, IMovable>();
        private readonly List<IMovable> _movablesList = new List<IMovable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of SceneNode class
        /// </summary>
        /// <param name="owner">Scene instance wich create this node</param>
        internal SceneNode(BaseScene owner)
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
            if(node == null) 
                throw new InvalidCastException("Must be a SceneNode instance");

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
        /// <param name="camera">Current camera</param>
        /// <param name="queue">Current render queue</param>
        /// <param name="addChildren">Boolean indicating if the search goes through childrens</param>
        internal void FindVisibleObjects(Camera camera, RenderQueue queue, bool addChildren)
        {
            for (int i = 0; i < _movablesList.Count; i++)
            {
                IMovable movable = _movablesList[i];
                movable.FrustumCulling(camera);

                if (movable.IsRendered)
                    movable.UpdateRenderQueue(queue, camera);
            }

            if (addChildren)
            {
                for (int j = 0; j < Childrens.Count; j++)
                    ((SceneNode)Childrens[j]).FindVisibleObjects(camera, queue, true);
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
            _aabb.Reset();
            for (int i = 0; i < _movablesList.Count; i++)
            {
                IMovable obj = _movablesList[i];
                obj.UpdateBounds();

                AxisAlignedBox objAabb = obj.WorldAabb;
                _aabb.Merge(objAabb);
            }

            for (int i = 0; i < Childrens.Count; i++)
            {
                SceneNode node = (SceneNode)Childrens[i];
                _aabb.Merge(node._aabb);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the scene tree which created this node
        /// </summary>
        public BaseScene Scene
        {
            get { return Owner; }
        }

        #endregion
    }
}
