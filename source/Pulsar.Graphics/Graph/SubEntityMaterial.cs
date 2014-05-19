using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.Graph
{
    public sealed class SubEntityMaterial : IRenderable, IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private SubMeshMaterial _submeshMaterial;
        private SubEntity _parent;
        private readonly RenderQueueKey _key = new RenderQueueKey();
        private Material _material;

        #endregion

        #region Constructors

        internal SubEntityMaterial(SubMeshMaterial material, SubEntity parent)
        {
            _submeshMaterial = material;
            _parent = parent;
            Material = _submeshMaterial.Material;
        }

        #endregion

        #region Methods

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
                _submeshMaterial = null;

                _isDisposed = true;
            }
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
            Debug.Assert(_material != null);

            _key.Material = _material.Id;
            _key.Transparency = _material.IsTransparent;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _submeshMaterial.Name; }
        }

        public RenderQueueKey Key
        {
            get { return _key; }
        }

        public byte RenderQueueGroup
        {
            get { return _parent.RenderQueueGroup; }
            set
            {
                _key.Group = value;
            }
        }

        public Matrix Transform
        {
            get { return _parent.Transform; }
        }

        public IRenderingInfo RenderInfo
        {
            get { return _submeshMaterial.RenderingInfo; }
        }

        public Material Material
        {
            get { return _material; }
            set
            {
                Material oldMtl = _material;
                Material newMtl = value ?? _submeshMaterial.Material;
                if (newMtl == null)
                    throw new ArgumentNullException("value");

                if ((oldMtl == newMtl) && (oldMtl.Technique == newMtl.Technique))
                    return;

                if (oldMtl != null)
                    oldMtl.TechniqueChanged -= OnMaterialTechniqueChanged;

                _material = newMtl;
                _material.TechniqueChanged += OnMaterialTechniqueChanged;

                UpdateKeysMaterialPart();
            }
        }

        #endregion
    }
}
