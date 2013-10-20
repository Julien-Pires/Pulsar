using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Materials;
using Pulsar.Core;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Used to create various type of primitive mesh(box, sphere, ...)
    /// </summary>
    public sealed class PrefabFactory : Singleton<PrefabFactory>
    {
        #region Constructors

        /// <summary>
        /// Constructor of the PrefabFactory class
        /// </summary>
        private PrefabFactory()
        {
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
        /// <returns>Returns a mesh</returns>
        public Mesh CreateBox(string name, float width, float height, float depth)
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

            Mesh box = MeshManager.Instance.LoadEmpty(name, "Default");
            box.Bones = new Matrix[1];
            box.Bones[0] = Matrix.Identity;

            box.Begin(string.Empty, PrimitiveType.TriangleList, true, true);

            // Front
            box.Position(ref bottomLeftFront); box.Normal(ref frontNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topLeftFront); box.Normal(ref frontNormal); box.Texture(ref texTopLeft);
            box.Position(ref topRightFront); box.Normal(ref frontNormal); box.Texture(ref texTopRight);
            box.Position(ref bottomRightFront); box.Normal(ref frontNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomLeftFront); box.Normal(ref frontNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topRightFront); box.Normal(ref frontNormal); box.Texture(ref texTopRight);

            // Back
            box.Position(ref bottomLeftBack); box.Normal(ref backNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topLeftBack); box.Normal(ref backNormal); box.Texture(ref texTopLeft);
            box.Position(ref topRightBack); box.Normal(ref backNormal); box.Texture(ref texTopRight);
            box.Position(ref bottomRightBack); box.Normal(ref backNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomLeftBack); box.Normal(ref backNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topRightBack); box.Normal(ref backNormal); box.Texture(ref texTopRight);

            // Top
            box.Position(ref topLeftFront); box.Normal(ref topNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topRightBack); box.Normal(ref topNormal); box.Texture(ref texTopLeft);
            box.Position(ref topLeftBack); box.Normal(ref topNormal); box.Texture(ref texTopRight);
            box.Position(ref topRightFront); box.Normal(ref topNormal); box.Texture(ref texBottomRight);
            box.Position(ref topLeftFront); box.Normal(ref topNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topLeftBack); box.Normal(ref topNormal); box.Texture(ref texTopRight);

            // Bottom
            box.Position(ref bottomLeftFront); box.Normal(ref bottomNormal); box.Texture(ref texTopLeft);
            box.Position(ref bottomLeftBack); box.Normal(ref bottomNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomRightBack); box.Normal(ref bottomNormal); box.Texture(ref texBottomLeft);
            box.Position(ref bottomRightFront); box.Normal(ref bottomNormal); box.Texture(ref texTopRight);
            box.Position(ref bottomLeftBack); box.Normal(ref bottomNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomLeftFront); box.Normal(ref bottomNormal); box.Texture(ref texTopLeft);

            // Left
            box.Position(ref topLeftFront); box.Normal(ref leftNormal); box.Texture(ref texTopRight);
            box.Position(ref bottomLeftFront); box.Normal(ref leftNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomRightBack); box.Normal(ref leftNormal); box.Texture(ref texBottomLeft);
            box.Position(ref bottomRightBack); box.Normal(ref leftNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topRightBack); box.Normal(ref leftNormal); box.Texture(ref texTopLeft);
            box.Position(ref topLeftFront); box.Normal(ref leftNormal); box.Texture(ref texTopRight);

            // Right
            box.Position(ref topRightFront); box.Normal(ref rightNormal); box.Texture(ref texTopLeft);
            box.Position(ref bottomLeftBack); box.Normal(ref rightNormal); box.Texture(ref texBottomRight);
            box.Position(ref bottomRightFront); box.Normal(ref rightNormal); box.Texture(ref texBottomLeft);
            box.Position(ref topLeftBack); box.Normal(ref rightNormal); box.Texture(ref texTopRight);
            box.Position(ref bottomLeftBack); box.Normal(ref rightNormal); box.Texture(ref texBottomRight);
            box.Position(ref topRightFront); box.Normal(ref rightNormal); box.Texture(ref texTopLeft);

            box.End();

            box.GetSubMesh(0).Material = MaterialManager.Instance.LoadDefault();

            return box;
        }

        #endregion
    }
}
