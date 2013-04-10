using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Graphics;
using Pulsar.Graphics.Graph;
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
        
        protected BoundingData bounds;

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

    /// <summary>
    /// Class representing a 3D mesh
    /// </summary>
    public sealed class Mesh : Asset
    {
        #region Fields

        private List<SubMesh> subMeshes = new List<SubMesh>();
        private BoundingData bounds;
        private VertexBuffer vBuffer;
        private IndexBuffer iBuffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the mesh class
        /// </summary>
        /// <param name="name"></param>
        internal Mesh(string name) : base(name) 
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create and add a sub mesh to this mesh instance
        /// </summary>
        /// <param name="renderInfo">Rendering information of the submesh</param>
        /// <param name="bounds">Bounding volume data of the submesh</param>
        public void AddSubMesh(RenderingInfo renderInfo, BoundingData bounds)
        {
            if (renderInfo.VBuffer != this.vBuffer)
            {
                throw new Exception("Failed to add new submesh, buffer are different from mesh buffer's");
            }
            if (renderInfo.IBuffer != this.iBuffer)
            {
                throw new Exception("Failed to add new submesh, buffer are different from mesh buffer's");
            }

            SubMesh sub = new SubMesh();
            sub.RenderInfo = renderInfo;
            sub.BoundingVolume = bounds;
            sub.ID = SubMesh.GetID();

            BoundingBox.CreateMerged(ref this.bounds.BoundingBox, ref bounds.BoundingBox,
                out this.bounds.BoundingBox);
            BoundingSphere.CreateMerged(ref this.bounds.BoundingSphere, ref bounds.BoundingSphere,
                out this.bounds.BoundingSphere);
            this.VerticesCount += renderInfo.VertexCount;
            this.PrimitiveCount += renderInfo.TriangleCount;
            this.subMeshes.Add(sub);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the total of vertices for this mesh
        /// </summary>
        public int VerticesCount { get; internal set; }

        public int PrimitiveCount { get; internal set; }

        /// <summary>
        /// Get the list of sub mesh
        /// </summary>
        internal List<SubMesh> SubMeshes
        {
            get { return this.subMeshes; }
        }

        /// <summary>
        /// Get or set the bounding volume data of the mesh
        /// </summary>
        internal BoundingData BoundingVolume
        {
            get { return this.bounds; }
            set { this.bounds = value; }
        }

        /// <summary>
        /// Get the aabb of the mesh
        /// </summary>
        public BoundingBox AxisAlignedBoundingBox
        {
            get { return this.bounds.BoundingBox; }
        }

        /// <summary>
        /// Get the bounding sphere of the mesh
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return this.bounds.BoundingSphere; }
        }

        /// <summary>
        /// Get the bones of the sub mesh
        /// </summary>
        public Matrix[] Bones { get; internal set; }

        /// <summary>
        /// Get the vertex buffer of the mesh
        /// </summary>
        public VertexBuffer VBuffer
        {
            get { return this.VBuffer; }
            internal set { this.vBuffer = value; }
        }

        /// <summary>
        /// Get the index buffer of the mesh
        /// </summary>
        public IndexBuffer IBuffer
        {
            get { return this.iBuffer; }
            internal set { this.iBuffer = value; }
        }

        #endregion
    }
}
