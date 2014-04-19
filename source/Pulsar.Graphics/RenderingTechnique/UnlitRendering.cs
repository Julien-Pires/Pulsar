using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Fx;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.RenderingTechnique
{
    internal sealed partial class UnlitRendering : IRenderingTechnique
    {
        #region Fields

        private bool _isDisposed;
        private Renderer _renderer;
        private UnlitRenderQueue _renderQueue = new UnlitRenderQueue();

        #endregion

        #region Constructors

        internal UnlitRendering(Renderer renderer)
        {
            Debug.Assert(renderer != null);

            _renderer = renderer;
        }

        #endregion

        #region Methods

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

        public void Render(Viewport viewport, Camera camera, FrameContext context)
        {
            _renderer.SetRenderTarget(viewport.Target);
            if(viewport.AlwaysClear)
                _renderer.Clear(Color.CornflowerBlue);

            _renderQueue.Sort();
            List<RenderQueueElement> queueElements = _renderQueue.Renderables;
            for (int i = 0; i < queueElements.Count; i++)
            {
                IRenderable renderable = queueElements[i].Renderable;
                if(renderable.Material == null)
                    continue;

                SetMaterial(renderable.Material, context);
                Draw(renderable, context);
            }

            _renderer.UnsetRenderTarget();
            _renderQueue.Reset();
        }

        private void Draw(IRenderable renderable, FrameContext context)
        {
            context.Renderable = renderable;
            renderable.Material.Technique.InstanceConstantsBinding.UpdateAndWrite(context);

            PassBinding[] passes = renderable.Material.Technique.PassesBindings;
            for (int i = 0; i < passes.Length; i++)
            {
                PassBinding pass = passes[i];
                SwitchState(pass.RenderState, context);

                pass.Apply();
                _renderer.Draw(renderable);
            }
        }

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

        private void SetMaterial(Material material, FrameContext context)
        {
            Material previousMaterial = context.Material;
            if (material == previousMaterial) 
                return;

            context.Material = material;

            TechniqueBinding technique = material.Technique;
            technique.UseTechnique();

            ShaderConstantBindingCollection constantBindings = technique.GlobalConstantsBinding;
            if (!constantBindings.AlreadyUpdated)
            {
                constantBindings.UpdateAndWrite(context);
                constantBindings.AlreadyUpdated = true;
            }

            technique.MaterialConstantsBinding.UpdateAndWrite(context);
        }

        #endregion

        #region Properties

        public int PassCount
        {
            get { return 1; }
        }

        public IRenderQueue RenderQueue
        {
            get { return _renderQueue; }
        }

        #endregion
    }
}
