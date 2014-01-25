using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Class representing a part of an Entity
    /// A SubEntity is attached to a ModelMeshPart instance
    /// </summary>
    public sealed class SubEntity : IRenderable
    {
        #region Fields

        private readonly Entity _parent;
        private readonly SubMesh _subMesh;
        private Material _material;
        private readonly string _name = string.Empty;

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
            _material = sub.Material;
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
            set { _material = value ?? _subMesh.Material; }
        }

        /// <summary>
        /// Get the full transform matrix
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (_parent.BonesCount > 0) return _parent.BonesTransform[_subMesh.BoneIndex] * _parent.Transform;

                return _parent.Transform;
            }
        }

        /// <summary>
        /// Get the ID of the render queue used by this SubEntity
        /// </summary>
        public int RenderQueueId { get; private set; }

        #endregion
    }
}