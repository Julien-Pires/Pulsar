using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    /// <summary>
    /// A component defines game object behaviors
    /// </summary>
    public abstract class Component
    {
        #region Fields

        private GameObject parent;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the owner of this component
        /// </summary>
        public GameObject Parent
        {
            get { return this.parent; }
            internal set { this.parent = value; }
        }

        #endregion
    }
}
