using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsar.Core;
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

        private const int IndexCount = 24;
        private const int PrimitiveCount = 12;
        private const int VertexCount = 8;
        private const short FrontTopLeft = 0;
        private const short FrontTopRight = 1;
        private const short FrontBottomLeft = 2;
        private const short FrontBottomRight = 3;
        private const short BackTopLeft = 4;
        private const short BackTopRight = 5;
        private const short BackBottomLeft = 6;
        private const short BackBottomRight = 7;

        private VertexBufferObject _vbo;
        private IndexBufferObject _ibo;
        private readonly VertexPositionNormalTexture[] _vertices = new VertexPositionNormalTexture[VertexCount];

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

        #region Static methods

        private static void GenerateIndices(IndexBufferObject ibo)
        {
            short[] indices = new short[IndexCount];

            // Top
            indices[0] = FrontTopLeft;
            indices[1] = FrontTopRight;
            indices[2] = FrontTopLeft;
            indices[3] = BackTopLeft;
            indices[4] = BackTopRight;
            indices[5] = FrontTopRight;
            indices[6] = BackTopRight;
            indices[7] = BackTopLeft;

            // Bottom
            indices[8] = FrontBottomLeft;
            indices[9] = FrontBottomRight;
            indices[10] = FrontBottomLeft;
            indices[11] = BackBottomLeft;
            indices[12] = BackBottomRight;
            indices[13] = FrontBottomRight;
            indices[14] = BackBottomRight;
            indices[15] = BackBottomLeft;

            //Sides
            indices[16] = FrontTopRight;
            indices[17] = FrontBottomRight;
            indices[18] = FrontTopLeft;
            indices[19] = FrontBottomLeft;
            indices[20] = BackTopRight;
            indices[21] = BackBottomRight;
            indices[22] = BackTopLeft;
            indices[23] = BackBottomLeft;

            ibo.SetData(indices);
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
                VertexCount);
            vData.SetBinding(_vbo);

            IndexData iData = new IndexData();
            _ibo = bufferMngr.CreateIndexBuffer(BufferType.DynamicWriteOnly, IndexElementSize.SixteenBits,
                IndexCount);
            GenerateIndices(_ibo);
            iData.IndexBuffer = _ibo;
            
            RenderInfo = new RenderingInfo
            {
                PrimitiveType = PrimitiveType.LineList,
                VertexData = vData,
                IndexData = iData,
                UseIndexes = true,
                VertexCount = VertexCount,
                PrimitiveCount = PrimitiveCount
            };
            Material = MaterialManager.Instance.LoadDefault();
            Material.DiffuseColor = Color.White;
        }

        /// <summary>
        /// Updates the buffers
        /// </summary>
        /// <param name="box">Boundig box used to update buffers data</param>
        internal void UpdateBox(AxisAlignedBox box)
        {
            Vector3 min = box.Minimum;
            Vector3 max = box.Maximum;
            Vector3 xOffset = new Vector3((max.X - min.X), 0, 0);
            Vector3 zOffset = new Vector3(0, 0, (max.Z - min.Z));

            Vector3 minOpposite;
            Vector3 maxOpposite;
            Vector3.Add(ref xOffset, ref zOffset, out minOpposite);
            Vector3.Subtract(ref max, ref minOpposite, out maxOpposite);
            Vector3.Add(ref min, ref minOpposite, out minOpposite);

            // Front vertices
            _vertices[FrontTopRight].Position = max;
            _vertices[FrontBottomRight].Position = minOpposite;
            Vector3.Subtract(ref max, ref xOffset, out _vertices[FrontTopLeft].Position);
            Vector3.Subtract(ref minOpposite, ref xOffset, out _vertices[FrontBottomLeft].Position);

            // Bottom vertices
            _vertices[BackBottomLeft].Position = min;
            _vertices[BackTopLeft].Position = maxOpposite;
            Vector3.Add(ref min, ref xOffset, out _vertices[BackBottomRight].Position);
            Vector3.Add(ref maxOpposite, ref xOffset, out _vertices[BackTopRight].Position);

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
            set { throw new NotSupportedException(); }
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
