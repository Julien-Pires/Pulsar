using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

using Pulsar.Core;

namespace Pulsar.Assets
{
    /// <summary>
    /// A storage containing different type of asset
    /// </summary>
    public sealed class AssetStorage : IDisposable
    {
        #region Fields

        private string name;
        private string resourceDirectory;
        private Dictionary<string, Asset> assetMap = new Dictionary<string, Asset>();
        private ContentManager resourceManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetStorage class
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <param name="path">Directory containg assets</param>
        /// <param name="services">Services provider</param>
        internal AssetStorage(string name, string path, IServiceProvider services)
        {
            this.name = name;
            this.resourceDirectory = path;
            this.resourceManager = new ContentManager(services, this.resourceDirectory);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete all ressources hold by this storage
        /// </summary>
        public void Dispose()
        {
            this.assetMap.Clear();
            this.assetMap = null;
            this.resourceManager.Dispose();
            this.resourceManager = null;
        }

        /// <summary>
        /// Add an asset in the storage
        /// </summary>
        /// <param name="res">Asset to add</param>
        internal void AddAsset(Asset res)
        {
            if (this.assetMap.ContainsKey(res.Name))
            {
                throw new Exception(string.Format("Asset {0} already exists in storage {1}", res.Name, this.name));
            }

            this.assetMap.Add(res.Name, res);
        }

        /// <summary>
        /// Remove an asset from the storage
        /// </summary>
        /// <param name="name">Name of the asset to remove</param>
        /// <returns>Return true if the asset is removed, otherwise false</returns>
        internal bool RemoveAsset(string name)
        {
            return this.assetMap.Remove(name);
        }

        /// <summary>
        /// Get an asset by its name
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <returns>Return the asset if it is contained in the storage, otherwise null</returns>
        public Asset GetAsset(string name)
        {
            Asset res; 
            this.assetMap.TryGetValue(name, out res);
            if (res == null)
            {
                throw new Exception(string.Format("Failed to retrieve asset {0}", res.Name));
            }

            return res;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of the storage
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Get the ContentManager used by this storage
        /// </summary>
        internal ContentManager ResourceManager
        {
            get { return this.resourceManager; }
        }

        #endregion
    }
}
