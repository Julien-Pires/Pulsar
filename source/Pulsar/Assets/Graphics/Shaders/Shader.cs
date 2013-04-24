using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Assets.Graphics.Shaders
{
    /// <summary>
    /// Class representing a shader
    /// </summary>
    public class Shader : Asset
    {
        #region Fields

        protected Effect fx = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the Shader class
        /// </summary>
        /// <param name="owner">Creator of this instance</param>
        /// <param name="name">Name of the shader</param>
        protected internal Shader(ShaderManager owner, string name)
            : base(name)
        {
            this.assetManager = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the effect used by this Shader
        /// </summary>
        /// <param name="fx">Effect instance</param>
        protected internal virtual void SetEffect(Effect fx)
        {
            this.fx = fx;
        }

        #endregion
    }
}
