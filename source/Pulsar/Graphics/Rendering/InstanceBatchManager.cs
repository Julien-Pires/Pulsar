using System;

using System.Collections.Generic;

using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    using InstanceBatchMap = System.Collections.Generic.Dictionary<uint, Pulsar.Graphics.Rendering.InstanceBatch>;

    /// <summary>
    /// Manager for the geometry batch
    /// </summary>
    internal sealed class InstanceBatchManager
    {
        #region Fields

        private Dictionary<int, InstanceBatchMap> batchByRenderQueue = new Dictionary<int, InstanceBatchMap>();
        private List<InstanceBatch> batchs = new List<InstanceBatch>();

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

        /// <summary>
        /// Update the render queue with geometry batchs
        /// </summary>
        /// <param name="queue">Render queue to update</param>
        internal void UpdateRenderQueue(RenderQueue queue)
        {
            IEnumerable<LazyBatchInfo> unsortedInstance = queue.LazyInstances;

            foreach (LazyBatchInfo lazy in unsortedInstance)
            {
                InstanceBatch batch = this.GetGeometryBatch(lazy.QueueID, lazy.BatchID);
                List<IRenderable> instances = lazy.LazyInstances;

                for (int j = 0; j < instances.Count; j++)
                {
                    batch.AddDrawable(instances[j]);
                }
            }

            for (int i = 0; i < this.batchs.Count; i++)
            {
                queue.AddRenderable(this.batchs[i]);
            }
        }

        /// <summary>
        /// Get a geometry batch
        /// </summary>
        /// <param name="queueID">Queue id of the batch</param>
        /// <param name="batchID">ID of the batch</param>
        /// <returns>Return a geometry batch instance</returns>
        private InstanceBatch GetGeometryBatch(int queueID, uint batchID)
        {
            if (!this.batchByRenderQueue.ContainsKey(queueID))
            {
                this.batchByRenderQueue.Add(queueID, new InstanceBatchMap());
            }

            InstanceBatchMap map = this.batchByRenderQueue[queueID];
            InstanceBatch batch;
            map.TryGetValue(batchID, out batch);
            if (batch != null)
            {
                return batch;
            }

            batch = new InstanceBatch();
            map.Add(batchID, batch);
            this.batchs.Add(batch);

            return batch;
        }

        #endregion
    }
}
