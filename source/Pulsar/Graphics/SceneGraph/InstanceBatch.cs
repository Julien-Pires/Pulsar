using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Materials;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    using InstanceBatchMap = System.Collections.Generic.Dictionary<uint, Pulsar.Graphics.SceneGraph.InstanceBatch>;

    /// <summary>
    /// Geometry batch for instancing one mesh
    /// </summary>
    internal sealed class InstanceBatch : IRenderable
    {
        #region Fields

        private uint batchID;
        private int renderGroupID;
        private bool renderInfoInit = false;
        private List<IRenderable> instances = new List<IRenderable>();
        private RenderingInfo renderInfo = null;
        private Material material = null;
        private Matrix[] transforms;

        #endregion

        #region Methods

        /// <summary>
        /// Add an instance of IRenderable to this batch
        /// </summary>
        /// <param name="instance">IRenderable instance</param>
        internal void AddDrawable(IRenderable instance)
        {
            this.instances.Add(instance);
        }

        /// <summary>
        /// Reset this batch
        /// </summary>
        internal void Reset()
        {
            this.instances.Clear();
            this.renderInfoInit = false;
        }

        /// <summary>
        /// Get the rendering info for this batch
        /// </summary>
        private void ExtractRenderingInfo()
        {
            IRenderable renderable = this.instances[0];

            this.material = renderable.Material;
            this.renderInfo = renderable.RenderInfo;
            this.batchID = this.renderInfo.id;
        }

        /// <summary>
        /// Update the array of instance transform
        /// </summary>
        private void UpdateTransforms()
        {
            if (this.transforms == null)
            {
                this.transforms = new Matrix[this.instances.Count * 2];
            }

            if (this.transforms.Length < this.instances.Count)
            {
                this.transforms = new Matrix[this.instances.Count * 2];
            }

            for (int i = 0; i < this.instances.Count; i++)
            {
                this.transforms[i] = this.instances[i].Transform;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this geometry batch
        /// </summary>
        public uint BatchID
        {
            get { return this.batchID; }
        }

        /// <summary>
        /// Get the array of instance transform
        /// </summary>
        internal Matrix[] InstanceTransforms
        {
            get
            {
                this.UpdateTransforms();

                return this.transforms;
            }
        }

        /// <summary>
        /// Get total of instance for this batch
        /// </summary>
        public int InstanceCount
        {
            get { return this.instances.Count; }
        }

        /// <summary>
        /// Get the name of this batch
        /// </summary>
        public string Name 
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Get or set a boolean indicating if this renderable use instancing
        /// </summary>
        public bool UseInstancing 
        {
            get { return true; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the ID of the render queue used by this batch
        /// </summary>
        public int RenderQueueID 
        {
            get { return this.renderGroupID; }
        }

        /// <summary>
        /// Get the local transform of this batch
        /// </summary>
        public Matrix LocalTransform 
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Get the full transform of this batch
        /// </summary>
        public Matrix Transform
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Get the rendering info of this batch
        /// </summary>
        public RenderingInfo RenderInfo 
        {
            get 
            {
                if (!this.renderInfoInit)
                {
                    this.ExtractRenderingInfo();

                    this.renderInfoInit = true;
                }

                return this.renderInfo; 
            }
        }

        /// <summary>
        /// Get the material associated to this batch
        /// </summary>
        public Material Material
        {
            get
            {
                if (!this.renderInfoInit)
                {
                    this.ExtractRenderingInfo();

                    this.renderInfoInit = true;
                }

                return this.material;
            }
        }

        #endregion
    }

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
            List<LazyBatchInfo> unsortedInstance = queue.LazyInstances;

            for (int i = 0; i < unsortedInstance.Count; i++)
            {
                LazyBatchInfo inf = unsortedInstance[i];
                InstanceBatch batch = this.GetGeometryBatch(inf.QueueID, inf.BatchID);
                List<IRenderable> instances = inf.LazyInstances;

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
