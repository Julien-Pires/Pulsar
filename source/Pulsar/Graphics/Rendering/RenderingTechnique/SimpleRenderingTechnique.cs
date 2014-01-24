using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Assets;
using Pulsar.Graphics.Asset;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering.RenderingTechnique
{
    /// <summary>
    /// Class performing a simple rendering with no lights nor shadows
    /// </summary>
    internal sealed class SimpleRenderingTechnique : IRenderingTechnique
    {
        #region Fields

        private const string ShaderFile = "SimpleShader";
        private const string ShaderName = "SimpleRenderingShader";

        private readonly SimpleRenderingShader _shader;
        private readonly Renderer _renderer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SimpleRenderingTechnique class
        /// </summary>
        /// <param name="renderer">Renderer</param>
        /// <param name="graphicsStorage">Storage</param>
        internal SimpleRenderingTechnique(Renderer renderer, Storage graphicsStorage)
        {
            _renderer = renderer;

            ShaderParameters shaderParameters = new ShaderParameters
            {
                Filename = ShaderFile,
                ShaderType = typeof(SimpleRenderingShader)
            };
            _shader = graphicsStorage[GraphicsConstant.ShaderFolderName].Load<Shader>(ShaderName, shaderParameters) as SimpleRenderingShader;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the GBuffer pass
        /// </summary>
        /// <param name="vp">Target viewport for the rendering</param>
        /// <param name="queue">Render queue containing objects to draw</param>
        /// <param name="cam">Camera to use for rendering</param>
        public void Render(Viewport vp, Camera cam, RenderQueue queue)
        {
            _renderer.SetRenderTarget(vp.Target);
            if(vp.AlwaysClear) _renderer.Clear(vp.ClearColor);

            _shader.SetViewProj(cam.View, cam.Projection);
            RenderQueueGroup[] queueGroups = queue.QueueGroupList;
            for (int i = 0; i < (int)RenderQueueGroupId.Count; i++)
            {
                RenderQueueGroup group = queueGroups[i];
                if (group == null) continue;

                RenderGroup(group);
            }
            _renderer.UnsetRenderTarget();
        }

        /// <summary>
        /// Renders a group of objects
        /// </summary>
        /// <param name="group">Group of objects to render</param>
        private void RenderGroup(RenderQueueGroup group)
        {
            List<IRenderable> solids = group.SolidList;
            List<IRenderable> transparents = group.TransparentList;
            RenderObjects(solids);
            RenderObjects(transparents);

            IEnumerable<InstanceBatch> instances = _renderer.InstancingManager.GetBatchList(group.Id);
            foreach (InstanceBatch b in instances)
            {
                RenderInstancedObjects(b);
            }
        }

        /// <summary>
        /// Renders a list of objects
        /// </summary>
        /// <param name="geometries">List of renderable objects</param>
        private void RenderObjects(List<IRenderable> geometries)
        {
            InstanceBatchManager instancingMngr = _renderer.InstancingManager;
            for (int i = 0; i < geometries.Count; i++)
            {
                IRenderable geoInstance = geometries[i];
                if (!geoInstance.UseInstancing)
                {
                    _shader.UseDefaultTechnique();
                    _shader.SetRenderable(geoInstance.Transform, geoInstance.Material);
                    _shader.Apply();
                    _renderer.Draw(geoInstance);
                }
                else
                {
                    instancingMngr.AddDrawable(geoInstance);
                }
            }
        }

        private void RenderInstancedObjects(InstanceBatch batch)
        {
            _shader.UseInstancingTechnique();
            _shader.SetRenderable(Matrix.Identity, batch.Material);
            _shader.Apply();
            _renderer.DrawInstancedGeometry(batch);
        }

        #endregion
    }
}
