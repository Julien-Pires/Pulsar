using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    public sealed class GameObject : ComponentCollection
    {
        #region Fields

        private ulong id;
        private GameObjectManager owner;

        #endregion

        #region Constructor

        public GameObject(ulong id, GameObjectManager owner)
        {
            this.id = id;
            this.owner = owner;
        }

        #endregion

        #region Methods

        public override void Add(Component compo, bool overwrite)
        {
            base.Add(compo, overwrite);

            compo.Parent = this;
        }

        #endregion

        #region Properties

        public ulong ID
        {
            get { return this.id; }
        }

        public GameObjectManager Owner
        {
            get { return this.owner; }
        }

        #endregion
    }
}
