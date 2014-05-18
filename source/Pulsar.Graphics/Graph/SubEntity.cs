using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Graph
{
    /// <summary>
    /// Represents a part of an Entity
    /// </summary>
    public sealed class SubEntity : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private Entity _parent;
        private SubMesh _submeshMesh;
        private byte _group;
        private List<SubEntityMaterial> _materials = new List<SubEntityMaterial>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the SubEntity class
        /// </summary>
        /// <param name="parent">Parent Entity</param>
        /// <param name="submesh">ModelMeshPart associated to this SubEntity</param>
        internal SubEntity(SubMesh submesh, Entity parent)
        {
            _parent = parent;
            _submeshMesh = submesh;

            SubMeshMaterialCollection materials = submesh.Materials;
            for (int i = 0; i < materials.Count; i++)
            {
                SubEntityMaterial material = new SubEntityMaterial(materials[i], this);
                _materials.Add(material);
            }
        }

        #endregion

        #region Operators

        public SubEntityMaterial this[string material]
        {
            get { return GetMaterial(material); }
        }

        public SubEntityMaterial this[int index]
        {
            get { return _materials[index]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
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
                _parent = null;
                _submeshMesh = null;
                _materials = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Computes the squared distance between this sub-entity and a camera
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <returns>Returns the squared distance</returns>
        public float GetViewDepth(Camera camera)
        {
            return _parent.Parent.GetViewDepth(camera);
        }

        public SubEntityMaterial GetMaterial(string material)
        {
            int index = IndexOf(material);

            return (index > -1) ? _materials[index] : null;
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

        /// <summary>
        /// Gets the name of this SubEntity
        /// </summary>
        public string Name
        {
            get { return _submeshMesh.Name; }
        }

        /// <summary>
        /// Gets the full transform matrix
        /// </summary>
        public Matrix Transform
        {
            get
            {
                Matrix transform = _parent.Transform;
                if (_parent.BonesCount > 0)
                {
                    Matrix bone = _parent.BonesTransform[_submeshMesh.BoneIndex];
                    Matrix.Multiply(ref bone, ref transform, out transform);

                    return transform;
                }

                return transform;
            }
        }

        /// <summary>
        /// Gets or sets the render queue group
        /// </summary>
        public byte RenderQueueGroup
        {
            get { return _group; }
            set
            {
                _group = value;
                for (int i = 0; i < _materials.Count; i++)
                    _materials[i].RenderQueueGroup = value;
            }
        }

        public int Count
        {
            get { return _materials.Count; }
        }

        internal List<SubEntityMaterial> Materials
        {
            get { return _materials; }
        }

        #endregion
    }
}