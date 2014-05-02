using System;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.Graph
{
    /// <summary>
    /// Represents a part of an Entity
    /// </summary>
    public sealed class SubEntity : IRenderable, IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private readonly string _name = string.Empty;
        private Entity _parent;
        private SubMesh _subMesh;
        private Material _material;
        private byte _group;
        private readonly RenderQueueKey _key = new RenderQueueKey();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the SubEntity class
        /// </summary>
        /// <param name="name">Name of the sub entity</param>
        /// <param name="parent">Parent Entity</param>
        /// <param name="sub">ModelMeshPart associated to this SubEntity</param>
        internal SubEntity(string name, Entity parent, SubMesh sub)
        {
            _name = name;
            _parent = parent;
            _subMesh = sub;
            Material = sub.Material;
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
                if (_material != null)
                    _material.TechniqueChanged -= OnMaterialTechniqueChanged;
            }
            finally
            {
                _parent = null;
                _subMesh = null;
                _material = null;

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

        /// <summary>
        /// Called when the associated material change its technique
        /// </summary>
        /// <param name="material">Material</param>
        /// <param name="definition">New technique used</param>
        private void OnMaterialTechniqueChanged(Material material, TechniqueDefinition definition)
        {
            UpdateKeysMaterialPart();
        }

        /// <summary>
        /// Updates the render queue key
        /// </summary>
        private void UpdateKeysMaterialPart()
        {
            if (_material == null)
            {
                _key.Material = 0;
                _key.Transparency = false;
            }
            else
            {
                _key.Material = _material.Id;
                _key.Transparency = _material.IsTransparent;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of this sub entity, the ID correspond to the ID of the submesh attached to this instance
        /// </summary>
        public uint Id
        {
            get { return _subMesh.RenderInfo.Id; }
        }

        /// <summary>
        /// Gets the render queue key
        /// </summary>
        public RenderQueueKey Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the name of this SubEntity
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets a boolean indicating if this instance use instancing
        /// </summary>
        public bool UseInstancing
        {
            get { return _parent.UseInstancing; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the rendering info
        /// </summary>
        public RenderingInfo RenderInfo
        {
            get { return _subMesh.RenderInfo; }
        }

        /// <summary>
        /// Gets or sets the material used by this sub entity
        /// </summary>
        public Material Material
        {
            get { return _material; }
            set
            {
                Material oldMtl = _material;
                Material newMtl = value ?? _subMesh.Material;
                if(newMtl == null)
                    throw new ArgumentNullException("value");

                if ((oldMtl == newMtl) && (oldMtl.Technique == newMtl.Technique))
                    return;

                if(oldMtl != null)
                    oldMtl.TechniqueChanged -= OnMaterialTechniqueChanged;

                _material = newMtl;
                _material.TechniqueChanged += OnMaterialTechniqueChanged;

                UpdateKeysMaterialPart();
            }
        }

        /// <summary>
        /// Gets the full transform matrix
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (_parent.BonesCount > 0) 
                    return _parent.BonesTransform[_subMesh.BoneIndex] * _parent.Transform;

                return _parent.Transform;
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
                _key.Group = value;
            }
        }

        #endregion
    }
}