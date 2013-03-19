using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public sealed class GameObject : ComponentCollection
    {
        #region Fields

        private ulong id;

        #endregion

        #region Constructor

        public GameObject(ulong id)
        {
            this.id = id;
        }

        #endregion

        #region Methods

        public override void AddComponent(Component compo, bool overwrite)
        {
            base.AddComponent(compo, overwrite);

            compo.Parent = this;
        }

        #endregion

        #region Properties

        public ulong ID
        {
            get { return this.id; }
        }

        #endregion
    }
}
