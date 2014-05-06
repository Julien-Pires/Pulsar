using System;
using System.Collections.Generic;

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

        private Dictionary<string, SubMeshPart> _parts = new Dictionary<string, SubMeshPart>();
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
        internal SubMesh()
        {
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
                foreach (SubMeshPart part in _parts.Values)
                    part.Dispose();
                _parts.Clear();

                if (!ShareVertexBuffer && (_vertexData.BufferCount > 0))
                    _vertexData.Dispose();
                if (!ShareIndexBuffer && (_indexData.IndexBuffer != null))
                    _indexData.Dispose();
            }
            finally
            {
                _parts = null;
                _vertexData = null;
                _indexData = null;

                _isDisposed = true;
            }
        }

        public SubMeshPart AddPart(string name, Material material)
        {
            return AddPart(name, material, VertexUsed, 0);
        }

        public SubMeshPart AddPart(string name, Material material, int vertexCount)
        {
            return AddPart(name, material, vertexCount, 0);
        }

        public SubMeshPart AddPart(string name, Material material, int vertexCount, int indicesOffset)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if(_parts.ContainsKey(name))
                throw new Exception(string.Format("SubMesh already have a material named {0}", name));

            SubMeshPart part = new SubMeshPart(name, this);
            part.SetGeometryData(vertexCount, indicesOffset);
            _parts.Add(name, part);

            return part;
        }

        public void RemovePart(string name)
        {
            SubMeshPart subMeshPart;
            if(!_parts.TryGetValue(name, out subMeshPart))
                return;

            _parts.Remove(name);
            subMeshPart.Dispose();
        }

        public void SetVertexBuffer(VertexBufferObject buffer, int count, int offset)
        {
            if(buffer == null)
                throw new ArgumentNullException("buffer");

            if(_vertexData.BufferCount > 0)
                _vertexData.UnsetBinding(0);

            _vertexCount = count;
            _vertexData.SetBinding(buffer, offset, 0, 0);

            Update();
        }

        public void SetIndexBuffer(IndexBufferObject buffer, int start, int count)
        {
            _indexData.IndexBuffer = buffer;
            _indexData.StartIndex = start;
            _indexData.IndexCount = count;
            foreach (SubMeshPart part in _parts.Values)
                part.IndexBuffer = buffer;

            Update();
        }

        public void Update()
        {
            bool useIndices = _indexData.IndexBuffer != null;
            _primitiveCount = RenderingInfo.ComputePrimitiveCount(_primitiveType, _vertexCount, useIndices,
                _indexData.IndexCount);

            foreach (SubMeshPart part in _parts.Values)
                part.Update();
        }

        #endregion

        #region Properties      

        /// <summary>
        /// Gets the index used to find the bone attached to this sub mesh
        /// </summary>
        public int BoneIndex { get; internal set; }

        public int VertexCount
        {
            get { return _vertexCount; }
        }

        public int PrimitiveCount
        {
            get { return _primitiveCount; }
        }

        public int IndexCount
        {
            get { return _indexData.IndexCount; }
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
                foreach (SubMeshPart subMeshMaterial in _parts.Values)
                    subMeshMaterial.PrimitiveType = value;
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
