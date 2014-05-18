using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a part of a 3D mesh
    /// A SubMesh is the smallest unit of a mesh
    /// </summary>
    public sealed class SubMesh : IDisposable
    {
        #region Fields

        private readonly string _name;
        private SubMeshMaterialCollection _materials;
        private VertexData _vertexData = new VertexData();
        private IndexData _indexData = new IndexData();
        private PrimitiveType _primitiveType;
        private int _vertexCount;
        private int _primitiveCount;
        private bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SubMesh class
        /// </summary>
        internal SubMesh(string name)
        {
            _name = name;
            _materials = new SubMeshMaterialCollection(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) 
                return;

            try
            {
                _materials.Dispose();

                if (!ShareVertexBuffer && (_vertexData.BufferCount > 0))
                    _vertexData.Dispose();
                if (!ShareIndexBuffer && (_indexData.IndexBuffer != null))
                    _indexData.Dispose();
            }
            finally
            {
                _materials = null;
                _vertexData = null;
                _indexData = null;

                _isDisposed = true;
            }
        }

        public void SetVertexBuffer(VertexBufferObject buffer, int vertexCount, int offset)
        {
            if(buffer == null)
                throw new ArgumentNullException("buffer");

            if(_vertexData.BufferCount > 0)
                _vertexData.UnsetBinding(0);

            _vertexCount = vertexCount;
            _vertexData.SetBinding(buffer, offset, 0, 0);

            Update();
        }

        public void SetIndexBuffer(IndexBufferObject buffer, int start, int count)
        {
            _indexData.IndexBuffer = buffer;
            _indexData.StartIndex = start;
            _indexData.IndexCount = count;
            _materials.IndexBuffer = buffer;

            Update();
        }

        internal void Update()
        {
            bool useIndices = _indexData.IndexBuffer != null;
            _primitiveCount = RenderingInfo.ComputePrimitiveCount(_primitiveType, _vertexCount, useIndices,
                _indexData.IndexCount);

            _materials.Update();
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the index used to find the bone attached to this sub mesh
        /// </summary>
        public int BoneIndex { get; internal set; }

        public SubMeshMaterialCollection Materials
        {
            get { return _materials; }
        }

        public int VertexCount
        {
            get { return _vertexCount; }
        }

        public int IndexCount
        {
            get { return _indexData.IndexCount; }
        }

        public int PrimitiveCount
        {
            get { return _primitiveCount; }
        }

        public VertexBufferObject VertexBuffer
        {
            get { return _vertexData.BufferCount > 0 ? _vertexData.GetBuffer(0) : null; }
        }

        public IndexBufferObject IndexBuffer
        {
            get { return _indexData.IndexBuffer; }
        }

        public PrimitiveType PrimitiveType
        {
            get { return _primitiveType; }
            set
            {
                _primitiveType = value;
                _materials.PrimitiveType = value;

                Update();
            }
        }

        /// <summary>
        /// Gets a value that indicates if this submesh share a vertex buffer
        /// </summary>
        public bool ShareVertexBuffer { get; set; }

        /// <summary>
        /// Gets a value that indicates if this submesh share an index buffer
        /// </summary>
        public bool ShareIndexBuffer { get; set; }

        /// <summary>
        /// Gets the aabb of this sub mesh
        /// </summary>
        public BoundingBox AxisAlignedBoundingBox { get; set; }

        /// <summary>
        /// Gets the bounding sphere of this sub mesh
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        internal int VertexUsed
        {
            get { return (_indexData.IndexBuffer != null) ? (_indexData.IndexCount) : _vertexCount; }
        }

        internal VertexData VertexData
        {
            get { return _vertexData; }
        }

        internal IndexData IndexData
        {
            get { return _indexData; }
        }

        #endregion
    }
}
