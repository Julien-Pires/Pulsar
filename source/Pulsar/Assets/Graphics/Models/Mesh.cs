using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;
using Pulsar.Assets.Graphics.Materials;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Class representing a 3D mesh
    /// </summary>
    public sealed class Mesh : Asset
    {
        #region Fields

        private List<SubMesh> subMeshes = new List<SubMesh>();
        private Dictionary<string, ushort> subMeshNamesMap = new Dictionary<string, ushort>();
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

        public SubMesh CreateSubMesh()
        {
            SubMesh sub = new SubMesh(this);
            this.subMeshes.Add(sub);

            return sub;
        }

        public SubMesh CreateSubMesh(string name)
        {
            if (this.subMeshNamesMap.ContainsKey(name))
            {
                throw new Exception(string.Format("A submesh with the name {0} already exists", name));
            }
            SubMesh sub = this.CreateSubMesh();
            this.subMeshNamesMap.Add(name, (ushort)(this.subMeshes.Count - 1));

            return sub;
        }

        public void ApplyChanges()
        {
            for (int i = 0; i < this.subMeshes.Count; i++)
            {
                RenderingInfo renderData = this.subMeshes[i].RenderInfo;
                renderData.UseIndexes = this.UseIndexes;
                renderData.IBuffer = this.iBuffer;
            }
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

        public bool UseIndexes { get; set; }

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
