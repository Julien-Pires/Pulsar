using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.RenderingTechnique
{
    /// <summary>
    /// Shader class for simple rendering with no lights nor shadows
    /// </summary>
    internal sealed class SimpleRenderingShader : ShaderOld
    {
        #region Fields

        private const string DefaultTechniqueName = "RenderSimple";
        private const string InstancingTechniqueName = "RenderSimpleInstanced";
        private const string WorldParameter = "World";
        private const string ViewParameter = "View";
        private const string ProjectionParameter = "Projection";
        private const string DiffuseParameter = "DiffuseMap";

        private EffectParameter _world;
        private EffectParameter _projection;
        private EffectParameter _view;
        private EffectParameter _diffuse;
        private EffectTechnique _defaultTechnique;
        private EffectTechnique _instancingTechnique;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of SimpleRenderingShader class
        /// </summary>
        /// <param name="name">Name of the shader</param>
        internal SimpleRenderingShader(string name) : base(name) 
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the effect instance used by this shader
        /// </summary>
        /// <param name="fx">Effect instance</param>
        protected internal override void SetEffect(Effect fx)
        {
            base.SetEffect(fx);

            ExtractTechnique();
            ExtractParameters();
        }

        /// <summary>
        /// Apply the current effect
        /// </summary>
        public void Apply()
        {
            InternalEffect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// Set the view and projection matrix
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="proj">Projection matrix</param>
        public void SetViewProj(Matrix view, Matrix proj)
        {
            _view.SetValue(view);
            _projection.SetValue(proj);
        }

        /// <summary>
        /// Set the view and projection matrix
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="proj">Projection matrix</param>
        public void SetViewProj(ref Matrix view, ref Matrix proj)
        {
            _view.SetValue(view);
            _projection.SetValue(proj);
        }

        /// <summary>
        /// Set renderable information
        /// </summary>
        /// <param name="world">World matrix</param>
        /// <param name="material">Material instance</param>
        public void SetRenderable(Matrix world, Material material)
        {
            _world.SetValue(world);

            Texture diffuse = material.DiffuseMap;
            if (diffuse != null) 
                _diffuse.SetValue(diffuse.InternalTexture);
        }

        /// <summary>
        /// Use the default technique, one draw one renderable
        /// </summary>
        internal void UseDefaultTechnique()
        {
            InternalEffect.CurrentTechnique = _defaultTechnique;
        }

        /// <summary>
        /// Use the instancing technique, one draw multiple renderable instance
        /// </summary>
        internal void UseInstancingTechnique()
        {
            InternalEffect.CurrentTechnique = _instancingTechnique;
        }

        /// <summary>
        /// Extract technique instance from the current effect
        /// </summary>
        private void ExtractTechnique()
        {
            _defaultTechnique = InternalEffect.Techniques[DefaultTechniqueName];
            _instancingTechnique = InternalEffect.Techniques[InstancingTechniqueName];
        }

        /// <summary>
        /// Extract parameters from the current effect
        /// </summary>
        private void ExtractParameters()
        {
            _world = InternalEffect.Parameters[WorldParameter];
            _projection = InternalEffect.Parameters[ProjectionParameter];
            _view = InternalEffect.Parameters[ViewParameter];
            _diffuse = InternalEffect.Parameters[DiffuseParameter];
        }

        #endregion
    }
}
