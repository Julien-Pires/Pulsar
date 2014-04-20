using System;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class PassDefinition
    {
        #region Fields

        private RenderState _state;

        #endregion

        #region Constructors

        internal PassDefinition(EffectPass pass)
        {
            Debug.Assert(pass != null);

            Pass = pass;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return Pass.Name; }
        }

        public ushort Index { get; internal set; }

        internal EffectPass Pass { get; private set; }
        
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
