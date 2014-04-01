using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed class ShaderPassDefinition
    {
        #region Fields

        private static readonly IndexPool IndexPool = new IndexPool();

        private readonly ushort _id;

        #endregion

        #region Constructors

        internal ShaderPassDefinition(string name, EffectPass pass, RenderState state)
        {
            Debug.Assert(pass != null);
            Debug.Assert(state != null);

            Name = name;
            Pass = pass;
            State = state;
            _id = (ushort)IndexPool.Get();
        }

        #endregion

        #region Properties

        public ushort Id
        {
            get { return _id; }
        }

        internal EffectPass Pass { get; private set; }

        public string Name { get; private set; }

        public RenderState State { get; private set; }

        #endregion
    }
}
