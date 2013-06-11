using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Game;
using Pulsar.Core;
using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Class to create various type of primitive mesh(box, sphere, ...)
    /// </summary>
    public sealed class PrefabFactory : Singleton<PrefabFactory>
    {
        #region Fields

        private GraphicsEngine engine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the PrefabFactory class
        /// </summary>
        private PrefabFactory()
        {
            GameServiceContainer services = GameApplication.GameServices;
            GraphicsEngineService engineService = services.GetService(typeof(IGraphicsEngineService)) as GraphicsEngineService;
            if (engineService == null)
            {
                throw new ArgumentException("GraophicsEngine service cannot be found");
            }
            this.engine = engineService.Engine;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a box mesh
        /// </summary>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        /// <param name="depth">Depth of the box</param>
        /// <returns>Return a new box mesh</returns>
        public Mesh CreateBox(float width, float height, float depth)
        {
            int verticesCount = 36;
            float halfW = width / 2.0f;
            float halfH = height / 2.0f;
            float halfD = depth / 2.0f;
            short[] indices = new short[verticesCount];

            Vector3 topLeftFront = new Vector3(-halfW, halfH, halfD);
            Vector3 topRightFront = new Vector3(halfW, halfH, halfD);
            Vector3 bottomLeftFront = new Vector3(-halfW, -halfH, halfD);
            Vector3 bottomRightFront = new Vector3(halfW, -halfH, halfD);
            Vector3 topLeftBack = new Vector3(-halfW, halfH, -halfD);
            Vector3 topRightBack = new Vector3(halfW, halfH, -halfD);
            Vector3 bottomLeftBack = new Vector3(-halfW, -halfH, -halfD);
            Vector3 bottomRightBack = new Vector3(halfW, -halfH, -halfD);
            Vector3[] vec3List = new Vector3[8]
            { 
                topLeftFront, topRightFront, bottomLeftFront, bottomRightFront,
                topLeftBack, topRightBack, bottomLeftBack, bottomRightBack 
            };

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            Vector2 texTopLeft = new Vector2(0.0f, 0.0f);
            Vector2 texTopRight = new Vector2(1.0f, 0.0f);
            Vector2 texBottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 texBottomRight = new Vector2(1.0f, 1.0f);

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[verticesCount];

            // Front
            vertices[1] = new VertexPositionNormalTexture(topLeftFront, frontNormal, texTopLeft);
            vertices[0] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, texBottomLeft);
            vertices[2] = new VertexPositionNormalTexture(topRightFront, frontNormal, texTopRight);
            vertices[4] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, texBottomLeft);
            vertices[3] = new VertexPositionNormalTexture(bottomRightFront, frontNormal, texBottomRight);
            vertices[5] = new VertexPositionNormalTexture(topRightFront, frontNormal, texTopRight);

            // Back
            vertices[7] = new VertexPositionNormalTexture(topLeftBack, backNormal, texTopRight);
            vertices[6] = new VertexPositionNormalTexture(topRightBack, backNormal, texTopLeft);
            vertices[8] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, texBottomRight);
            vertices[10] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, texBottomRight);
            vertices[9] = new VertexPositionNormalTexture(topRightBack, backNormal, texTopLeft);
            vertices[11] = new VertexPositionNormalTexture(bottomRightBack, backNormal, texBottomLeft);

            // Top
            vertices[13] = new VertexPositionNormalTexture(topLeftFront, topNormal, texBottomLeft);
            vertices[12] = new VertexPositionNormalTexture(topRightBack, topNormal, texTopRight);
            vertices[14] = new VertexPositionNormalTexture(topLeftBack, topNormal, texTopLeft);
            vertices[16] = new VertexPositionNormalTexture(topLeftFront, topNormal, texBottomLeft);
            vertices[15] = new VertexPositionNormalTexture(topRightFront, topNormal, texBottomRight);
            vertices[17] = new VertexPositionNormalTexture(topRightBack, topNormal, texTopRight);

            // Bottom
            vertices[19] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, texTopLeft);
            vertices[18] = new VertexPositionNormalTexture(bottomLeftBack, bottomNormal, texBottomLeft);
            vertices[20] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, texBottomRight);
            vertices[22] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, texTopLeft);
            vertices[21] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, texBottomRight);
            vertices[23] = new VertexPositionNormalTexture(bottomRightFront, bottomNormal, texTopRight);

            // Left
            vertices[25] = new VertexPositionNormalTexture(topLeftFront, leftNormal, texTopRight);
            vertices[24] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, texBottomLeft);
            vertices[26] = new VertexPositionNormalTexture(bottomLeftFront, leftNormal, texBottomRight);
            vertices[28] = new VertexPositionNormalTexture(topLeftBack, leftNormal, texTopLeft);
            vertices[27] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, texBottomLeft);
            vertices[29] = new VertexPositionNormalTexture(topLeftFront, leftNormal, texTopRight);

            // Right
            vertices[31] = new VertexPositionNormalTexture(topRightFront, rightNormal, texTopLeft);
            vertices[30] = new VertexPositionNormalTexture(bottomRightFront, rightNormal, texBottomLeft);
            vertices[32] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, texBottomRight);
            vertices[34] = new VertexPositionNormalTexture(topRightBack, rightNormal, texTopRight);
            vertices[33] = new VertexPositionNormalTexture(topRightFront, rightNormal, texTopLeft);
            vertices[35] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, texBottomRight);

            for (short i = 0; i < verticesCount; i++)
            {
                indices[i] = i;
            }

            string name = width + "x" + height + "x" + depth + "_box";
            Mesh mesh = MeshManager.Instance.LoadEmpty(name, "Default");

            VertexData vData = mesh.vertexData;
            VertexBufferObject vbo = this.engine.BufferManager.CreateVertexBuffer(BufferType.StaticWriteOnly, 
                typeof(VertexPositionNormalTexture), verticesCount);
            vbo.SetData(vertices);
            vData.SetBinding(vbo);

            IndexData iData = mesh.indexData;
            IndexBufferObject ibo = this.engine.BufferManager.CreateIndexBuffer(BufferType.StaticWriteOnly,
                IndexElementSize.SixteenBits, verticesCount);
            ibo.SetData(indices);
            iData.indexBuffer = ibo;

            SubMesh sub = mesh.CreateSubMesh();
            sub.SetRenderingInfo(PrimitiveType.TriangleList, 0, (verticesCount / 3), verticesCount);

            BoundingData bounds = this.ComputeBoundingVolume(vec3List);
            sub.BoundingVolume = bounds;

            return mesh;
        }

        /// <summary>
        /// Cmpute multiple bounding volume from an array of vertex
        /// </summary>
        /// <param name="vertices">Array containing vertices</param>
        /// <returns>Return new bounding data</returns>
        private BoundingData ComputeBoundingVolume(Vector3[] vertices)
        {
            BoundingData bounds = new BoundingData();
            bounds.BoundingBox = BoundingBox.CreateFromPoints(vertices);
            bounds.BoundingSphere = BoundingSphere.CreateFromPoints(vertices);

            return bounds;
        }

        #endregion
    }
}
