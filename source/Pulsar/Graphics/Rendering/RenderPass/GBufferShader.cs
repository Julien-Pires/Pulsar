using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Shaders;
using Pulsar.Assets.Graphics.Materials;

using PangoTexture = Pulsar.Assets.Graphics.Materials.Texture;

namespace Pulsar.Graphics.Rendering.RenderPass
{
    /// <summary>
    /// Shader class for GBuffer rendering
    /// </summary>
    internal sealed class GBufferShader : Shader
    {
        #region Fields

        public const string ShaderFile = "Shaders/defaultShader";
        private const string defaultTechniqueName = "RenderToGBuffer";
        private const string instancingTechniqueName = "RenderToGBufferInstanced";

        private EffectParameter world = null;
        private EffectParameter projection = null;
        private EffectParameter view = null;
        private EffectParameter diffuse = null;
        private EffectTechnique defaultTechnique = null;
        private EffectTechnique instancingTechnique = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GBufferShader class
        /// </summary>
        /// <param name="owner">Manager which creates this instance</param>
        /// <param name="name">Name of the shader</param>
        internal GBufferShader(ShaderManager owner, string name)
            : base(owner, name) 
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the effect instance used by this shader
        /// </summary>
        /// <param name="fx">Effect instance</param>
        protected internal override void SetEffect(Microsoft.Xna.Framework.Graphics.Effect fx)
        {
            base.SetEffect(fx);
            this.ExtractTechnique();
            this.ExtractParameters();
        }

        /// <summary>
        /// Apply the current effect
        /// </summary>
        public void Apply()
        {
            this.fx.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// Set the view and projection matrix
        /// </summary>
        /// <param name="view">View matrix</param>
        /// <param name="proj">Projection matrix</param>
        public void SetViewProj(Matrix view, Matrix proj)
        {
            this.view.SetValue(view);
            this.projection.SetValue(proj);
        }

        /// <summary>
        /// Set renderable information
        /// </summary>
        /// <param name="world">World matrix</param>
        /// <param name="material">Material instance</param>
        public void SetRenderable(Matrix world, Material material)
        {
            this.world.SetValue(world);

            PangoTexture diffuse = material.DiffuseMap;
            if (diffuse != null)
            {
                this.diffuse.SetValue(diffuse.Image);
            }
        }

        /// <summary>
        /// Use the default technique, one draw one renderable
        /// </summary>
        internal void UseDefaultTechnique()
        {
            this.fx.CurrentTechnique = this.defaultTechnique;
        }

        /// <summary>
        /// Use the instancing technique, one draw multiple renderable instance
        /// </summary>
        internal void UseInstancingTechnique()
        {
            this.fx.CurrentTechnique = this.instancingTechnique;
        }

        /// <summary>
        /// Extract technique instance from the current effect
        /// </summary>
        private void ExtractTechnique()
        {
            this.defaultTechnique = this.fx.Techniques[GBufferShader.defaultTechniqueName];
            this.instancingTechnique = this.fx.Techniques[GBufferShader.instancingTechniqueName];
        }

        /// <summary>
        /// Extract parameters from the current effect
        /// </summary>
        private void ExtractParameters()
        {
            this.world = this.fx.Parameters["World"];
            this.projection = this.fx.Parameters["Projection"];
            this.view = this.fx.Parameters["View"];
            this.diffuse = this.fx.Parameters["DiffuseMap"];
        }

        #endregion
    }
}
