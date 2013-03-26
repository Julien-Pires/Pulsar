using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

using Pulsar.Core;

namespace Pulsar.Assets
{
    /// <summary>
    /// Manager for asset storage, keep track of all the storage
    /// </summary>
    public sealed class AssetStorageManager : Singleton<AssetStorageManager>
    {
        #region Fields

        private IServiceProvider services;
        private AssetStorage systemStorage;
        private Dictionary<string, AssetStorage> storageMap = new Dictionary<string, AssetStorage>();

        #endregion

        #region Events

        public event EventHandler<AssetStorageEventArgs> StorageCreated;
        public event EventHandler<AssetStorageEventArgs> StorageDestroyed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the AssetStorageManager class
        /// </summary>
        private AssetStorageManager()
        {
            this.services = GameApplication.GameServices;
            this.systemStorage = this.CreateStorage("System_Storage", "Content");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new asset storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Return a new storage</returns>
        public AssetStorage CreateStorage(string name, string path)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Storage name is null or empty");
            }
            if (this.storageMap.ContainsKey(name))
            {
                throw new Exception(string.Format("Storage {0} already exists", name));
            }

            AssetStorage store = new AssetStorage(name, path, this.services);
            this.storageMap.Add(name, store);
            if (this.StorageCreated != null)
            {
                this.StorageCreated(this, new AssetStorageEventArgs(store));
            }

            return store;
        }

        /// <summary>
        /// Clear and destroy a storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Return true if the storage is removed, otherwise false</returns>
        public bool DestroyStorage(string name)
        {
            AssetStorage store;
            this.storageMap.TryGetValue(name, out store);
            if (store == null)
            {
                throw new Exception(string.Format("Failed to retrieve storage {0}", name));
            }

            bool result = this.storageMap.Remove(name);
            if (result)
            {
                if (this.StorageDestroyed != null)
                {
                    this.StorageDestroyed(this, new AssetStorageEventArgs(store));
                }
                store.Dispose();
            }

            return result;
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
        internal void AddResourceInStorage(Asset res, string storage)
        {
            AssetStorage store;
            this.storageMap.TryGetValue(storage, out store);
            if (store == null)
            {
                throw new Exception(string.Format("No storage with the name {0} exists", storage));
            }

            store.AddAsset(res);
        }

        internal bool RemoveResourceInStorage(string name, string storage)
        {
            AssetStorage store;
            this.storageMap.TryGetValue(storage, out store);
            if (store == null)
            {
                throw new Exception(string.Format("No storage with the name {0} exists", storage));
            }

            return store.RemoveAsset(storage);
        }

        #endregion

        #region Properties

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
