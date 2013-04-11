using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Assets;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Game;
using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

using PangoTexture = Pulsar.Assets.Graphics.Materials.Texture;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Class used to load Mesh
    /// </summary>
    public sealed class MeshManager : Singleton<MeshManager>, IAssetManager
    {
        #region Fields

        private readonly AssetGroup<Mesh> assetGroup = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the MeshManager class
        /// </summary>
        private MeshManager()
        {
            this.assetGroup = new AssetGroup<Mesh>("Mesh", this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load an empty mesh
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="storage">Storage in which the instance will be stored</param>
        /// <returns>Return an instance of mesh if it exists in the storage, otherwise a new one</returns>
        public Mesh LoadEmpty(string name, string storage)
        {
            AssetSearchResult<Mesh> result = this.assetGroup.Load(name, storage);
            Mesh mesh = result.Resource;
            mesh.VBuffer = new VertexBuffer(GameApplication.GameGraphicsDevice, typeof(VertexPositionNormalTexture), 0,
                BufferUsage.WriteOnly);
            mesh.IBuffer = new IndexBuffer(GameApplication.GameGraphicsDevice, IndexElementSize.ThirtyTwoBits, 0,
                BufferUsage.WriteOnly);

            return mesh;
        }

        /// <summary>
        /// Load a mesh from a file
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="storage">Storage in which the instance will be stored</param>
        /// <param name="file">Name of the file containing the 3D mesh</param>
        /// <returns></returns>
        public Mesh Load(string name, string storage, string file)
        {
            AssetSearchResult<Mesh> result = this.assetGroup.Load(name, storage);
            Mesh mesh = result.Resource;

            if (result.Created)
            {
                AssetStorage usedStorage = AssetStorageManager.Instance.GetStorage(storage);
                Model model = usedStorage.ResourceManager.Load<Model>(file);
                this.ProcessModel(mesh, model, storage);
            }

            return mesh;
        }

        public bool Unload(string name, string storage)
        {
            return this.assetGroup.Unload(name, storage);
        }

        /// <summary>
        /// Create a new instance of a mesh
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="parameter">Additional parameter to create the mesh</param>
        /// <returns>Return a new mesh</returns>
        public Asset CreateInstance(string name, params object[] parameter)
        {
            return new Mesh(name);
        }

        /// <summary>
        /// Create a new sub mesh
        /// </summary>
        /// <param name="name">Name of the sub mesh</param>
        /// <param name="renderInf">Rendering informations of the sub mesh</param>
        /// <param name="bounds">Bounding volume of the sub mesh</param>
        /// <param name="mat">Material of the sub mesh</param>
        /// <param name="boneIdx">Index of the bone</param>
        /// <returns>Returns a new sub mesh</returns>
        private SubMesh CreateSubMesh(string name, RenderingInfo renderInf, BoundingData bounds, Material mat, int boneIdx)
        {
            SubMesh sub = new SubMesh()
            {
                Name = name,
                RenderInfo = renderInf,
                Material = mat,
                BoundingVolume = bounds,
                BoneIndex = boneIdx
            };

            return sub;
        }

        /// <summary>
        /// Create rendering information
        /// </summary>
        /// <param name="id">ID of the rendering batch</param>
        /// <param name="vBuffer">VertexBuffer for the rendering</param>
        /// <param name="iBuffer">IndexBuffer for the rendering</param>
        /// <param name="vertexCount">Number of vertex for the rendering</param>
        /// <param name="vertexOffset">Offset for the vertx buffer</param>
        /// <param name="polyCount">Number of polygon drawn by the rendering</param>
        /// <param name="startIdx">Starting index in the vertex buffer</param>
        /// <returns>Return new rendering information</returns>
        private RenderingInfo CreateRenderingInfo(uint id, VertexBuffer vBuffer, IndexBuffer iBuffer, int vertexCount, 
            int vertexOffset, int polyCount, int startIdx)
        {
            RenderingInfo renderInf = new RenderingInfo()
            {
                ID = id,
                VBuffer = vBuffer,
                IBuffer = iBuffer,
                VertexCount = vertexCount,
                VertexOffset = vertexOffset,
                TriangleCount = polyCount,
                StartIndex = startIdx
            };

            return renderInf;
        }

        /// <summary>
        /// Convert a XNA model instance to a mesh instance
        /// </summary>
        /// <param name="mesh">Instance of a mesh to receive all the model information</param>
        /// <param name="model">Model instance from which to extract data</param>
        /// <param name="storage">Storage in which data will be stored</param>
        private void ProcessModel(Mesh mesh, Model model, string storage)
        {
            MeshData data = (MeshData)model.Tag;
            List<SubMesh> subList = mesh.SubMeshes;
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh currMesh = model.Meshes[i];

                for (int j = 0; j < currMesh.MeshParts.Count; j++)
                {
                    ModelMeshPart part = currMesh.MeshParts[j];
                    SubMeshData subData = data.SubMeshData[i];

                    uint id = SubMesh.GetID();
                    RenderingInfo renderInf = this.CreateRenderingInfo(id, part.VertexBuffer, part.IndexBuffer, part.NumVertices,
                        part.VertexOffset, part.PrimitiveCount, part.StartIndex);

                    string materialName = mesh.Name + @"/" + currMesh.Name + "_material";
                    Material mat = MaterialManager.Instance.CreateMaterial(materialName, storage, part.Effect, subData.TexturesName);

                    SubMesh sub = this.CreateSubMesh(currMesh.Name, renderInf, subData.BoundingVolume, mat,
                        currMesh.ParentBone.Index);
                    subList.Add(sub);
                }
            }

            Matrix[] bones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones);
            mesh.Bones = bones;
            mesh.BoundingVolume = data.BoundingVolume;
            if (model.Meshes.Count > 0)
            {
                mesh.VBuffer = model.Meshes[0].MeshParts[0].VertexBuffer;
                mesh.IBuffer = model.Meshes[0].MeshParts[0].IndexBuffer;
            }
        }

        #endregion
    }
}