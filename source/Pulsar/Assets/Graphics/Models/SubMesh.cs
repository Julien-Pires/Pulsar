using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Graphics;
using Pulsar.Graphics.Rendering;
using Pulsar.Assets.Graphics.Materials;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Class representing a part of a 3D mesh
    /// A SubMesh is the smallest unit of a mesh, one SubMesh means one draw call
    /// </summary>
    public sealed class SubMesh
    {
        #region Fields

        private static uint uniqueID = uint.MinValue;

        private uint id;
        private BoundingData bounds;

        #endregion

        #region Constructors

        internal SubMesh()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a unique ID for a SubMesh
        /// </summary>
        /// <returns>Return an uint wich represents an ID</returns>
        internal static uint GetID()
        {
            SubMesh.uniqueID++;

            return SubMesh.uniqueID;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this instance
        /// </summary>
        public uint ID
        {
            get { return this.id; }

            internal set
            {
                this.id = value;
                if (this.RenderInfo != null)
                {
                    this.RenderInfo.ID = value;
                }
            }
        }

        /// <summary>
        /// Get the index to find the bone attached to this sub mesh
        /// </summary>
        public int BoneIndex { get; internal set; }

        /// <summary>
        /// Get the name of this sub mesh
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Get the information to render this sub mesh
        /// </summary>
        public RenderingInfo RenderInfo { get; internal set; }

        /// <summary>
        /// Get the material attached to this sub mesh
        /// </summary>
        public Material Material { get; internal set; }

        /// <summary>
        /// Get or set the bounding volume data
        /// </summary>
        internal BoundingData BoundingVolume
        {
            get { return this.bounds; }
            set { this.bounds = value; }
        }

        /// <summary>
        /// Get the aabb of this sub mesh
        /// </summary>
        public BoundingBox AxisAlignedBoundingBox
        {
            get { return this.bounds.BoundingBox; }
        }

        /// <summary>
        /// Get the bounding sphere of this sub mesh
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return this.bounds.BoundingSphere; }
        }

        #endregion
    }
}
