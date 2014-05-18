using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class SubMeshMaterialCollection : IDisposable
    {
        #region Fields

        private bool _isDisposed ;
        private List<SubMeshMaterial> _materials = new List<SubMeshMaterial>();
        private SubMesh _parent;

        #endregion

        #region Operators

        public SubMeshMaterial this[string material]
        {
            get { return GetMaterial((material)); }
        }

        public SubMeshMaterial this[int index]
        {
            get { return _materials[index]; }
        }

        #endregion

        #region Constructors

        internal SubMeshMaterialCollection(SubMesh parent)
        {
            Debug.Assert(parent != null);

            _parent = parent;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed)
                return;

            try
            {
                for(int i = 0; i < _materials.Count; i++)
                    _materials[i].Dispose();

                _materials.Clear();
            }
            finally
            {
                _materials = null;
                _parent = null;

                _isDisposed = true;
            }
        }

        public SubMeshMaterial Add(string name, Material material)
        {
            return Add(name, material, _parent.VertexUsed, 0);
        }

        public SubMeshMaterial Add(string name, Material material, int vertexCount)
        {
            return Add(name, material, vertexCount, 0);
        }

        public SubMeshMaterial Add(string name, Material material, int vertexCount, int indicesOffset)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (IndexOf(name) > -1)
                throw new Exception(string.Format("SubMesh already have a material named {0}", name));

            SubMeshMaterial subMaterial = new SubMeshMaterial(name, material, _parent);
            subMaterial.SetGeometryData(vertexCount, indicesOffset);
            _materials.Add(subMaterial);

            return subMaterial;
        }

        public bool Remove(string material)
        {
            int index = IndexOf(material);
            if (index == -1)
                return false;

            SubMeshMaterial subMaterial = _materials[index];
            _materials.RemoveAt(index);
            subMaterial.Dispose();

            return true;
        }

        public SubMeshMaterial GetMaterial(int index)
        {
            return _materials[index];
        }

        public SubMeshMaterial GetMaterial(string material)
        {
            int index = IndexOf(material);

            return (index > -1) ? _materials[index] : null;
        }

        internal void Update()
        {
            for(int i = 0; i < _materials.Count; i++)
                _materials[i].Update();
        }

        private int IndexOf(string material)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                if (string.Equals(material, _materials[i].Name, StringComparison.InvariantCulture))
                    return i;
            }

            return -1;
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _materials.Count; }
        }

        internal PrimitiveType PrimitiveType
        {
            set
            {
                for (int i = 0; i < _materials.Count; i++)
                    _materials[i].PrimitiveType = value;
            }
        }

        internal IndexBufferObject IndexBuffer
        {
            set
            {
                for (int i = 0; i < _materials.Count; i++)
                    _materials[i].IndexBuffer = value;
            }
        }

        #endregion
    }
}
