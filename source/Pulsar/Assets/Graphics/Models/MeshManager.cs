using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Core;
using Pulsar.Assets;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Game;
using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Class used to load Mesh
    /// </summary>
    public sealed class MeshManager : Singleton<MeshManager>, IAssetManager
    {
        #region Fields

        private GameServiceContainer services;
        private GraphicsEngine engine;
        private readonly AssetGroup<Mesh> assetGroup = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the MeshManager class
        /// </summary>
        private MeshManager()
        {
            this.assetGroup = new AssetGroup<Mesh>("Mesh", this);
            this.services = GameApplication.GameServices;
            GraphicsEngineService engineService = this.services.GetService(typeof(IGraphicsDeviceService)) as GraphicsEngineService;
            if (engineService == null)
            {
                throw new ArgumentException("GraophicsEngine service cannot be found");
            }
            this.engine = engineService.Engine;
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
            mesh.VBuffer = new VertexBuffer(this.engine.Renderer.GraphicsDevice, typeof(VertexPositionNormalTexture), 0,
                BufferUsage.WriteOnly);
            mesh.IBuffer = new IndexBuffer(this.engine.Renderer.GraphicsDevice, IndexElementSize.ThirtyTwoBits, 0,
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

        /// <summary>
        /// Unload a mesh
        /// </summary>
        /// <param name="name">Name of the mesh to unload</param>
        /// <param name="storage">Storage in which the mesh is stored</param>
        /// <returns>Returns true if the mesh is unloaded successfully otherwise false</returns>
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
        /// Convert a XNA model instance to a mesh instance
        /// </summary>
        /// <param name="mesh">Instance of a mesh to receive all the model information</param>
        /// <param name="model">Model instance from which to extract data</param>
        /// <param name="storage">Storage in which data will be stored</param>
        private void ProcessModel(Mesh mesh, Model model, string storage)
        {
            MeshData data = (MeshData)model.Tag;
            Matrix[] bones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones);
            mesh.UseIndexes = true;
            mesh.Bones = bones;
            mesh.BoundingVolume = data.BoundingVolume;
            if (model.Meshes.Count > 0)
            {
                mesh.VBuffer = model.Meshes[0].MeshParts[0].VertexBuffer;
                mesh.IBuffer = model.Meshes[0].MeshParts[0].IndexBuffer;
            }
            else
            {
                mesh.VBuffer = new VertexBuffer(this.engine.Renderer.GraphicsDevice, typeof(VertexPositionNormalTexture), 0,
                BufferUsage.WriteOnly);
                mesh.IBuffer = new IndexBuffer(this.engine.Renderer.GraphicsDevice, IndexElementSize.ThirtyTwoBits, 0,
                    BufferUsage.WriteOnly);
            }

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh currMesh = model.Meshes[i];
                for (int j = 0; j < currMesh.MeshParts.Count; j++)
                {
                    ModelMeshPart part = currMesh.MeshParts[j];
                    SubMeshData subData = data.SubMeshData[i];
                    string materialName = mesh.Name + @"/" + currMesh.Name + "_material";
                    Material mat = MaterialManager.Instance.CreateMaterial(materialName, storage, part.Effect, subData.TexturesName);
                    SubMesh sub = mesh.CreateSubMesh(currMesh.Name);
                    sub.SetRenderingInfo(PrimitiveType.TriangleList, part.StartIndex, part.PrimitiveCount, part.NumVertices, part.VertexOffset);
                    sub.Material = mat;
                    sub.BoneIndex = currMesh.ParentBone.Index;
                    sub.BoundingVolume = subData.BoundingVolume;
                }
            }
        }

        #endregion
    }
}