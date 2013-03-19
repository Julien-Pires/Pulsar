using System;

namespace Pulsar.Components
{
    public sealed class ComponentEventArgs : EventArgs
    {
        #region Fields

        public readonly Component Component;

        #endregion

        #region Constructors

        public ComponentEventArgs(Component compo)
        {
            this.Component = compo;
        }

        #endregion
    }
}
