using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PulsarRuntime.Graphics;

using Pulsar.Core;
using Pulsar.Assets.Graphics.Materials;
using Pulsar.Game;
using Pulsar.Graphics;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Assets.Graphics.Models
{
    /// <summary>
    /// Used to load Mesh
    /// </summary>
    public sealed class MeshManager : Singleton<MeshManager>, IAssetManager
    {
        #region Fields

        private readonly GraphicsEngine _engine;
        private readonly AssetGroup<Mesh> _assetGroup;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the MeshManager class
        /// </summary>
        private MeshManager()
        {
            _assetGroup = new AssetGroup<Mesh>("Mesh", this);
            GraphicsEngineService engineService = GameApplication.GameServices.GetService(typeof(IGraphicsEngineService)) 
                as GraphicsEngineService;

            if (engineService == null) throw new ArgumentException("GraophicsEngine service cannot be found");
            _engine = engineService.Engine;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads an empty mesh
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="storage">Storage in which the instance will be stored</param>
        /// <returns>Returns an instance of mesh if it exists in the storage, otherwise a new one</returns>
        public Mesh LoadEmpty(string name, string storage)
        {
            AssetSearchResult<Mesh> result = _assetGroup.Load(name, storage);
            Mesh mesh = result.Resource;

            return mesh;
        }

        /// <summary>
        /// Loads a mesh from a file
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="storage">Storage in which the instance will be stored</param>
        /// <param name="file">Name of the file containing the 3D mesh</param>
        /// <returns>Returns an instance of mesh if it exists in the storage, otherwise a new one</returns>
        public Mesh Load(string name, string storage, string file)
        {
            AssetSearchResult<Mesh> result = _assetGroup.Load(name, storage);
            Mesh mesh = result.Resource;
            if (!result.Created) return mesh;

            AssetStorage usedStorage = AssetStorageManager.Instance.GetStorage(storage);
            Model model = usedStorage.ResourceManager.Load<Model>(file);
            ProcessModel(mesh, model, storage);

            return mesh;
        }

        /// <summary>
        /// Unloads a mesh
        /// </summary>
        /// <param name="name">Name of the mesh to unload</param>
        /// <param name="storage">Storage in which the mesh is stored</param>
        /// <returns>Returns true if the mesh is unloaded successfully otherwise false</returns>
        public bool Unload(string name, string storage)
        {
            return _assetGroup.Unload(name, storage);
        }

        /// <summary>
        /// Creates a new instance of a mesh
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="parameter">Additional parameter to create the mesh</param>
        /// <returns>Returns a new mesh</returns>
        public Asset CreateInstance(string name, params object[] parameter)
        {
            return new Mesh(name, _engine.BufferManager);
        }

        /// <summary>
        /// Converts a XNA model instance to a mesh instance
        /// </summary>
        /// <param name="mesh">Instance of a mesh to receive all the model information</param>
        /// <param name="model">Model instance from which to extract data</param>
        /// <param name="storage">Storage in which data will be stored</param>
        private void ProcessModel(Mesh mesh, Model model, string storage)
        {
            MeshData data = (MeshData)model.Tag;
            Matrix[] bones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones);
            mesh.Bones = bones;

            VertexData meshVertexData = mesh.VertexData;
            IndexData meshIndexData = mesh.IndexData;
            VertexBufferObject vbo = null;
            IndexBufferObject ibo = null;
            if (model.Meshes.Count > 0)
            {
                ModelMeshPart part = model.Meshes[0].MeshParts[0];
                vbo = _engine.BufferManager.CreateVertexBuffer(part.VertexBuffer);
                ibo = _engine.BufferManager.CreateIndexBuffer(part.IndexBuffer);
            }

            if (vbo != null) meshVertexData.SetBinding(vbo, 0, 0, 0);
            if (ibo != null) meshIndexData.IndexBuffer = ibo;

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh currMesh = model.Meshes[i];
                for (int j = 0; j < currMesh.MeshParts.Count; j++)
                {
                    ModelMeshPart part = currMesh.MeshParts[j];
                    SubMeshData subData = data.SubMeshData[i];
                    SubMesh sub = mesh.CreateSubMesh(currMesh.Name);

                    sub.AxisAlignedBoundingBox = subData.BoundingVolume.BoundingBox;
                    sub.BoundingSphere = subData.BoundingVolume.BoundingSphere;
                    sub.BoneIndex = currMesh.ParentBone.Index;

                    sub.ShareVertexBuffer = true;
                    sub.VertexData.SetBinding(vbo, part.VertexOffset, 0);
                    sub.RenderInfo.PrimitiveType = PrimitiveType.TriangleList;
                    sub.RenderInfo.VertexCount = part.NumVertices;
                    sub.RenderInfo.PrimitiveCount = part.PrimitiveCount;

                    sub.ShareIndexBuffer = true;
                    sub.RenderInfo.UseIndexes = true;
                    sub.IndexData.IndexBuffer = ibo;
                    sub.IndexData.StartIndex = part.StartIndex;
                    

                    string materialName = mesh.Name + @"/" + currMesh.Name + "_material";
                    Material mat = MaterialManager.Instance.CreateMaterial(materialName, storage, part.Effect, subData.TexturesName);
                    sub.Material = mat;
                }
            }

            mesh.UpdateBounds();
            mesh.UpdateMeshInfo();
        }

        #endregion
    }
}