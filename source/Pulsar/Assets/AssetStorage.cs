using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

using Pulsar.Core;

namespace Pulsar.Assets
{
    /// <summary>
    /// A storage containing different type of asset
    /// </summary>
    public sealed class AssetStorage
    {
        #region Fields

        private string name = string.Empty;
        private Dictionary<string, IAssetPtr<Asset>> assetMap = new Dictionary<string, IAssetPtr<Asset>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetStorage class
        /// </summary>
        /// <param name="name">Name of the storage</param>
        internal AssetStorage(string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add an asset in the storage
        /// </summary>
        /// <param name="res">Asset to add</param>
        internal void AddAsset(IAssetPtr<Asset> res)
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
        public bool RemoveAsset(string name)
        {
            return this.assetMap.Remove(name);
        }

        /// <summary>
        /// Get an asset by its name
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <returns>Return the asset if it is contained in the storage, otherwise null</returns>
        public IAssetPtr<Asset> GetAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Asset name is null or empty");
            }

            IAssetPtr<Asset> res; 
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

        #endregion
    }

    /// <summary>
    /// Manager for asset storage, keep track of all the storage
    /// </summary>
    public sealed class AssetStorageManager : Singleton<AssetStorageManager>
    {
        #region Fields

        private ContentManager content = null;
        private AssetStorage systemStorage = new AssetStorage("SystemStorage");
        private Dictionary<string, AssetStorage> storageMap = new Dictionary<string, AssetStorage>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the AssetStorageManager class
        /// </summary>
        private AssetStorageManager()
        {
            this.content = (ContentManager)GameApplication.GameServices.GetService(typeof(ContentManager));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new asset storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Return a new storage</returns>
        public AssetStorage CreateStorage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Storage name is null or empty");
            }
            if (this.storageMap.ContainsKey(name))
            {
                throw new Exception(string.Format("Storage {0} already exists", name));
            }

            AssetStorage store = new AssetStorage(name);
            this.storageMap.Add(name, store);

            return store;
        }

        /// <summary>
        /// Clear and destroy a storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Return true if the storage is removed, otherwise false</returns>
        public bool DestroyStorage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Storage name is null or empty");
            }

            AssetStorage store;
            this.storageMap.TryGetValue(name, out store);
            if (store == null)
            {
                throw new Exception(string.Format("Failed to retrieve storage {0}", name));
            }

            return this.storageMap.Remove(name);
        }

        /// <summary>
        /// Get a storage by its name
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Return a storage</returns>
        public AssetStorage GetStorage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Storage name is null or empty");
            }

            AssetStorage store;
            this.storageMap.TryGetValue(name, out store);
            if (store == null)
            {
                throw new Exception(string.Format("Failed to retrieve storage {0}", name));
            }

            return store;
        }

        /// <summary>
        /// Add an asset in a specific storage
        /// </summary>
        /// <param name="res">Asset to add</param>
        /// <param name="storage">Storage in which the asset is stored</param>
        internal void AddResourceInStorage(IAssetPtr<Asset> res, string storage)
        {
            AssetStorage store;
            this.storageMap.TryGetValue(storage, out store);
            if (store == null)
            {
                store = this.CreateStorage(storage);
            }

            store.AddAsset(res);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the content manager used to load unmanaged asset
        /// </summary>
        public ContentManager Content
        {
            get { return this.content; }
        }

        /// <summary>
        /// Get the system asset storage
        /// </summary>
        public AssetStorage System
        {
            get { return this.systemStorage; }
        }

        #endregion
    }
}
