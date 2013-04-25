using System;

namespace Pulsar.Components
{
    /// <summary>
    /// Event arguments that are used for all component events
    /// </summary>
    public sealed class ComponentEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Component concerned by the event
        /// </summary>
        public readonly Component Component;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ComponentEventArgs class
        /// </summary>
        /// <param name="compo"></param>
        public ComponentEventArgs(Component compo)
        {
            this.Component = compo;
        }

        #endregion
    }
}
