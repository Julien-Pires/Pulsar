using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public abstract class Component
    {
        #region Fields

        private GameObject parent;

        #endregion

        #region Properties

        public GameObject Parent
        {
            get { return this.parent; }
            internal set { this.parent = value; }
        }

        #endregion
    }
}
