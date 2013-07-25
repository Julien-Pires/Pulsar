using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Define a Transform node
    /// </summary>
    public class SceneNode : Node
    {
        #region Fields

        private SceneTree owner = null;
        private BoundingBox boundingBox = new BoundingBox();
        private Dictionary<string, IMovable> movablesByName = new Dictionary<string, IMovable>();
        private List<IMovable> movablesList = new List<IMovable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of SceneNode class
        /// </summary>
        /// <param name="scene">SceneGraph which contains this node</param>
        internal SceneNode(SceneTree scene)
            : base(string.Empty)
        {
            this.owner = scene;
            this.NeedUpdate(false);
        }

        /// <summary>
        /// Constructor of the SceneNode class
        /// </summary>
        /// <param name="scene">SceneGraph which contains this node</param>
        /// <param name="name">Name of this node</param>
        internal SceneNode(SceneTree scene, string name) 
            : base(name)
        {
            this.owner = scene;
            this.NeedUpdate(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attach a movable object to this scene node
        /// </summary>
        /// <param name="movObj">IMovable instance</param>
        public void AttachObject(IMovable movObj)
        {
            if (this.movablesByName.ContainsKey(movObj.Name))
            {
                this.movablesByName.Remove(movObj.Name);
                this.movablesList.Remove(movObj);
            }

            this.movablesByName.Add(movObj.Name, movObj);
            this.movablesList.Add(movObj);
            movObj.AttachParent(this);

            this.NeedUpdate(false);
        }

        /// <summary>
        /// Detach a movable from this scene node
        /// </summary>
        /// <param name="name">Name of the movable object</param>
        public void DetachObject(string name)
        {
            if (!this.movablesByName.ContainsKey(name))
                return;

            IMovable movObj = this.movablesByName[name];

            movObj.DetachParent();
            this.movablesByName.Remove(name);
            this.movablesList.Remove(movObj);

            this.NeedUpdate(false);
        }

        /// <summary>
        /// Create a child node by calling create node methods of the scene graph
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Return a new child node</returns>
        protected override Node CreateChildIntern(string name)
        {
            return this.owner.CreateNode(name);
        }

        /// <summary>
        /// Remove a child by calling remove node methods of the scene graph
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override bool RemoveChildIntern(string name)
        {
            return this.owner.RemoveNode(name);
        }

        /// <summary>
        /// Create a child node
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <returns>Return a new child node</returns>
        public SceneNode CreateChildSceneNode(string name)
        {
            return (SceneNode)this.CreateChild(name);
        }

        /// <summary>
        /// Find all visible objects attached to this scene node
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <param name="queue">Current render queue</param>
        /// <param name="addChildren">Boolean indicating if the search goes through childrens</param>
        internal void FindVisibleObjects(Camera cam, RenderQueue queue, bool addChildren)
        {
            for (int i = 0; i < movablesList.Count; i++)
            {
                queue.ProcessVisibleObject(cam, movablesList[i]);
            }

            if (addChildren)
            {
                for (int j = 0; j < this.childrensList.Count; j++)
                {
                    ((SceneNode)this.childrensList[j]).FindVisibleObjects(cam, queue, addChildren);
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
            this.UpdateBounds();
        }

        /// <summary>
        /// Update the bounding volumes
        /// </summary>
        private void UpdateBounds()
        {
            this.boundingBox = new BoundingBox();

            for (int i = 0; i < this.movablesList.Count; i++)
            {
                IMovable obj = this.movablesList[i];
                BoundingBox objBox = obj.WorldBoundingBox;

                BoundingBox.CreateMerged(ref this.boundingBox, ref objBox, out this.boundingBox);
            }

            for (int i = 0; i < this.childrensList.Count; i++)
            {
                SceneNode node = (SceneNode)this.childrensList[i];
                BoundingBox.CreateMerged(ref this.boundingBox, ref node.boundingBox, out this.boundingBox);
            }
        }

        #endregion
    }
}
