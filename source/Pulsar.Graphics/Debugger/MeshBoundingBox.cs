using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

using Pulsar.Graphics.Asset;
using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics.Debugger
{
    /// <summary>
    /// Creates a wireframe bounding box used for debugging
    /// </summary>
    internal sealed class MeshBoundingBox : IRenderable, IDisposable
    {
        #region Fields

        private const string AabbMaterial = "MeshBoundingBox_Material";
        private const string AabbTexture = "MeshBoundingBox_Texture";

        private const int IndexCount = 24;
        private const int VertexCount = 8;
        private const int FrontTopLeft = 0;
        private const int FrontTopRight = 1;
        private const int FrontBottomLeft = 2;
        private const int FrontBottomRight = 3;
        private const int BackTopLeft = 4;
        private const int BackTopRight = 5;
        private const int BackBottomLeft = 6;
        private const int BackBottomRight = 7;

        private bool _isDisposed;
        private Storage _storage;
        private Mesh _internalMesh;
        private VertexBufferObject _vbo;
        private Vector3 _min;
        private Vector3 _max;
        private readonly VertexPositionNormalTexture[] _vertices = new VertexPositionNormalTexture[VertexCount];
        private readonly RenderQueueKey _key = new RenderQueueKey();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MeshBoundingBox class
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="assetEngine">Asset engine</param>
        internal MeshBoundingBox(string name, AssetEngine assetEngine)
        {
            Debug.Assert(assetEngine != null);

            Name = name;
            _storage = assetEngine[GraphicsConstant.Storage];

            LoadAsset();
            InitializeMesh();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Generates indices for an index buffer
        /// </summary>
        /// <param name="ibo">Index buffer instance</param>
        private static void GenerateIndices(IndexBufferObject ibo)
        {
            int[] indices = new int[IndexCount];

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
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                _storage[GraphicsConstant.MeshFolderName].Unload(Name);
            }
            finally
            {
                _vbo = null;
                _internalMesh = null;
                _storage = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Loads asset
        /// </summary>
        private void LoadAsset()
        {
            _internalMesh = _storage[GraphicsConstant.MeshFolderName].Load<Mesh>(Name, MeshParameters.NewInstance);
            if (!_storage[GraphicsConstant.MaterialFolderName].IsLoaded(AabbMaterial))
            {
                TextureParameters textureParameters = new TextureParameters
                {
                    Height = 1,
                    Width = 1
                };
                Texture texture = _storage[GraphicsConstant.TextureFolderName].Load<Texture>(AabbTexture, textureParameters);
                texture.SetData(new[]{ new Color(255, 255, 255, 255) });
                Material = _storage[GraphicsConstant.MaterialFolderName].Load<Material>(AabbMaterial,
                    MaterialParameters.NewInstance);
                Material.DiffuseMap = texture;
            }
            else
                Material = _storage[GraphicsConstant.MaterialFolderName].Load<Material>(AabbMaterial);

            _key.Transparency = Material.IsTransparent;
            _key.Material = Material.Id;
            _key.Group = RenderQueueGroup;     
        }

        /// <summary>
        /// Initializes mesh
        /// </summary>
        private void InitializeMesh()
        {
            _internalMesh.Begin(PrimitiveType.LineList, true, false, true);
            _internalMesh.EstimateBufferSize(VertexCount, IndexCount);
            _internalMesh.Position(0.0f, 0.0f, 0.0f);
            _internalMesh.Index(0);
            _internalMesh.End();

            _vbo = _internalMesh.VertexData.GetBuffer(0);
            GenerateIndices(_internalMesh.IndexData.IndexBuffer);

            SubMesh subMesh = _internalMesh.GetSubMesh(0);
            subMesh.RenderInfo.VertexCount = VertexCount;
            subMesh.IndexData.IndexCount = IndexCount;
            subMesh.RenderInfo.ComputePrimitiveCount();

            _internalMesh.UpdateMeshInfo();
        }

        /// <summary>
        /// Updates the buffers
        /// </summary>
        /// <param name="box">Boundig box used to update buffers data</param>
        internal void UpdateBox(AxisAlignedBox box)
        {
            _min = box.Minimum;
            _max = box.Maximum;
            Vector3 xOffset = new Vector3((_max.X - _min.X), 0, 0);
            Vector3 zOffset = new Vector3(0, 0, (_max.Z - _min.Z));

            Vector3 minOpposite;
            Vector3 maxOpposite;
            Vector3.Add(ref xOffset, ref zOffset, out minOpposite);
            Vector3.Subtract(ref _max, ref minOpposite, out maxOpposite);
            Vector3.Add(ref _min, ref minOpposite, out minOpposite);

            // Front vertices
            _vertices[FrontTopRight].Position = _max;
            _vertices[FrontBottomRight].Position = minOpposite;
            Vector3.Subtract(ref _max, ref xOffset, out _vertices[FrontTopLeft].Position);
            Vector3.Subtract(ref minOpposite, ref xOffset, out _vertices[FrontBottomLeft].Position);

            // Bottom vertices
            _vertices[BackBottomLeft].Position = _min;
            _vertices[BackTopLeft].Position = maxOpposite;
            Vector3.Add(ref _min, ref xOffset, out _vertices[BackBottomRight].Position);
            Vector3.Add(ref maxOpposite, ref xOffset, out _vertices[BackTopRight].Position);

            _vbo.SetData(_vertices);
        }

        /// <summary>
        /// Computes the squared distance between the bounding box and a camera
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <returns>Returns the squared distance</returns>
        public float GetViewDepth(Camera camera)
        {
            Vector3 middle, distance;
            Vector3.Subtract(ref _max, ref _max, out middle);
            Vector3.Multiply(ref middle, 0.5f, out middle);
            Vector3.Add(ref middle, ref _min, out middle);

            Vector3.Subtract(ref camera.WorldTransformPosition, ref middle, out distance);

            return distance.LengthSquared();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this instace
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a boolean to enable instancing
        /// </summary>
        public bool UseInstancing 
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the batch ID of this instance
        /// </summary>
        public uint Id 
        {
            get { return RenderInfo.Id; } 
        }

        /// <summary>
        /// Gets the key used by the render queue
        /// </summary>
        public RenderQueueKey Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the render qeue ID of this instance
        /// </summary>
        public byte RenderQueueGroup 
        {
            get { return (byte)Graphics.RenderQueueGroup.Default; } 
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
        public RenderingInfo RenderInfo
        {
            get { return _internalMesh.GetSubMesh(0).RenderInfo; }
        }

        /// <summary>
        /// Gets the material associated to this instance
        /// </summary>
        public Material Material { get; private set; }

        #endregion
    }
}
