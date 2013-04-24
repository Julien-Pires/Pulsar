using System;

using System.Collections.Generic;

namespace Pulsar.Components
{
    /// <summary>
    /// Represents a game object.
    /// A GameObject is composed of component.
    /// </summary>
    public sealed class GameObject : ComponentCollection
    {
        #region Fields

        private ulong id;
        private GameObjectManager owner;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of GameObject class
        /// </summary>
        /// <param name="id">Id of the game object</param>
        public GameObject(ulong id)
        {
            this.id = id;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a component
        /// </summary>
        /// <param name="compo">Component to add</param>
        /// <param name="overwrite">Indicates to remove a component if this collection already contains one of the same type</param>
        public override void Add(Component compo, bool overwrite)
        {
            base.Add(compo, overwrite);

            compo.Parent = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the Id
        /// </summary>
        public ulong ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Get the owner of this game object
        /// </summary>
        public GameObjectManager Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        #endregion
    }
}
