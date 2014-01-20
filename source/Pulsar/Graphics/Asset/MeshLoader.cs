using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;
using Pulsar.Graphics.Rendering;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Asset
{
    public sealed class MeshLoader : AssetLoader
    {
        #region Fields

        internal const string LoaderName = "MeshLoader";

        private const string DiffuseMapKey = "Texture";
        private const string NormalMapKey = "NormalMap";
        private const string SpecularMapKey = "SpecularMap";

        private readonly Type[] _supportedTypes = { typeof(Mesh) };
        private readonly BufferManager _bufferManager;
        private readonly MeshParameters _defaultParameters = new MeshParameters();
        private readonly LoadedAsset _result = new LoadedAsset();
        private readonly LoadedAsset _fromFileResult = new LoadedAsset();

        #endregion

        #region Constructors

        internal MeshLoader(GraphicsDeviceManager deviceManager, BufferManager bufferManager)
        {
            Debug.Assert(deviceManager != null);
            Debug.Assert(bufferManager != null);

            _bufferManager = bufferManager;
        }

        #endregion

        #region Static methods

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

                    string materialName = string.Format("{0}/{1}_material", mesh.Name, currentMesh.Name);
                    subMesh.Material = CreateMaterial(materialName, part.Effect, subData.TexturesName, assetFolder);
                }
            }

            mesh.UpdateBounds();
            mesh.UpdateMeshInfo();
        }

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

        private static Texture GetTextureFromFullPath(string path, AssetFolder folder)
        {
            string assetName = folder.GetNameFromFullPath(path);

            return folder.Load<Texture>(assetName);
        }

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

        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();
            _result.Name = assetName;

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
            }

            _fromFileResult.Reset();

            return _result;
        }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LoaderName; }
        }

        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}