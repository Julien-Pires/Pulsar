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

        private bool useIndexes;
        private List<SubMesh> subMeshes = new List<SubMesh>();
        private Dictionary<string, int> subMeshNamesMap = new Dictionary<string, int>();
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

        public void RemoveSubMesh(int index)
        {
            this.subMeshes.RemoveAt(index);
            foreach (KeyValuePair<string, int> mapKvp in this.subMeshNamesMap)
            {
                if (mapKvp.Value == index)
                {
                    this.subMeshNamesMap.Remove(mapKvp.Key);
                }
                else
                {
                    if (mapKvp.Value > index)
                    {
                        this.subMeshNamesMap[mapKvp.Key] = mapKvp.Value + 1;
                    }
                }
            }

            this.ComputeData();
        }

        public void RemoveSubMesh(string name)
        {
            int idx = this.GetSubMeshIndex(name);
            this.RemoveSubMesh(idx);
        }

        public SubMesh GetSubMesh(int index)
        {
            return this.subMeshes[index];
        }

        public SubMesh GetSubMesh(string name)
        {
            int idx = this.GetSubMeshIndex(name);
            
            return this.GetSubMesh(idx);
        }

        private int GetSubMeshIndex(string name)
        {
            int idx;
            if (!this.subMeshNamesMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("No submesh with a name {0} exists", name));
            }

            return idx;
        }

        private void ApplyChanges()
        {
            for (int i = 0; i < this.subMeshes.Count; i++)
            {
                RenderingInfo renderData = this.subMeshes[i].RenderInfo;
                renderData.useIndexes = this.UseIndexes;
                renderData.iBuffer = this.iBuffer;
            }
        }

        internal void ComputeData()
        {
            this.VerticesCount = this.vBuffer.VertexCount;
            this.PrimitiveCount = 0;
            for (int i = 0; i < this.subMeshes.Count; i++)
            {
                SubMesh sub = this.subMeshes[i];
                this.PrimitiveCount += sub.RenderInfo.triangleCount;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the total of vertices for this mesh
        /// </summary>
        public int VerticesCount { get; internal set; }

        public int PrimitiveCount { get; internal set; }

        public int SubMeshCount
        {
            get { return this.subMeshes.Count; }
        }

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

        public bool UseIndexes 
        {
            get { return this.useIndexes; }
            set
            {
                this.useIndexes = value;
                this.ApplyChanges();
            }
        }

        /// <summary>
        /// Get the vertex buffer of the mesh
        /// </summary>
        public VertexBuffer VBuffer
        {
            get { return this.vBuffer; }
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
