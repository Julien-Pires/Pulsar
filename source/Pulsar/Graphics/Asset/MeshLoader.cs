using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;
using Pulsar.Graphics.Rendering;
using PulsarRuntime.Graphics;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Asset
{
    public sealed class MeshLoader : AssetLoader
    {
        #region Fields

        private const string DiffuseMapKey = "Texture";
        private const string NormalMapKey = "NormalMap";
        private const string SpecularMapKey = "SpecularMap";

        private readonly Type[] _supportedTypes = { typeof(Mesh) };
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly BufferManager _bufferManager;
        private readonly MeshParameters _defaultParameters = new MeshParameters();
        private readonly LoadResult _result = new LoadResult();
        private readonly LoadResult _fromFileResult = new LoadResult();

        #endregion

        #region Constructors

        internal MeshLoader(GraphicsDeviceManager deviceManager, BufferManager bufferManager)
        {
            Debug.Assert(deviceManager != null);
            Debug.Assert(bufferManager != null);

            _deviceManager = deviceManager;
            _bufferManager = bufferManager;
        }

        #endregion

        #region Static methods

        private static void ProcessModel(string meshName, Model model, GraphicsDeviceManager deviceManager, 
            BufferManager bufferManager, LoadResult result)
        {
            MeshData data = model.Tag as MeshData;
            if(data == null)
                throw new Exception("Mesh data incomplete");

            LoadedAsset loadedMesh = result.AddAsset(meshName);
            Mesh mesh = new Mesh(meshName, bufferManager);
            loadedMesh.Asset = mesh;

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
                    SubMeshData subData = data.SubMeshData[i];
                    SubMesh subMesh = mesh.CreateSubMesh(currentMesh.Name);
                    subMesh.AxisAlignedBoundingBox = subData.BoundingVolume.BoundingBox;
                    subMesh.BoundingSphere = subData.BoundingVolume.BoundingSphere;
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

                    string materialName = string.Format("{0}/{1}_material", mesh.Name, currentMesh.Name);
                    subMesh.Material = CreateMaterial(materialName, part.Effect, subData.TexturesName, deviceManager,
                        result);
                }
            }

            mesh.UpdateBounds();
            mesh.UpdateMeshInfo();
        }

        private static Material CreateMaterial(string materialName, Effect effect, Dictionary<string, string> texturesName, 
            GraphicsDeviceManager deviceManager, LoadResult result)
        {
            Material material = new Material(materialName);
            LoadedAsset materialAsset = result.AddAsset(materialName);
            materialAsset.Asset = material;

            EffectParameter fxParameter = effect.Parameters[DiffuseMapKey];
            if (fxParameter != null)
                material.DiffuseMap = CreateTexture(texturesName[DiffuseMapKey], fxParameter.GetValueTexture2D(),
                    deviceManager, result);

            fxParameter = effect.Parameters[NormalMapKey];
            if (fxParameter != null)
                material.NormalMap = CreateTexture(texturesName[NormalMapKey], fxParameter.GetValueTexture2D(),
                    deviceManager, result);

            fxParameter = effect.Parameters[SpecularMapKey];
            if (fxParameter != null)
                material.SpecularMap = CreateTexture(texturesName[SpecularMapKey], fxParameter.GetValueTexture2D(),
                    deviceManager, result);

            return material;
        }

        private static Texture CreateTexture(string textureName, XnaTexture rawTexture, GraphicsDeviceManager deviceManager,
            LoadResult result)
        {
            LoadedAsset loadedTexture = result.Get(textureName);
            if (loadedTexture != null)
                return (Texture)loadedTexture.Asset;

            Texture texture = new Texture(deviceManager, textureName, rawTexture);
            LoadedAsset textureAsset = result.AddAsset(textureName);
            textureAsset.Asset = texture;
            textureAsset.Disposables.Add(texture);

            return texture;
        }

        private static void TransferDisposable(string filename, string meshName, LoadResult fileResult, 
            LoadResult finalResult)
        {
            LoadedAsset loadedModel = fileResult.Get(filename);
            LoadedAsset loadedMesh = finalResult.Get(meshName);
            List<IDisposable> modelDisposables = loadedModel.Disposables;
            for (int i = 0; i < modelDisposables.Count; i++)
            {
                Type iDisposableType = modelDisposables[i].GetType();
                if (iDisposableType == typeof (VertexDeclaration))
                {
                    loadedMesh.Disposables.Add(modelDisposables[i]);
                    continue;
                }

                if (iDisposableType.IsSubclassOf(typeof (Effect)))
                    modelDisposables[i].Dispose();
            }
        }

        #endregion

        #region Methods

        public override LoadResult Load<T>(string assetName, object parameters, Storage storage)
        {
            _result.Reset();

            MeshParameters meshParameters;
            if (parameters != null)
            {
                meshParameters = parameters as MeshParameters;
                if (meshParameters == null)
                    throw new Exception("");
            }
            else
            {
                meshParameters = _defaultParameters;
                meshParameters.Filename = assetName;
            }

            switch (meshParameters.Source)
            {
                case AssetSource.FromFile:
                    LoadFromFile<Model>(meshParameters.Filename, storage, _fromFileResult);
                    LoadedAsset modelAsset = _fromFileResult.Get(meshParameters.Filename);
                    Model model = modelAsset.Asset as Model;
                    ProcessModel(assetName, model, _deviceManager, _bufferManager, _result);
                    TransferDisposable(meshParameters.Filename, assetName, _fromFileResult, _result);
                    break;

                case AssetSource.NewInstance:
                    Mesh mesh = new Mesh(assetName, _bufferManager);
                    LoadedAsset loadedMesh = _result.AddAsset(assetName);
                    loadedMesh.Asset = mesh;
                    loadedMesh.Disposables.Add(mesh);
                    break;
            }

            _fromFileResult.Reset();

            return _result;
        }

        #endregion

        #region Properties

        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}