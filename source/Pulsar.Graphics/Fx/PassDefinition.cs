using System;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Describes a shader pass
    /// </summary>
    public sealed class PassDefinition
    {
        #region Fields

        private RenderState _state;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of PassDefinition
        /// </summary>
        /// <param name="pass">Underlying effect pass</param>
        internal PassDefinition(EffectPass pass)
        {
            Debug.Assert(pass != null);

            Pass = pass;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the pass
        /// </summary>
        public string Name
        {
            get { return Pass.Name; }
        }

        /// <summary>
        /// Gets the index of the pass inside the technique
        /// </summary>
        public ushort Index { get; internal set; }

        /// <summary>
        /// Gets the underlying effect pass
        /// </summary>
        internal EffectPass Pass { get; private set; }
        
        /// <summary>
        /// Gets the render state of the pass
        /// </summary>
        public RenderState State
        {
            get { return _state; }
            internal set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                _state = value;
            }
        }

        #endregion
    }
}
