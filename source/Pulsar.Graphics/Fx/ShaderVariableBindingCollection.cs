using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    internal sealed class ShaderVariableBindingCollection : List<ShaderVariableBinding>
    {
        #region Constructors

        internal ShaderVariableBindingCollection()
        {
        }

        internal ShaderVariableBindingCollection(int capacity) : base(capacity)
        {
        }

        #endregion

        #region Methods

        internal void Update(FrameContext context)
        {
            for(int i = 0; i < Count; i++)
                this[i].Update(context);
        }

        internal void Write()
        {
            for(int i = 0; i < Count; i++)
                this[i].Write();
        }

        #endregion
    }
}
