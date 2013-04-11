using System;

using System.Collections.Generic;

using Pulsar.Assets.Graphics.Materials;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// List of render queue ID
    /// </summary>
    public enum RenderQueueGroupID
    {
        Background = 0,
        Default = 5,
        Overlay = 10,
        Count = 11
    }

    /// <summary>
    /// Represents a group of instancing object identified by a batch ID and queue ID
    /// </summary>
    internal class LazyBatchInfo
    {
        #region Fields

        public int QueueID;

        public uint BatchID;

        public List<IRenderable> LazyInstances = new List<IRenderable>();

        #endregion

        #region Methods

        /// <summary>
        /// Clear the list of renderable object
        /// </summary>
        internal void Clear()
        {
            this.LazyInstances.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Keep track of all instancing groups
    /// </summary>
    internal sealed class LazyRenderQueue
    {
        #region Fields

        private List<LazyBatchInfo> lazyInstances = new List<LazyBatchInfo>();
        private Dictionary<ulong, int> indexList = new Dictionary<ulong, int>();

        #endregion

        #region Methods

        /// <summary>
        /// Clear all the list of instancing groups
        /// </summary>
        internal void Clear()
        {
            for (int i = 0; i < this.lazyInstances.Count; i++)
            {
                this.lazyInstances[i].Clear();
            }
        }

        /// <summary>
        /// Find an instancing group by its ID and queue ID
        /// </summary>
        /// <param name="queueID">Queue ID of the group</param>
        /// <param name="batchID">ID of the batch</param>
        /// <returns></returns>
        internal LazyBatchInfo GetBatchInfo(int queueID, uint batchID)
        {
            ulong key = this.GetKey(queueID, batchID);
            int idx;
            LazyBatchInfo inf = null;

            if (!this.indexList.ContainsKey(key))
            {
                inf = new LazyBatchInfo();
                inf.QueueID = queueID;
                inf.BatchID = batchID;
                this.lazyInstances.Add(inf);

                idx = this.lazyInstances.Count - 1;
                this.indexList.Add(key, idx);
            }
            else
            {
                idx = this.indexList[key];
            }

            return this.lazyInstances[idx];
        }

        /// <summary>
        /// Compute a unique key for a specific queue and batch
        /// </summary>
        /// <param name="queueID">Queue ID of the group</param>
        /// <param name="batchID">ID of the batch</param>
        /// <returns></returns>
        private ulong GetKey(int queueID, uint batchID)
        {
            ulong key = queueID > batchID ? (uint)batchID | ((ulong)queueID << 32) :
                                            (uint)queueID | ((ulong)batchID << 32);

            return key;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get all instancing group
        /// </summary>
        internal List<LazyBatchInfo> LazyInstances
        {
            get { return this.lazyInstances; }
        }

        #endregion
    }

    /// <summary>
    /// Class describing a group of the render queue
    /// This class do distinction between solids objects and transparents
    /// </summary>
    internal sealed class RenderQueueGroup
    {
        #region Fields

        private int groupID;
        private List<IRenderable> solids = new List<IRenderable>();
        private List<IRenderable> transparents = new List<IRenderable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the RenderQueueGroup class
        /// </summary>
        /// <param name="id">ID of this group</param>
        public RenderQueueGroup(int id)
        {
            this.groupID = id;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear this group
        /// </summary>
        internal void Clear()
        {
            this.solids.Clear();
            this.transparents.Clear();
        }

        /// <summary>
        /// Add a IRenderable to this group
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddRenderable(IRenderable renderable)
        {
            Material material = renderable.Material;

            if (!material.IsTransparent)
            {
                this.solids.Add(renderable);
            }
            else if (material.IsTransparent)
            {
                this.transparents.Add(renderable);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the list of transparent object
        /// </summary>
        public List<IRenderable> TransparentList
        {
            get { return this.transparents; }
        }

        /// <summary>
        /// Get the list of solid object
        /// </summary>
        public List<IRenderable> SolidList
        {
            get { return this.solids; }
        }

        /// <summary>
        /// Get the ID of this group
        /// </summary>
        public int ID
        {
            get { return this.groupID; }
        }

        #endregion
    }

    /// <summary>
    /// Class describing a render queue
    /// </summary>
    public sealed class RenderQueue
    {
        #region Fields

        private RenderQueueGroup[] queueGroups = new RenderQueueGroup[(int)RenderQueueGroupID.Count];
        private LazyRenderQueue lazyQueue = new LazyRenderQueue();

        #endregion

        #region Methods

        /// <summary>
        /// Add a IRenderable using instancing in the render queue
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddInstancedRenderable(IRenderable renderable)
        {
            uint batchID = renderable.BatchID;
            int queueID = renderable.RenderQueueID;
            LazyBatchInfo inf = this.lazyQueue.GetBatchInfo(queueID, batchID);

            inf.LazyInstances.Add(renderable);
        }

        /// <summary>
        /// Add a IRenderable in the render queue
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddRenderable(IRenderable renderable)
        {
            this.AddRenderable(renderable, renderable.RenderQueueID);
        }

        /// <summary>
        /// Add a IRenderable in the render queue at a specific position
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        /// <param name="groupID">Render queue id to attached the IRenderable</param>
        internal void AddRenderable(IRenderable renderable, int groupID)
        {
            RenderQueueGroup group = this.GetGroup(groupID);

            group.AddRenderable(renderable);
        }

        /// <summary>
        /// Clear all render queue group
        /// </summary>
        internal void Clear()
        {
            for (int i = 0; i < this.queueGroups.Length; i++)
            {
                RenderQueueGroup group = this.queueGroups[i];

                if(group != null)
                    group.Clear();
            }

            this.lazyQueue.Clear();
        }

        /// <summary>
        /// Check if an object is visible and add it to the render queue
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <param name="movObj">IMovable instance</param>
        internal void ProcessVisibleObject(Camera cam, IMovable movObj)
        {
            movObj.NotifyCurrentCamera(cam);

            if (movObj.IsRendered)
            {
                movObj.UpdateRenderQueue(this);
            }
        }

        /// <summary>
        /// Get a specific render queue group
        /// </summary>
        /// <param name="id">ID of the render queue group</param>
        /// <returns>Returns an RenderQueueGroup</returns>
        private RenderQueueGroup GetGroup(int id)
        {
            if (id > (int)RenderQueueGroupID.Count)
                throw new Exception();

            RenderQueueGroup group;
            
            if (this.queueGroups[id] != null)
            {
                group = this.queueGroups[id];
            }
            else
            {
                group = new RenderQueueGroup(id);

                this.AddNewRenderGroup(group);
            }

            return group;
        }

        /// <summary>
        /// Create a new RenderQueueGroup
        /// </summary>
        /// <param name="renderGroup"></param>
        private void AddNewRenderGroup(RenderQueueGroup renderGroup)
        {
            this.queueGroups[renderGroup.ID] = renderGroup;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get an array of RenderQueueGroup
        /// </summary>
        internal RenderQueueGroup[] QueueGroupList
        {
            get { return this.queueGroups; }
        }

        internal List<LazyBatchInfo> LazyInstances
        {
            get { return this.lazyQueue.LazyInstances; }
        }

        #endregion
    }
}
