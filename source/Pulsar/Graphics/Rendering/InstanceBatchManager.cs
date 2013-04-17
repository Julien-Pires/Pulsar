using System;

using System.Collections.Generic;

using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Manager for the geometry batch
    /// </summary>
    internal sealed class InstanceBatchManager
    {
        #region Fields

        private Renderer owner;
        private Dictionary<uint, InstanceBatch>[] batchByQueue = new Dictionary<uint, InstanceBatch>[(int)RenderQueueGroupID.Count];
        private List<InstanceBatch> batchs = new List<InstanceBatch>();

        #endregion

        #region Constructors

        internal InstanceBatchManager(Renderer owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all batchs
        /// </summary>
        internal void Reset()
        {
            for (int i = 0; i < batchs.Count; i++)
            {
                this.batchs[i].Reset();
            }
        }

        internal void AddDrawable(IRenderable renderable)
        {
            InstanceBatch batch = this.GetInstanceBatch(renderable.BatchID, renderable.RenderQueueID);
            batch.AddDrawable(renderable);
        }

        internal IEnumerable<InstanceBatch> GetBatchList(int queueId)
        {
            Dictionary<uint, InstanceBatch> map = this.batchByQueue[queueId];
            if (map == null)
            {
                map = this.AddNewMap(queueId);
            }

            return map.Values;
        }

        private InstanceBatch GetInstanceBatch(uint id, int queueId)
        {
            Dictionary<uint, InstanceBatch> map = this.batchByQueue[queueId];
            if (map == null)
            {
                map = this.AddNewMap(queueId);
            }

            InstanceBatch batch;
            map.TryGetValue(id, out batch);
            if (batch == null)
            {
                batch = new InstanceBatch(this.owner.GraphicsDevice, id, queueId);
                map.Add(id, batch);
                this.batchs.Add(batch);
            }

            return batch;
        }

        private Dictionary<uint, InstanceBatch> AddNewMap(int queueId)
        {
            Dictionary<uint, InstanceBatch> map = new Dictionary<uint, InstanceBatch>();
            this.batchByQueue[queueId] = map;

            return map;
        }

        #endregion
    }
}
