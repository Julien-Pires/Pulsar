using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Instance batch for instancing one mesh
    /// </summary>
    internal sealed class InstanceBatch
    {
        #region Fields

        private static readonly VertexDeclaration InstanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
        );

        private readonly uint _id;
        private bool _renderInfoInit;
        private readonly List<IRenderable> _instances = new List<IRenderable>();
        private RenderingInfo _renderInfo;
        private Material _material;
        private Matrix[] _transforms;
        private readonly DynamicVertexBuffer _transformsBuffer;

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
            _transformsBuffer = new DynamicVertexBuffer(gDevice, InstanceVertexDeclaration, 0, BufferUsage.WriteOnly);
            _id = id;
            RenderQueueId = queueId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add an instance of IRenderable to this batch
        /// </summary>
        /// <param name="instance">IRenderable instance</param>
        internal void AddDrawable(IRenderable instance)
        {
            _instances.Add(instance);
        }

        /// <summary>
        /// Reset this batch
        /// </summary>
        internal void Reset()
        {
            _instances.Clear();
            _renderInfoInit = false;
        }

        /// <summary>
        /// Get the rendering info for this batch
        /// </summary>
        private void ExtractRenderingInfo()
        {
            if (_renderInfoInit) return;

            IRenderable renderable = _instances[0];
            _material = renderable.Material;
            _renderInfo = renderable.RenderInfo;
            _renderInfoInit = true;
        }

        /// <summary>
        /// Update the array of instance transform
        /// </summary>
        private void UpdateTransforms()
        {
            if (_transforms == null) _transforms = new Matrix[_instances.Count * 2];

            if (_transforms.Length < _instances.Count) _transforms = new Matrix[_instances.Count * 2];

            for (int i = 0; i < _instances.Count; i++)
            {
                _transforms[i] = _instances[i].Transform;
            }
            _transformsBuffer.SetData(_transforms, 0, _instances.Count, SetDataOptions.Discard);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this geometry batch
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Get the vertex buffer
        /// </summary>
        internal DynamicVertexBuffer Buffer
        {
            get
            {
                UpdateTransforms();

                return _transformsBuffer;
            }
        }

        /// <summary>
        /// Get the array of instance transform
        /// </summary>
        internal Matrix[] InstanceTransforms
        {
            get
            {
                UpdateTransforms();

                return _transforms;
            }
        }

        /// <summary>
        /// Get total of instance for this batch
        /// </summary>
        public int InstanceCount
        {
            get { return _instances.Count; }
        }

        /// <summary>
        /// Get the ID of the render queue used by this batch
        /// </summary>
        public int RenderQueueId { get; private set; }

        /// <summary>
        /// Get the rendering info of this batch
        /// </summary>
        public RenderingInfo RenderInfo 
        {
            get 
            {
                ExtractRenderingInfo();

                return _renderInfo; 
            }
        }

        /// <summary>
        /// Get the material associated to this batch
        /// </summary>
        public Material Material
        {
            get
            {
                ExtractRenderingInfo();

                return _material;
            }
        }

        #endregion
    }
}
