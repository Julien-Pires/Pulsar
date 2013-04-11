using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;
using Pulsar.Assets.Graphics.Shaders;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering.RenderPass
{
    /// <summary>
    /// Class performing a GBuffer rendering pass
    /// </summary>
    internal sealed class GBufferPass
    {
        #region Fields

        private GBufferShader shader = null;
        private Renderer renderer = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GBufferPass class
        /// </summary>
        /// <param name="renderer"></param>
        internal GBufferPass(Renderer renderer)
        {
            this.renderer = renderer;
            this.shader = (GBufferShader)ShaderManager.Instance.LoadShader("GBufferPass", 
                AssetStorageManager.Instance.System.Name, GBufferShader.ShaderFile, typeof(GBufferShader));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Render the GBuffer pass
        /// </summary>
        /// <param name="queue">Render queue containing objects to draw</param>
        /// <param name="cam">Camera to use for rendering</param>
        internal void Render(RenderQueue queue, Camera cam)
        {
            this.shader.SetViewProj(cam.ViewTransform, cam.ProjectionTransform);

            RenderQueueGroup[] queueGroups = queue.QueueGroupList;
            for (int i = 0; i < (int)RenderQueueGroupID.Count; i++)
            {
                RenderQueueGroup group = queueGroups[i];
                if (group == null)
                    continue;

                this.RenderGroup(group);
            }
        }

        /// <summary>
        /// Render a group of objects
        /// </summary>
        /// <param name="group">Group of objects to render</param>
        private void RenderGroup(RenderQueueGroup group)
        {
            List<IRenderable> solids = group.SolidList;
            List<IRenderable> transparents = group.TransparentList;

            this.RenderObjects(solids);
            this.RenderObjects(transparents);
        }

        /// <summary>
        /// Render a list of objects
        /// </summary>
        /// <param name="geometries">List of renderable objects</param>
        private void RenderObjects(List<IRenderable> geometries)
        {
            for (int i = 0; i < geometries.Count; i++)
            {
                IRenderable geoInstance = geometries[i];
                GeometryBatch instancingBatch = geoInstance as GeometryBatch;
                if (instancingBatch == null)
                {
                    this.shader.UseDefaultTechnique();
                    this.shader.SetRenderable(geoInstance.Transform, geoInstance.Material);
                    this.shader.Apply();
                    this.renderer.RenderGeometry(geoInstance);
                }
                else
                {
                    this.shader.UseInstancingTechnique();
                    this.shader.SetRenderable(Matrix.Identity, instancingBatch.Material);
                    this.shader.Apply();
                    this.renderer.RenderInstancedGeometry(instancingBatch);
                }
            }
        }

        #endregion
    }
}
