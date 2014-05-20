using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Mesh asset
    /// </summary>
    [AssetLoader(AssetTypes = new[] { typeof(Mesh) }, LazyInitCategory = GraphicsStorage.LoadersCategory)]
    public sealed class MeshLoader : AssetLoader
    {
        #region Fields

        private const string DiffuseMapKey = "Texture";
        private const string NormalMapKey = "NormalMap";
        private const string SpecularMapKey = "SpecularMap";

        private BufferManager _bufferManager;
        private readonly MeshParameters _defaultParameters = new MeshParameters();
        private readonly LoadedAsset _result = new LoadedAsset();
        private readonly LoadedAsset _fromFileResult = new LoadedAsset();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MeshLoader class
        /// </summary>
        internal MeshLoader()
        {
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Converts an XNA model to a mesh
        /// </summary>
        /// <param name="meshName">Name of the mesh</param>
        /// <param name="model">XNA model</param>
        /// <param name="bufferManager">Buffer manager</param>
        /// <param name="assetFolder">Asset folder</param>
        /// <param name="result">Result that contains the mesh and his resources</param>
        private static void ProcessModel(string meshName, Model model, BufferManager bufferManager, 
            AssetFolder assetFolder, LoadedAsset result)
        {
            MeshData data = model.Tag as MeshData;
            if(data == null)
                throw new Exception("Mesh data incomplete");

            Mesh mesh = new Mesh(meshName, bufferManager);
            result.Asset = mesh;

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
                vbo = bufferManager.CreateVertexBuffer(part.VertexBuffer);
                ibo = bufferManager.CreateIndexBuffer(part.IndexBuffer);
            }

            if (vbo != null)
                meshVertexData.SetBinding(vbo, 0, 0, 0);

            if (ibo != null)
                meshIndexData.IndexBuffer = ibo;

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh currentMesh = model.Meshes[i];
                for (int j = 0; j < currentMesh.MeshParts.Count; j++)
                {
                    SubMeshData subData = data.SubMeshDatas[i];
                    SubMesh subMesh = mesh.CreateSubMesh(currentMesh.Name);
                    subMesh.AxisAlignedBoundingBox = subData.Aabb;
                    subMesh.BoundingSphere = subData.BoundingSphere;
                    subMesh.BoneIndex = currentMesh.ParentBone.Index;

                    ModelMeshPart part = currentMesh.MeshParts[j];
                    subMesh.ShareVertexBuffer = true;
                    subMesh.VertexData.SetBinding(vbo, part.VertexOffset, 0);
                    subMesh.RenderInfo.PrimitiveType = PrimitiveType.TriangleList;
                    subMesh.RenderInfo.VertexCount = part.NumVertices;
                    subMesh.RenderInfo.PrimitiveCount = part.PrimitiveCount;

                    subMesh.ShareIndexBuffer = true;
                    subMesh.RenderInfo.UseIndexes = true;
                    subMesh.IndexData.IndexBuffer = ibo;
                    subMesh.IndexData.StartIndex = part.StartIndex;
                    subMesh.IndexData.IndexCount = part.IndexBuffer.IndexCount;

                    string materialName = string.Format("{0}\\{1}_material", mesh.Name, currentMesh.Name);
                    subMesh.Material = CreateMaterial(materialName, part.Effect, subData.TexturesName, assetFolder);
                }
            }

            mesh.UpdateBounds();
            mesh.UpdateMeshInfo();
        }

        /// <summary>
        /// Creates a material from an effect
        /// </summary>
        /// <param name="materialName">Name of the material</param>
        /// <param name="effect">Effect</param>
        /// <param name="texturesName">Map with the name of texture to extract</param>
        /// <param name="assetFolder">Asset folder</param>
        /// <returns>Returns a material instance</returns>
        private static Material CreateMaterial(string materialName, Effect effect, Dictionary<string, string> texturesName,
             AssetFolder assetFolder)
        {
            Material material = assetFolder.Load<Material>(materialName, MaterialParameters.NewInstance);
            EffectParameter fxParameter = effect.Parameters[DiffuseMapKey];
            if (fxParameter != null)
                material.DiffuseMap = GetTextureFromFullPath(texturesName[DiffuseMapKey], assetFolder);

            fxParameter = effect.Parameters[NormalMapKey];
            if (fxParameter != null)
                material.NormalMap = GetTextureFromFullPath(texturesName[NormalMapKey], assetFolder);

            fxParameter = effect.Parameters[SpecularMapKey];
            if (fxParameter != null)
                material.SpecularMap = GetTextureFromFullPath(texturesName[SpecularMapKey], assetFolder);

            return material;
        }

        /// <summary>
        /// Gets a texture from a folder with a specified path
        /// </summary>
        /// <param name="path">Full path of the texture</param>
        /// <param name="folder">Asset folder</param>
        /// <returns>Returns a texture instance</returns>
        private static Texture GetTextureFromFullPath(string path, AssetFolder folder)
        {
            string assetName = folder.GetNameFromFullPath(path);

            return folder.Load<Texture>(assetName);
        }

        /// <summary>
        /// Transfers disposable from XNA model result to mesh result
        /// </summary>
        /// <param name="fileResult">XNA model result</param>
        /// <param name="finalResult">Mesh result</param>
        private static void TransferDisposable(LoadedAsset fileResult, LoadedAsset finalResult)
        {
            List<IDisposable> modelDisposables = fileResult.Disposables;
            for (int i = 0; i < modelDisposables.Count; i++)
            {
                Type iDisposableType = modelDisposables[i].GetType();
                if (iDisposableType == typeof (VertexDeclaration))
                {
                    finalResult.Disposables.Add(modelDisposables[i]);
                    continue;
                }

                if (iDisposableType.IsSubclassOf(typeof (Effect)))
                    modelDisposables[i].Dispose();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the loader
        /// </summary>
        /// <param name="engine">Asset engine that owns this loader</param>
        /// <param name="serviceProvider">Service provider</param>
        public override void Initialize(AssetEngine engine, IServiceProvider serviceProvider)
        {
            base.Initialize(engine, serviceProvider);

            IGraphicsEngineService engineService =
                serviceProvider.GetService(typeof (IGraphicsEngineService)) as IGraphicsEngineService;
            if(engineService == null)
                throw new Exception("Failed to find graphics engine service");

            _bufferManager = engineService.Engine.BufferManager;
        }

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <param name="assetFolder">Folder where the asset will be stored</param>
        /// <returns>Returns a loaded asset</returns>
        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();

            MeshParameters meshParameters;
            if (parameters != null)
            {
                meshParameters = parameters as MeshParameters;
                if (meshParameters == null)
                    throw new Exception("Invalid parameters for this loader");
            }
            else
            {
                meshParameters = _defaultParameters;
                meshParameters.Filename = path;
            }

            switch (meshParameters.Source)
            {
                case AssetSource.FromFile:
                    LoadFromFile<Model>(meshParameters.Filename, assetFolder, _fromFileResult);

                    Model model = _fromFileResult.Asset as Model;
                    ProcessModel(assetName, model, _bufferManager, assetFolder, _result);
                    TransferDisposable(_fromFileResult, _result);
                    break;

                case AssetSource.NewInstance:
                    Mesh mesh = new Mesh(assetName, _bufferManager);
                    _result.Asset = mesh;
                    _result.Disposables.Add(mesh);
                    break;

                default:
                    throw new Exception("Invalid asset source provided");
            }

            _fromFileResult.Reset();

            return _result;
        }

        #endregion
    }
}