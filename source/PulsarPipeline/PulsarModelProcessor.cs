using System;
using System.Text.RegularExpressions;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using PulsarRuntime.Graphics;

namespace Pulsar
{

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ModelBoundingData
    {
        #region Fields

        private List<BoundingData> subBoundings = new List<BoundingData>();

        #endregion

        #region Methods

        public void AddSubBounding(BoundingData bounds)
        {
            this.subBoundings.Add(bounds);
        }

        #endregion

        #region Properties

        public List<BoundingData> SubBounding
        {
            get { return this.subBoundings; }
        }

        public BoundingData ModelBounding { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [ContentProcessor(DisplayName = "Model - Pulsar")]
    public class PulsarModelProcessor : ModelProcessor
    {
        #region Fields

        private const string extension = ".xnb";

        #endregion

        #region Methods

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            MeshHelper.TransformScene(input, input.Transform);
            input.Transform = Matrix.Identity;
            
            ModelContent model = base.Process(input, context);
            this.CreateMeshData(model);
            this.FindTextureName(model, context);

            ModelBoundingData boundingContainer = new ModelBoundingData();
            this.ComputeBoundingVolume(input, boundingContainer);
            this.BoundingDataToMeshData((MeshData)model.Tag, boundingContainer);

            return model;
        }

        private void CreateMeshData(ModelContent model)
        {
            MeshData data = new MeshData();
            foreach (ModelMeshContent mesh in model.Meshes)
            {
                foreach (ModelMeshPartContent part in mesh.MeshParts)
                {
                    data.SubMeshData.Add(new SubMeshData());
                }
            }

            model.Tag = data;
        }

        private void BoundingDataToMeshData(MeshData data, ModelBoundingData bounds)
        {
            List<BoundingData> subBounds = bounds.SubBounding;
            List<SubMeshData> subData = data.SubMeshData;
            for (int i = 0; i < subData.Count; i++)
            {
                subData[i].BoundingVolume = subBounds[i];
            }
            data.BoundingVolume = bounds.ModelBounding;
        }

        private void ComputeBoundingVolume(NodeContent input, ModelBoundingData modelBounding)
        {
            this.ComputeChildBoundingVolume(input, modelBounding);

            List<BoundingData> subs = modelBounding.SubBounding;
            BoundingSphere modelS = new BoundingSphere();
            BoundingBox modelB = new BoundingBox();

            for (int i = 0; i < subs.Count; i++)
            {
                BoundingData bound = subs[i];

                modelS = BoundingSphere.CreateMerged(modelS, bound.BoundingSphere);
                modelB = BoundingBox.CreateMerged(modelB, bound.BoundingBox);
            }

            BoundingData modelBounds = new BoundingData();

            modelBounds.BoundingSphere = modelS;
            modelBounds.BoundingBox = modelB;
            modelBounding.ModelBounding = modelBounds;
        }

        private void ComputeChildBoundingVolume(NodeContent input, ModelBoundingData modelBounding)
        {
            BoundingSphere s;
            BoundingBox b;
            MeshContent mesh = input as MeshContent;

            if (mesh != null)
            {
                MeshHelper.TransformScene(mesh, mesh.Transform);
                mesh.Transform = Matrix.Identity;

                if (mesh.Geometry.Count > 0)
                {
                    for (int i = 0; i < mesh.Geometry.Count; i++)
                    {
                        GeometryContent geo = mesh.Geometry[i];
                        s = BoundingSphere.CreateFromPoints(geo.Vertices.Positions);
                        b = BoundingBox.CreateFromPoints(geo.Vertices.Positions);

                        BoundingData subBound = new BoundingData();
                        subBound.BoundingBox = b;
                        subBound.BoundingSphere = s;
                        modelBounding.AddSubBounding(subBound);
                    }
                }
                else
                {
                    s = BoundingSphere.CreateFromPoints(mesh.Positions);
                    b = BoundingBox.CreateFromPoints(mesh.Positions);

                    BoundingData subBound = new BoundingData();
                    subBound.BoundingBox = b;
                    subBound.BoundingSphere = s;
                    modelBounding.AddSubBounding(subBound);
                }
            }

            foreach (NodeContent n in input.Children)
            {
                this.ComputeChildBoundingVolume(n, modelBounding);
            }
        }

        private void FindTextureName(ModelContent model, ContentProcessorContext context)
        {
            MeshData meshData = (MeshData)model.Tag;
            string[] dirSeparators = { context.OutputDirectory };
            string[] extSeparators = { extension };

            int counter = 0;
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMeshContent mesh = model.Meshes[i];
                for (int j = 0; j < mesh.MeshParts.Count; j++)
                {
                    ModelMeshPartContent part = mesh.MeshParts[j];
                    SubMeshData subData = meshData.SubMeshData[counter];
                    foreach (var texRef in part.Material.Textures)
                    {
                        ExternalReference<TextureContent> texture = texRef.Value;
                        string[] texName = texture.Filename.Split(dirSeparators, StringSplitOptions.RemoveEmptyEntries);
                        string finalName = texName[0].Split(extSeparators, StringSplitOptions.RemoveEmptyEntries)[0];
                        subData.TexturesName.Add(texRef.Key, finalName);
                    }

                    counter++;
                }
            }
        }

        #endregion
    }
}