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

        private readonly Renderer _owner;
        private readonly Dictionary<uint, InstanceBatch>[] _batchByQueue = new Dictionary<uint, InstanceBatch>[(int)RenderQueueGroupId.Count];
        private readonly List<InstanceBatch> _batchs = new List<InstanceBatch>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InstanceBatchManager class
        /// </summary>
        /// <param name="owner"></param>
        internal InstanceBatchManager(Renderer owner)
        {
            _owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all batchs
        /// </summary>
        internal void Reset()
        {
            for (int i = 0; i < _batchs.Count; i++)
            {
                _batchs[i].Reset();
            }
        }

        /// <summary>
        /// Add a renderable object to a batch
        /// </summary>
        /// <param name="renderable">Renderable object to add</param>
        internal void AddDrawable(IRenderable renderable)
        {
            InstanceBatch batch = GetInstanceBatch(renderable.BatchId, renderable.RenderQueueId);
            batch.AddDrawable(renderable);
        }

        /// <summary>
        /// Get an enumerator for all batchs
        /// </summary>
        /// <param name="queueId">Queue Id</param>
        /// <returns>Return an enumerator of InstanceBatch</returns>
        internal IEnumerable<InstanceBatch> GetBatchList(int queueId)
        {
            Dictionary<uint, InstanceBatch> map = _batchByQueue[queueId] ?? AddNewMap(queueId);

            return map.Values;
        }

        /// <summary>
        /// Get an InstanceBatch
        /// </summary>
        /// <param name="id">Id of the batch</param>
        /// <param name="queueId">Queue Id</param>
        /// <returns>Return an InstanceBatch instance</returns>
        private InstanceBatch GetInstanceBatch(uint id, int queueId)
        {
            Dictionary<uint, InstanceBatch> map = _batchByQueue[queueId] ?? AddNewMap(queueId);
            InstanceBatch batch;
            map.TryGetValue(id, out batch);
            if (batch != null) return batch;

            batch = new InstanceBatch(_owner.GraphicsDevice, id, queueId);
            map.Add(id, batch);
            _batchs.Add(batch);

            return batch;
        }

        /// <summary>
        /// Add new InstanceBatch map for a specific queue
        /// </summary>
        /// <param name="queueId">Queue Id</param>
        /// <returns>Return the new map</returns>
        private Dictionary<uint, InstanceBatch> AddNewMap(int queueId)
        {
            Dictionary<uint, InstanceBatch> map = new Dictionary<uint, InstanceBatch>();
            _batchByQueue[queueId] = map;

            return map;
        }

        #endregion
    }
}
