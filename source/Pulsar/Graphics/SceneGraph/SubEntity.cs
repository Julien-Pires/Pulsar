using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Models;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Class representing a part of an Entity
    /// A SubEntity is attached to a ModelMeshPart instance
    /// </summary>
    public sealed class SubEntity : IRenderable
    {
        #region Fields

        private int queueID;
        private Entity parent = null;
        private SubMesh subMesh = null;
        private Material material = null;
        private string name = string.Empty;

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
            this.name = name;
            this.parent = parent;
            this.subMesh = sub;
            this.material = sub.Material;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this sub entity, the ID correspond to the ID of the submesh attached to this instance
        /// </summary>
        public uint BatchID
        {
            get { return this.subMesh.RenderInfo.id; }
        }

        /// <summary>
        /// Get the name of this SubEntity
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Get or set a boolean indicating if this instance use instancing
        /// </summary>
        public bool UseInstancing
        {
            get { return this.parent.UseInstancing; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the rendering info
        /// </summary>
        public RenderingInfo RenderInfo
        {
            get { return this.subMesh.RenderInfo; }
        }

        /// <summary>
        /// Get or set the material used by this sub entity
        /// </summary>
        public Material Material
        {
            get { return this.material; }
            set
            {
                if (value == null)
                {
                    this.material = this.subMesh.Material;
                }
                else
                {
                    this.material = value;
                }
            }
        }

        /// <summary>
        /// Get the full transform matrix
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (this.parent.bonesCount > 0)
                {
                    return this.parent.bonesTransform[this.subMesh.BoneIndex] * this.parent.Transform;
                }
                else
                {
                    return this.parent.Transform;
                }
            }
        }

        /// <summary>
        /// Get the ID of the render queue used by this SubEntity
        /// </summary>
        public int RenderQueueID
        {
            get { return this.queueID; }
        }

        #endregion
    }
}