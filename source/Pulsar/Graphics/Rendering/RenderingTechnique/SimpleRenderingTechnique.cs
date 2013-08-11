using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Assets;
using Pulsar.Assets.Graphics.Shaders;
using Pulsar.Graphics.SceneGraph;


namespace Pulsar.Graphics.Rendering.RenderingTechnique
{
    /// <summary>
    /// Class performing a simple rendering with no lights nor shadows
    /// </summary>
    internal sealed class SimpleRenderingTechnique : IRenderingTechnique
    {
        #region Fields

        private const string ShaderFile = "Shaders/SimpleShader";
        private const string ShaderName = "SimpleRenderingShader";

        private readonly SimpleRenderingShader _shader;
        private readonly Renderer _renderer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SimpleRenderingTechnique class
        /// </summary>
        /// <param name="renderer"></param>
        internal SimpleRenderingTechnique(Renderer renderer)
        {
            _renderer = renderer;
            _shader = (SimpleRenderingShader)ShaderManager.Instance.LoadShader(ShaderName, 
                AssetStorageManager.Instance.System.Name, ShaderFile, typeof(SimpleRenderingShader));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Render the GBuffer pass
        /// </summary>
        /// <param name="vp">Target viewport for the rendering</param>
        /// <param name="queue">Render queue containing objects to draw</param>
        /// <param name="cam">Camera to use for rendering</param>
        public void Render(Viewport vp, Camera cam, RenderQueue queue)
        {
            _renderer.SetRenderTarget(vp.RenderTarget);
            _shader.SetViewProj(ref cam.ViewMatrix, ref cam.ProjectionMatrix);

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
        /// Render a group of objects
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
        /// Render a list of objects
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
                    _renderer.DrawGeometry(geoInstance);
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
