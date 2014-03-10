using System;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Class representing a shader
    /// </summary>
    public class ShaderOld : IDisposable
    {
        #region Fields

        protected Effect InternalEffect;

        private bool _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the Shader class
        /// </summary>
        /// <param name="name">Name of the shader</param>
        protected internal ShaderOld(string name)
        {
            Name = name;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                InternalEffect.Dispose();
            }
            finally
            {
                InternalEffect = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Set the effect used by this Shader
        /// </summary>
        /// <param name="fx">Effect instance</param>
        protected internal virtual void SetEffect(Effect fx)
        {
            InternalEffect = fx;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        #endregion
    }
}
