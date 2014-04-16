using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Class representing a part of an Entity
    /// A SubEntity is attached to a ModelMeshPart instance
    /// </summary>
    public sealed class SubEntity : IRenderable, IDisposable
    {
        #region Fields

        private const int RenderQueueKeyCount = 2;

        private bool _isDisposed;
        private readonly string _name = string.Empty;
        private Entity _parent;
        private SubMesh _subMesh;
        private Material _material;
        private byte _group;
        private readonly List<RenderQueueKey> _renderQueueKeys = new List<RenderQueueKey>(RenderQueueKeyCount);
        private readonly ReadOnlyCollection<RenderQueueKey> _readRenderQueueKeys;

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
            _readRenderQueueKeys = new ReadOnlyCollection<RenderQueueKey>(_renderQueueKeys);

            _name = name;
            _parent = parent;
            _subMesh = sub;
            Material = sub.Material;
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
                _subMesh = null;
                _material = null;

                _isDisposed = true;
            }
        }

        public float GetViewDepth(Camera camera)
        {
            return _parent.Parent.GetViewDepth(camera);
        }

        private void OnMaterialTechniqueChanged(Material material, TechniqueDefinition definition)
        {
            CreateKeys(material.PassCount, _renderQueueKeys.Count);
            UpdateKeysMaterialPart();
        }

        private void CreateKeys(int passesCount, int oldPassesCount)
        {
            int diff = passesCount - oldPassesCount;
            if (diff > 0)
            {
                for(int i = 0; i < diff; i++)
                    _renderQueueKeys.Add(new RenderQueueKey());
            }
            else
            {
                diff = Math.Abs(diff);
                for (int i = 0; i < diff; i++)
                    _renderQueueKeys.RemoveAt(_renderQueueKeys.Count - 1);
            }
        }

        private void UpdateKeysMaterialPart()
        {
            PassDefinition[] definitions = _material.Technique.Passes;
            for (int i = 0; i < definitions.Length; i++)
            {
                RenderQueueKey key = _renderQueueKeys[i];
                key.Material = _material.Id;
                key.Pass = definitions[i].Id;
                key.Transparency = _material.IsTransparent;
            }
        }

        private void UpdateKeysGroupPart()
        {
            for (int i = 0; i < _renderQueueKeys.Count; i++)
                _renderQueueKeys[i].Group = _group;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this sub entity, the ID correspond to the ID of the submesh attached to this instance
        /// </summary>
        public uint Id
        {
            get { return _subMesh.RenderInfo.Id; }
        }

        /// <summary>
        /// Get the name of this SubEntity
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get or set a boolean indicating if this instance use instancing
        /// </summary>
        public bool UseInstancing
        {
            get { return _parent.UseInstancing; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Get the rendering info
        /// </summary>
        public RenderingInfo RenderInfo
        {
            get { return _subMesh.RenderInfo; }
        }

        /// <summary>
        /// Get or set the material used by this sub entity
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

                int oldPassCount = (oldMtl == null) ? 0 : oldMtl.PassCount;
                CreateKeys(newMtl.PassCount, oldPassCount);
                UpdateKeysMaterialPart();

                if(oldMtl != null)
                    oldMtl.TechniqueChanged -= OnMaterialTechniqueChanged;

                _material = newMtl;
                _material.TechniqueChanged += OnMaterialTechniqueChanged;
            }
        }

        /// <summary>
        /// Get the full transform matrix
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

        public IList<RenderQueueKey> RenderQueueKeys
        {
            get { return _readRenderQueueKeys; }
        }

        /// <summary>
        /// Get the ID of the render queue used by this SubEntity
        /// </summary>
        public byte RenderQueueGroup
        {
            get { return _group; }
            set
            {
                _group = value;
                UpdateKeysGroupPart();
            }
        }

        #endregion
    }
}