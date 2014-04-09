using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class PassDefinition
    {
        #region Constructors

        internal PassDefinition(EffectPass pass, RenderState state)
        {
            Debug.Assert(pass != null);
            Debug.Assert(state != null);

            Pass = pass;
            State = state;
        }

        #endregion

        #region Properties

        public ushort Id { get; internal set; }

        public ushort Index { get; internal set; }

        internal EffectPass Pass { get; private set; }

        public string Name
        {
            get { return Pass.Name; }
        }

        public RenderState State { get; private set; }

        #endregion
    }
}
