using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Effect used to render an object
    /// </summary>
    public class BaseEffect
    {
        #region Fields

        private const string instancingTech = "RenderToGBufferInstanced";

        private Effect effect = null;
        private EffectParameter viewParam = null;
        private EffectParameter projectionParam = null;
        private EffectParameter worldParam = null;
        private EffectParameter diffuseParam = null;
        private bool isSolid = true;
        private bool isTransparent = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the BaseEffect class
        /// </summary>
        /// <param name="fx">Effect used by this instance</param>
        public BaseEffect(Effect fx)
        {
            this.SetEffect(fx);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the world, view and projection matrix for this effect
        /// </summary>
        /// <param name="world">World matrix</param>
        /// <param name="view">View matrix</param>
        /// <param name="projection">Projection matrix</param>
        public void SetMatrix(Matrix world, Matrix view, Matrix projection)
        {
            this.worldParam.SetValue(world);
            this.viewParam.SetValue(view);
            this.projectionParam.SetValue(projection);
        }

        /// <summary>
        /// Set the current technique specified by an index
        /// </summary>
        /// <param name="current">The index used to specified the technique to use</param>
        public void SetCurrentTechnique(int current)
        {
            this.effect.CurrentTechnique = this.effect.Techniques[current];
        }

        /// <summary>
        /// Set the current technique to do instanced drawing
        /// </summary>
        public void SetInstancingTechnique()
        {
            this.effect.CurrentTechnique = this.effect.Techniques[BaseEffect.instancingTech];
        }

        /// <summary>
        /// Apply the current pass for the current technique
        /// </summary>
        public void Apply()
        {
            this.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// Get the parameter for this effect
        /// </summary>
        private void ExtractParameters()
        {
            this.viewParam = this.effect.Parameters["View"];
            this.projectionParam = this.effect.Parameters["Projection"];
            this.worldParam = this.effect.Parameters["World"];
            this.diffuseParam = this.effect.Parameters["DiffuseMap"];
        }

        /// <summary>
        /// Bind an effect to use
        /// </summary>
        /// <param name="fx">Effect used by this instance</param>
        private void SetEffect(Effect fx)
        {
            this.effect = fx;
            this.ExtractParameters();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set a value indicating that renderable linked to this effect is opaque
        /// </summary>
        public bool IsSolid
        {
            get { return this.isSolid; }
            set
            {
                this.isSolid = value;
                this.isTransparent = !value;
            }
        }

        /// <summary>
        /// Get or set a value indicating that renderable linked to this effect is transparent
        /// </summary>
        public bool IsTransparent
        {
            get { return this.isTransparent; }
            set
            {
                this.isTransparent = value;
                this.isSolid = !value;
            }
        }

        #endregion
    }
}
