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
        private Mesh parent;
        private BoundingData bounds;
        private RenderingInfo renderData = new RenderingInfo();

        #endregion

        #region Constructors

        internal SubMesh(Mesh parent)
        {
            this.parent = parent;
            this.renderData.UseIndexes = this.parent.UseIndexes;
            this.renderData.VBuffer = this.parent.VBuffer;
            this.renderData.IBuffer = this.parent.IBuffer;
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

        public void SetRenderingInfo(PrimitiveType pType, int vertexOffset, int vertexCount, int startIndex)
        {
            int vCount = vertexCount;
            if (this.renderData.UseIndexes)
            {
                int indexCount = this.renderData.IBuffer.IndexCount;
                vCount = indexCount - startIndex;
            }

            int primitiveCount = 0;
            switch (pType)
            {
                case PrimitiveType.LineList: primitiveCount = vertexCount / 2;
                    break;
                case PrimitiveType.LineStrip: primitiveCount = vertexCount - 1;
                    break;
                case PrimitiveType.TriangleList: primitiveCount = vertexCount / 3;
                    break;
                case PrimitiveType.TriangleStrip: primitiveCount = vertexCount - 2;
                    break;
            }


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
        /// Get the information to render this sub mesh
        /// </summary>
        internal RenderingInfo RenderInfo 
        {
            get { return this.renderData; }  
        }

        /// <summary>
        /// Get the material attached to this sub mesh
        /// </summary>
        public Material Material { get; internal set; }

        /// <summary>
        /// Get or set the bounding volume data
        /// </summary>
        public BoundingData BoundingVolume
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
