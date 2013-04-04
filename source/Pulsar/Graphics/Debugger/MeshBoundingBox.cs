using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Game;
using Pulsar.Core;
using Pulsar.Assets.Graphics.Models;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Graphics.Graph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics.Debugger
{
    /// <summary>
    /// Class to create an draw a bounding box
    /// Used only for debugging
    /// </summary>
    internal sealed class MeshBoundingBox : IRenderable
    {
        #region Fields

        private const string materialName = "MeshBoundingBox_Material";
        private const int verticesCount = 24;
        private const int primitiveCount = 12;

        private uint id;
        private DynamicVertexBuffer vBuffer = null;
        private IndexBuffer iBuffer = null;
        private RenderingInfo renderInfo = null;
        private Material material = null;
        private int[] indices = new int[MeshBoundingBox.verticesCount];
        private VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[MeshBoundingBox.verticesCount];

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MeshBoundingBox class
        /// </summary>
        internal MeshBoundingBox()
        {
            this.InitBuffers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize all the buffers
        /// </summary>
        private void InitBuffers()
        {
            this.id = SubMesh.GetID();
            for (int i = 0; i < MeshBoundingBox.verticesCount; i++)
            {
                indices[i] = i;
            }
            this.vBuffer = new DynamicVertexBuffer(GameApplication.GameGraphicsDevice, typeof(VertexPositionNormalTexture),
                MeshBoundingBox.verticesCount, BufferUsage.WriteOnly);
            this.iBuffer = new IndexBuffer(GameApplication.GameGraphicsDevice, IndexElementSize.ThirtyTwoBits,
                MeshBoundingBox.verticesCount, BufferUsage.WriteOnly);
            this.iBuffer.SetData<int>(this.indices);

            this.renderInfo = new RenderingInfo()
            {
                ID = this.id,
                Primitive = PrimitiveType.LineList,
                VBuffer = this.vBuffer,
                IBuffer = this.iBuffer,
                VertexCount = MeshBoundingBox.verticesCount,
                TriangleCount = MeshBoundingBox.primitiveCount,
                StartIndex = 0,
                VertexOffset = 0
            };

            this.material = MaterialManager.Instance.LoadDefault();
            this.material.DiffuseColor = Color.White;
        }

        /// <summary>
        /// Update the buffers
        /// </summary>
        /// <param name="box">Boundig box used to update buffers data</param>
        internal void UpdateBox(ref BoundingBox box)
        {
            Vector3 xOffset = new Vector3((box.Max.X - box.Min.X), 0, 0);
            Vector3 yOffset = new Vector3(0, (box.Max.Y - box.Min.Y), 0);
            Vector3 zOffset = new Vector3(0, 0, (box.Max.Z - box.Min.Z));
            Vector3 minOpposite = box.Min + (xOffset + zOffset);
            Vector3 maxOpposite = box.Max - (xOffset + zOffset);

            //// Top
            this.vertices[0] = new VertexPositionNormalTexture(box.Min, Vector3.UnitY, Vector2.Zero);
            this.vertices[1] = new VertexPositionNormalTexture(box.Min + xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[2] = new VertexPositionNormalTexture(box.Min, Vector3.UnitY, Vector2.Zero);
            this.vertices[3] = new VertexPositionNormalTexture(box.Min + zOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[4] = new VertexPositionNormalTexture(minOpposite, Vector3.UnitY, Vector2.Zero);
            this.vertices[5] = new VertexPositionNormalTexture(box.Min + xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[6] = new VertexPositionNormalTexture(minOpposite, Vector3.UnitY, Vector2.Zero);
            this.vertices[7] = new VertexPositionNormalTexture(box.Min + zOffset, Vector3.UnitY, Vector2.Zero);

            //// Bottom
            this.vertices[8] = new VertexPositionNormalTexture(box.Max, Vector3.UnitY, Vector2.Zero);
            this.vertices[9] = new VertexPositionNormalTexture(box.Max - xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[10] = new VertexPositionNormalTexture(box.Max, Vector3.UnitY, Vector2.Zero);
            this.vertices[11] = new VertexPositionNormalTexture(box.Max - zOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[12] = new VertexPositionNormalTexture(maxOpposite, Vector3.UnitY, Vector2.Zero);
            this.vertices[13] = new VertexPositionNormalTexture(box.Max - xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[14] = new VertexPositionNormalTexture(maxOpposite, Vector3.UnitY, Vector2.Zero);
            this.vertices[15] = new VertexPositionNormalTexture(box.Max - zOffset, Vector3.UnitY, Vector2.Zero);

            //// Sides
            this.vertices[16] = new VertexPositionNormalTexture(box.Min, Vector3.UnitY, Vector2.Zero);
            this.vertices[17] = new VertexPositionNormalTexture(box.Min + yOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[18] = new VertexPositionNormalTexture(box.Min + xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[19] = new VertexPositionNormalTexture((box.Min + xOffset) + yOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[20] = new VertexPositionNormalTexture(box.Max, Vector3.UnitY, Vector2.Zero);
            this.vertices[21] = new VertexPositionNormalTexture(box.Max - yOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[22] = new VertexPositionNormalTexture(box.Max - xOffset, Vector3.UnitY, Vector2.Zero);
            this.vertices[23] = new VertexPositionNormalTexture((box.Max - xOffset) - yOffset, Vector3.UnitY, Vector2.Zero);
            
            this.vBuffer.SetData<VertexPositionNormalTexture>(this.vertices);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of this instace
        /// </summary>
        public string Name 
        {
            get { return "Debug AABB"; } 
        }

        /// <summary>
        /// Get or set a boolean to enable instancing
        /// </summary>
        public bool UseInstancing 
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the batch ID of this instance
        /// </summary>
        public uint BatchID 
        {
            get { return this.id; } 
        }

        /// <summary>
        /// Get the render qeue ID of this instance
        /// </summary>
        public int RenderQueueID 
        {
            get { return (int)RenderQueueGroupID.Default; } 
        }

        /// <summary>
        /// Get the transform matrix of this instance
        /// </summary>
        public Matrix Transform 
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Get the rendering info of this instance
        /// </summary>
        public RenderingInfo RenderInfo 
        {
            get { return this.renderInfo; }
        }

        /// <summary>
        /// Get the material associated to this instance
        /// </summary>
        public Material Material
        {
            get { return this.material; }
        }

        #endregion
    }
}
