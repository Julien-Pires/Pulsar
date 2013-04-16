﻿using System;

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

        private Mesh parent;
        private BoundingData bounds;
        private RenderingInfo renderData = new RenderingInfo();

        #endregion

        #region Constructors

        internal SubMesh(Mesh parent)
        {
            this.parent = parent;
            this.renderData.useIndexes = this.parent.UseIndexes;
            this.renderData.vBuffer = this.parent.VBuffer;
            this.renderData.iBuffer = this.parent.IBuffer;
        }

        #endregion

        #region Methods

        public void SetRenderingInfo(PrimitiveType pType, int startIdx, int primitiveCount, int numVertices, int vertexOffset)
        {
            this.renderData.Primitive = pType;
            this.renderData.startIndex = startIdx;
            this.renderData.triangleCount = primitiveCount;
            this.renderData.vertexCount = numVertices;
            this.renderData.vertexOffset = vertexOffset;
            this.parent.ComputeData();
        }

        public void SetBoundingVolume(BoundingBox aabb, BoundingSphere sphere)
        {
            this.bounds.BoundingBox = aabb;
            this.bounds.BoundingSphere = sphere;
        }

        public void SetBoundingVolume(ref BoundingBox aabb, ref BoundingSphere sphere)
        {
            this.bounds.BoundingBox = aabb;
            this.bounds.BoundingSphere = sphere;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ID of this instance
        /// </summary>
        public uint ID
        {
            get { return this.renderData.id; }
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
