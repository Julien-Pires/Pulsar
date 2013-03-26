using System;
using System.Text;

using System.Collections.Generic;

namespace Pulsar.Assets
{
    /// <summary>
    /// Base class for all resources
    /// </summary>
    public abstract class Asset
    {
        #region Fields

        private static long uniqueID = long.MinValue;

        protected IAssetManager assetManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Asset class
        /// </summary>
        /// <param name="name"></param>
        protected Asset(string name)
        {
            this.ID = Asset.uniqueID;
            Asset.uniqueID++;
            this.Name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reload the asset and all it's resources
        /// </summary>
        internal virtual void Reload()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the asset ID
        /// </summary>
        public long ID { get; protected set; }

        /// <summary>
        /// Get the name of the asset
        /// </summary>
        public string Name { get; internal set; }

        #endregion
    }
}
