using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class SubMeshMaterial : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private SubMesh _parent;
        private RenderingInfo _renderingInfo = new RenderingInfo();
        private IndexData _indexData;
        private int _indicesOffset;
        private Material _material;

        #endregion

        #region Constructors

        internal SubMeshMaterial(string name, Material material, SubMesh parent)
        {
            Name = name;
            Material = material;
            _parent = parent;

            _renderingInfo.VertexData = parent.VertexData;
            _renderingInfo.IndexData = (IndexData)parent.IndexData.Clone();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed)
                return;

            _parent = null;
            _indexData = null;
            _renderingInfo = null;
            _material = null;

            _isDisposed = true;
        }

        public void SetGeometryData(int vertexCount, int indicesOffset = 0)
        {
            _renderingInfo.VertexCount = vertexCount;
            _indexData.IndexCount = vertexCount;
            _indicesOffset = indicesOffset;

            Update();
        }
        
        internal void Update()
        {
            int startIdx = _parent.IndexData.StartIndex + _indicesOffset;
            _indexData.StartIndex = startIdx;

            _renderingInfo.UpdatePrimitiveCount();
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public Material Material
        {
            get { return _material; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _material = value;
            }
        }

        internal IndexBufferObject IndexBuffer
        {
            get { return _indexData.IndexBuffer; }
            set
            {
                _renderingInfo.UseIndexes = (value != null);
                _indexData.IndexBuffer = value;
            }
        }

        internal PrimitiveType PrimitiveType
        {
            get { return _renderingInfo.PrimitiveType; }
            set { _renderingInfo.PrimitiveType = value; }
        }

        #endregion
    }
}
