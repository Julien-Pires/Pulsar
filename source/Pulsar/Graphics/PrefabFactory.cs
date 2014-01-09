using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;
using Pulsar.Graphics.Asset;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Used to create various type of primitive mesh(box, sphere, ...)
    /// </summary>
    public sealed class PrefabFactory
    {
        #region Fields

        private readonly AssetEngine _assetEngine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the PrefabFactory class
        /// </summary>
        internal PrefabFactory(AssetEngine assetEngine)
        {
            _assetEngine = assetEngine;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a mesh that represents a box
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        /// <param name="depth">Depth of the box</param>
        /// <param name="storage">Storage used to store the mesh</param>
        /// <returns>Returns a mesh</returns>
        public Mesh CreateBox(string name, float width, float height, float depth, string storage)
        {
            float halfWidth = width/2.0f;
            float halfHeight = height/2.0f;
            float halfDepth = height/2.0f;
            Vector3 topLeftFront = new Vector3(-halfWidth, halfHeight, halfDepth);
            Vector3 topRightFront = new Vector3(halfWidth, halfHeight, halfDepth);
            Vector3 bottomLeftFront = new Vector3(-halfWidth, -halfHeight, halfDepth);
            Vector3 bottomRightFront = new Vector3(halfWidth, -halfHeight, halfDepth);
            Vector3 topRightBack = new Vector3(-halfWidth, halfHeight, -halfDepth);
            Vector3 topLeftBack = new Vector3(halfWidth, halfHeight, -halfDepth);
            Vector3 bottomRightBack = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            Vector3 bottomLeftBack = new Vector3(halfWidth, -halfHeight, -halfDepth);

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

            Mesh meshBox = _assetEngine[storage].Load<Mesh>(name, MeshParameters.NewInstance);
            meshBox.Bones = new Matrix[1];
            meshBox.Bones[0] = Matrix.Identity;

            meshBox.Begin(string.Empty, PrimitiveType.TriangleList, true, true, true);

            // Front
            meshBox.Position(ref bottomLeftFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topLeftFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref topRightFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref bottomRightFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomLeftFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topRightFront); meshBox.Normal(ref frontNormal); meshBox.Texture(ref texTopRight);

            // Back
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topLeftBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref topRightBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref bottomRightBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topRightBack); meshBox.Normal(ref backNormal); meshBox.Texture(ref texTopRight);

            // Top
            meshBox.Position(ref topLeftFront); meshBox.Normal(ref topNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topRightBack); meshBox.Normal(ref topNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref topLeftBack); meshBox.Normal(ref topNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref topRightFront); meshBox.Normal(ref topNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref topLeftFront); meshBox.Normal(ref topNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topLeftBack); meshBox.Normal(ref topNormal); meshBox.Texture(ref texTopRight);

            // Bottom
            meshBox.Position(ref bottomLeftFront); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomRightBack); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref bottomRightFront); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomLeftFront); meshBox.Normal(ref bottomNormal); meshBox.Texture(ref texTopLeft);

            // Left
            meshBox.Position(ref topLeftFront); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref bottomLeftFront); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomRightBack); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref bottomRightBack); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topRightBack); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref topLeftFront); meshBox.Normal(ref leftNormal); meshBox.Texture(ref texTopRight);

            // Right
            meshBox.Position(ref topRightFront); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texTopLeft);
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref bottomRightFront); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texBottomLeft);
            meshBox.Position(ref topLeftBack); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texTopRight);
            meshBox.Position(ref bottomLeftBack); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texBottomRight);
            meshBox.Position(ref topRightFront); meshBox.Normal(ref rightNormal); meshBox.Texture(ref texTopLeft);

            meshBox.End();

            string materialName = string.Format("{0}_material", name);
            meshBox.GetSubMesh(0).Material = _assetEngine[storage].Load<Material>(materialName,
                MaterialParameters.NewInstance);

            return meshBox;
        }

        #endregion
    }
}
