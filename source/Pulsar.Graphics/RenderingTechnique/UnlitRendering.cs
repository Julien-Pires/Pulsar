using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Fx;
using Pulsar.Graphics.Graph;

namespace Pulsar.Graphics.RenderingTechnique
{
    /// <summary>
    /// Represents a basic rendering technique with no light no shadows
    /// </summary>
    internal sealed partial class UnlitRendering : IRenderingTechnique
    {
        #region Fields

        private bool _isDisposed;
        private Renderer _renderer;
        private UnlitRenderQueue _renderQueue = new UnlitRenderQueue();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructors of UnlitRendering class
        /// </summary>
        /// <param name="renderer">Renderer</param>
        internal UnlitRendering(Renderer renderer)
        {
            Debug.Assert(renderer != null);

            _renderer = renderer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed)
                return;

            try
            {
                _renderQueue.Dispose();
            }
            finally
            {
                _renderQueue = null;
                _renderer = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Renders a scene from its own render queue
        /// </summary>
        /// <param name="viewport">Target viewport</param>
        /// <param name="camera">Point of view</param>
        /// <param name="context">Frame context</param>
        public void Render(Viewport viewport, Camera camera, FrameContext context)
        {
            _renderer.SetRenderTarget(viewport.Target);
            if(viewport.AlwaysClear)
                _renderer.Clear(Color.CornflowerBlue);

            foreach (Shader shader in _renderQueue.UsedShaders.Values)
                shader.GlobalConstantsBinding.UpdateAndWrite(context);

            _renderQueue.Sort();
            List<RenderQueueElement> queueElements = _renderQueue.Renderables;
            for (int i = 0; i < queueElements.Count; i++)
            {
                IRenderable renderable = queueElements[i].Renderable;
                Material material = renderable.Material;
                if (material == null)
                    continue;

                context.Renderable = renderable;

                Material previousMaterial = context.Material;
                TechniqueBinding technique = material.Technique;
                if (material != previousMaterial)
                {
                    context.Material = material;
                    technique.UseTechnique();
                    technique.MaterialConstantsBinding.UpdateAndWrite(context);
                }
                technique.InstanceConstantsBinding.UpdateAndWrite(context);

                PassBinding[] passes = technique.PassesBindings;
                for (int j = 0; j < passes.Length; j++)
                {
                    PassBinding pass = passes[j];
                    SwitchState(pass.RenderState, context);

                    pass.Apply();
                    _renderer.Draw(renderable);
                }
            }

            _renderer.UnsetRenderTarget();
            _renderQueue.Reset();
        }

        /// <summary>
        /// Changes the render state
        /// </summary>
        /// <param name="state">New render state</param>
        /// <param name="context">Frame context</param>
        private void SwitchState(RenderState state, FrameContext context)
        {
            if(state == context.RenderState)
                return;

            context.RenderState = state;

            DepthStencilState previousDepthStencil = context.DepthStencilState;
            DepthStencilState newDepthStencil = state.DepthStencil;
            if (newDepthStencil != previousDepthStencil)
            {
                _renderer.SetDepthStencilState(newDepthStencil);
                context.DepthStencilState = newDepthStencil;
            }

            RasterizerState previousRasterizerState = context.RasterizerState;
            RasterizerState newRasterizerState = state.Rasterizer;
            if (newRasterizerState != previousRasterizerState)
            {
                _renderer.SetRasterizerState(newRasterizerState);
                context.RasterizerState = newRasterizerState;
            }

            BlendState previousBlendState = context.BlendState;
            BlendState newBlendState = state.Blend;
            if (newBlendState != previousBlendState)
            {
                _renderer.SetBlendState(newBlendState);
                context.BlendState = newBlendState;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of pass for this rendering technique
        /// </summary>
        public int PassCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the render queue used by this rendering technique
        /// </summary>
        public IRenderQueue RenderQueue
        {
            get { return _renderQueue; }
        }

        #endregion
    }
}
