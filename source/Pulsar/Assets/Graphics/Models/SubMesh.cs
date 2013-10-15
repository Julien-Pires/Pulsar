using System;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Rendering;
using Pulsar.Assets.Graphics.Materials;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Represents a part of a 3D mesh
    /// A SubMesh is the smallest unit of a mesh
    /// </summary>
    public sealed class SubMesh : IDisposable
    {
        #region Fields

        private readonly Mesh _parent;
        private readonly RenderingInfo _renderData = new RenderingInfo();
        private readonly VertexData _vertexData = new VertexData();
        private readonly IndexData _indexData = new IndexData();
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SubMesh class
        /// </summary>
        /// <param name="parent">Parent of the submesh</param>
        internal SubMesh(Mesh parent)
        {
            _parent = parent;
            _renderData.VertexData = _vertexData;
            _renderData.IndexData = _indexData;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            if (!ShareVertexBuffer && (_vertexData.BufferCount > 0))
            {
                _vertexData.GetBuffer(0).Dispose();
                _vertexData.UnsetBinding(0);
            }
            if (!ShareIndexBuffer && (_vertexData.BufferCount > 0))
            {
                _indexData.IndexBuffer.Dispose();
                _indexData.IndexBuffer = null;
            }
            _disposed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of this instance
        /// </summary>
        public uint Id
        {
            get { return _renderData.Id; }
        }        

        /// <summary>
        /// Gets the information to render this sub mesh
        /// </summary>
        public RenderingInfo RenderInfo 
        {
            get { return _renderData; }  
        }

        /// <summary>
        /// Gets the index used to find the bone attached to this sub mesh
        /// </summary>
        public int BoneIndex { get; internal set; }

        /// <summary>
        /// Gets the VertexData instance
        /// </summary>
        public VertexData VertexData
        {
            get { return _vertexData; }
        }

        /// <summary>
        /// Gets the IndexData instance
        /// </summary>
        public IndexData IndexData
        {
            get { return _indexData; }
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
        /// Gets or set the material attached to this sub mesh
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Gets the aabb of this sub mesh
        /// </summary>
        public BoundingBox AxisAlignedBoundingBox { get; set; }

        /// <summary>
        /// Gets the bounding sphere of this sub mesh
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        #endregion
    }
}
