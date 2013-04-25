using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Materials;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Instance batch for instancing one mesh
    /// </summary>
    internal sealed class InstanceBatch
    {
        #region Fields

        private static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
        );

        private uint id;
        private int renderQueueId;
        private bool renderInfoInit = false;
        private List<IRenderable> instances = new List<IRenderable>();
        private RenderingInfo renderInfo = null;
        private Material material = null;
        private Matrix[] transforms;
        private DynamicVertexBuffer transformsBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of InstanceBatch class
        /// </summary>
        /// <param name="gDevice">Graphic device</param>
        /// <param name="id">Id of the batch</param>
        /// <param name="queueId">Queue Id of the batch</param>
        internal InstanceBatch(GraphicsDevice gDevice, uint id, int queueId)
        {
            this.transformsBuffer = new DynamicVertexBuffer(gDevice, instanceVertexDeclaration, 0, BufferUsage.WriteOnly);
            this.id = id;
            this.renderQueueId = queueId;
        }

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
            if (!this.renderInfoInit)
            {
                IRenderable renderable = this.instances[0];

                this.material = renderable.Material;
                this.renderInfo = renderable.RenderInfo;
                this.renderInfoInit = true;
            }
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
            this.transformsBuffer.SetData<Matrix>(this.transforms, 0, this.instances.Count, SetDataOptions.Discard);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this geometry batch
        /// </summary>
        public uint ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Get the vertex buffer
        /// </summary>
        internal DynamicVertexBuffer Buffer
        {
            get
            {
                this.UpdateTransforms();

                return this.transformsBuffer;
            }
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
        /// Get the ID of the render queue used by this batch
        /// </summary>
        public int RenderQueueID 
        {
            get { return this.renderQueueId; }
        }

        /// <summary>
        /// Get the rendering info of this batch
        /// </summary>
        public RenderingInfo RenderInfo 
        {
            get 
            {
                this.ExtractRenderingInfo();

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
                this.ExtractRenderingInfo();

                return this.material;
            }
        }

        #endregion
    }
}
