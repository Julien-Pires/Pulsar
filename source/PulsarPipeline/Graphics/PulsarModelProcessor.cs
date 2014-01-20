using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Pulsar.Graphics
{
    [ContentProcessor(DisplayName = "Mesh - Pulsar")]
    public class PulsarModelProcessor : ModelProcessor
    {
        #region Fields

        private static readonly string[] ExtensionSeparators = {".xnb"};

        #endregion

        #region Static methods

        private static int ComputeSubMesh(NodeContent nodeContent)
        {
            int result = 0;
            MeshContent meshContent = nodeContent as MeshContent;
            if (meshContent == null)
            {
                for (int i = 0; i < nodeContent.Children.Count; i++)
                    result += ComputeSubMesh(nodeContent.Children[i]);
            }
            else
                result += meshContent.Geometry.Count;

            return result;
        }

        private static void ComputeBoundingVolume(NodeContent nodeContent, MeshData meshData)
        {
            meshData.Reset();
            ComputeSubMeshBoundingVolume(nodeContent, meshData);

            for (int i = 0; i < meshData.SubMeshDatas.Count; i++)
            {
                meshData.Aabb = BoundingBox.CreateMerged(meshData.Aabb, meshData.SubMeshDatas[i].Aabb);
                meshData.BoundingSphere = BoundingSphere.CreateMerged(meshData.BoundingSphere,
                    meshData.SubMeshDatas[i].BoundingSphere);
            }

            meshData.Reset();
        }

        private static void ComputeSubMeshBoundingVolume(NodeContent nodeContent, MeshData meshData)
        {
            MeshContent meshContent = nodeContent as MeshContent;
            if (meshContent == null)
            {
                for (int i = 0; i < nodeContent.Children.Count; i++)
                    ComputeSubMeshBoundingVolume(nodeContent.Children[i], meshData);
            }
            else
            {
                MeshHelper.TransformScene(meshContent, meshContent.Transform);
                meshContent.Transform = Matrix.Identity;

                for (int i = 0; i < meshContent.Geometry.Count; i++)
                {
                    SubMeshData subMeshData = meshData.CurrentSubMeshData;
                    GeometryContent geometry = meshContent.Geometry[i];
                    subMeshData.Aabb = BoundingBox.CreateFromPoints(geometry.Vertices.Positions);
                    subMeshData.BoundingSphere = BoundingSphere.CreateFromPoints(geometry.Vertices.Positions);

                    meshData.MoveNext();
                }
            }
        }

        private static void FindTextureName(ModelContent model, ContentProcessorContext context, MeshData meshData)
        {
            string[] dirSeparators = { context.OutputDirectory };

            int counter = 0;
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMeshContent mesh = model.Meshes[i];
                for (int j = 0; j < mesh.MeshParts.Count; j++)
                {
                    ModelMeshPartContent part = mesh.MeshParts[j];
                    SubMeshData subData = meshData.SubMeshDatas[counter];
                    foreach (KeyValuePair<string, ExternalReference<TextureContent>> textureRef in part.Material.Textures)
                    {
                        ExternalReference<TextureContent> texture = textureRef.Value;
                        string textureName = texture.Filename.Split(dirSeparators, StringSplitOptions.RemoveEmptyEntries)[0];
                        string finalName = textureName.Split(ExtensionSeparators, StringSplitOptions.RemoveEmptyEntries)[0];
                        subData.TexturesName.Add(textureRef.Key, finalName);
                    }

                    counter++;
                }
            }
        }

        #endregion

        #region Methods

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            MeshHelper.TransformScene(input, input.Transform);
            input.Transform = Matrix.Identity;

            int subMeshCount = ComputeSubMesh(input);
            MeshData meshData = new MeshData(subMeshCount);
            ComputeBoundingVolume(input, meshData);

            ModelContent model = base.Process(input, context);
            FindTextureName(model, context, meshData);

            model.Tag = meshData;

            return model;
        }

        #endregion
    }
}