using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Game;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.Debugger
{
    /// <summary>
    /// Creates a wireframe bounding box used for debugging
    /// </summary>
    internal sealed class MeshBoundingBox : IRenderable
    {
        #region Fields

        private const int VerticesCount = 24;
        private const int PrimitiveCount = 12;

        private VertexBufferObject _vbo;
        private IndexBufferObject _ibo;
        private readonly short[] _indices = new short[VerticesCount];
        private readonly VertexPositionNormalTexture[] _vertices = new VertexPositionNormalTexture[VerticesCount];

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MeshBoundingBox class
        /// </summary>
        internal MeshBoundingBox()
        {
            InitBuffers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes all the buffers
        /// </summary>
        private void InitBuffers()
        {
            GraphicsEngineService engineService = GameApplication.GameServices.GetService(typeof(IGraphicsEngineService)) 
                as GraphicsEngineService;
            if (engineService == null) 
                throw new NullReferenceException("GraphicsEngineService cannot be found");

            BufferManager bufferMngr = engineService.Engine.BufferManager;
            VertexData vData = new VertexData();
            _vbo = bufferMngr.CreateVertexBuffer(BufferType.DynamicWriteOnly, typeof(VertexPositionNormalTexture),
                VerticesCount);
            vData.SetBinding(_vbo);

            IndexData iData = new IndexData();
            _ibo = bufferMngr.CreateIndexBuffer(BufferType.DynamicWriteOnly, IndexElementSize.SixteenBits,
                VerticesCount);
            _ibo.SetData(_indices);
            iData.IndexBuffer = _ibo;

            for (short i = 0; i < VerticesCount; i++) _indices[i] = i;
            
            RenderInfo = new RenderingInfo
            {
                PrimitiveType = PrimitiveType.LineList,
                VertexData = vData,
                IndexData = iData,
                VertexCount = VerticesCount,
                PrimitiveCount = PrimitiveCount
            };
            Material = MaterialManager.Instance.LoadDefault();
            Material.DiffuseColor = Color.White;
        }

        /// <summary>
        /// Updates the buffers
        /// </summary>
        /// <param name="box">Boundig box used to update buffers data</param>
        internal void UpdateBox(ref BoundingBox box)
        {
            Vector3 xOffset = new Vector3((box.Max.X - box.Min.X), 0, 0);
            Vector3 yOffset = new Vector3(0, (box.Max.Y - box.Min.Y), 0);
            Vector3 zOffset = new Vector3(0, 0, (box.Max.Z - box.Min.Z));
            Vector3 minOpposite = box.Min + (xOffset + zOffset);
            Vector3 maxOpposite = box.Max - (xOffset + zOffset);

            //// Top
            _vertices[0].Position = box.Min;
            _vertices[1].Position = box.Min + xOffset;
            _vertices[2].Position = box.Min;
            _vertices[3].Position = box.Min + zOffset;
            _vertices[4].Position = minOpposite;
            _vertices[5].Position = box.Min + xOffset;
            _vertices[6].Position = minOpposite;
            _vertices[7].Position = box.Min + zOffset;

            //// Bottom
            _vertices[8].Position = box.Max;
            _vertices[9].Position = box.Max - xOffset;
            _vertices[10].Position = box.Max;
            _vertices[11].Position = box.Max - zOffset;
            _vertices[12].Position = maxOpposite;
            _vertices[13].Position = box.Max - xOffset;
            _vertices[14].Position = maxOpposite;
            _vertices[15].Position = box.Max - zOffset;

            //// Sides
            _vertices[16].Position = box.Min;
            _vertices[17].Position = box.Min + yOffset;
            _vertices[18].Position = box.Min + xOffset;
            _vertices[19].Position = (box.Min + xOffset) + yOffset;
            _vertices[20].Position = box.Max;
            _vertices[21].Position = box.Max - yOffset;
            _vertices[22].Position = box.Max - xOffset;
            _vertices[23].Position = (box.Max - xOffset) - yOffset;

            _vbo.SetData(_vertices);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this instace
        /// </summary>
        public string Name 
        {
            get { return "Debug AABB"; } 
        }

        /// <summary>
        /// Gets or set a boolean to enable instancing
        /// </summary>
        public bool UseInstancing 
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the batch ID of this instance
        /// </summary>
        public uint BatchId 
        {
            get { return RenderInfo.Id; } 
        }

        /// <summary>
        /// Gets the render qeue ID of this instance
        /// </summary>
        public int RenderQueueId 
        {
            get { return (int)RenderQueueGroupId.Default; } 
        }

        /// <summary>
        /// Gets the transform matrix of this instance
        /// </summary>
        public Matrix Transform 
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Gets the rendering info of this instance
        /// </summary>
        public RenderingInfo RenderInfo { get; private set; }

        /// <summary>
        /// Gets the material associated to this instance
        /// </summary>
        public Material Material { get; private set; }

        #endregion
    }
}
