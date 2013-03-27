using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    /// <summary>
    /// Struct containing the result of a search in an asset storage
    /// </summary>
    /// <typeparam name="T">Type of asset contained in this result</typeparam>
    public struct AssetSearchResult<T>
    {
        #region Fields

        /// <summary>
        /// Asset found
        /// </summary>
        public readonly T Resource;

        /// <summary>
        /// Boolean indicating if the asset is a new one or an existing
        /// </summary>
        public readonly bool Created;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetSearchResult struct
        /// </summary>
        /// <param name="res">Asset found</param>
        /// <param name="create">Define if the asset is a fresh new instance or an existing</param>
        internal AssetSearchResult(T res, bool create)
        {
            this.Resource = res;
            this.Created = create;
        }

        #endregion
    }

    /// <summary>
    /// Represents a group of asset (eg: Texture, Material, Mesh, ...)
    /// An asset group keep track of all the asset for a specific type of asset
    /// and across all storage
    /// </summary>
    /// <typeparam name="T">Type of asset managed by the AssetGroup</typeparam>
    public sealed class AssetGroup<T> where T : Asset
    {
        #region Fields

        private string name;
        private IAssetManager concreteManager = null;
        private Dictionary<string, Dictionary<string, T>> assetStorageMap =
            new Dictionary<string, Dictionary<string, T>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetGroup class
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="manager">Asset manager for a specific type of asset</param>
        public AssetGroup(string name, IAssetManager manager)
        {
            this.name = name;
            this.concreteManager = manager;

            AssetStorageManager.Instance.StorageCreated += this.OnStorageCreated;
            AssetStorageManager.Instance.StorageDestroyed += this.OnStorageDestroyed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load an asset
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storage in which the asset will be stored</param>
        /// <param name="parameter">Additional parameter for the asset creation</param>
        /// <returns>Return an existing asset if the storage has one, otherwise a new</returns>
        public AssetSearchResult<T> Load(string name, string storage, object parameter = null)
        {
            AssetSearchResult<T> res = this.CreateOrFind(name, storage, parameter);

            return res;
        }

        public bool Unload(string name, string storage)
        {
            bool result = AssetStorageManager.Instance.RemoveResourceInStorage(name, storage);
            if (result)
            {
                Dictionary<string, T> storageMap;
                this.assetStorageMap.TryGetValue(storage, out storageMap);
                if (storageMap == null)
                {
                    throw new Exception(string.Format("No storage {0} found in this asset group", name));
                }
                storageMap.Remove(name);
            }

            return result;
        }

        /// <summary>
        /// Create or find an asset in a storage
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storae in which the asset will be stored</param>
        /// <param name="parameter">Additional parameter for the asset creation</param>
        /// <returns>Return an existing asset if the storage has one, otherwise a new</returns>
        private AssetSearchResult<T> CreateOrFind(string name, string storage, object parameter = null)
        {
            AssetSearchResult<T> result;
            bool create = false;

            T res = this.GetByName(name, storage);
            if (res == null)
            {
                res = this.Create(name, storage, parameter);
                create = true;
            }

            result = new AssetSearchResult<T>(res, create);

            return result;
        }

        /// <summary>
        /// Create a new asset
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storage in which the asset will be stored</param>
        /// <param name="parameter">Addition parameter for the asset creation</param>
        /// <returns>Return a new asset</returns>
        private T Create(string name, string storage, object parameter = null)
        {
            T res = this.concreteManager.CreateInstance(name, parameter) as T;
            if (res == null)
            {
                throw new Exception(string.Format("CreateInstance method doesn't return a {0} instance", typeof(T)));
            }

            this.AddIntern(res, storage);
            AssetStorageManager.Instance.AddResourceInStorage(res, storage);

            return res;
        }

        /// <summary>
        /// Add an asset
        /// </summary>
        /// <param name="res">Asset to add</param>
        /// <param name="storage">Storage in which the asset will be stored</param>
        private void AddIntern(T res, string storage)
        {
            Dictionary<string, T> map = null;
            this.assetStorageMap.TryGetValue(storage, out map);
            if (map == null)
            {
                map = new Dictionary<string, T>();
                this.assetStorageMap.Add(storage, map);
            }

            map.Add(res.Name, res);
        }

        /// <summary>
        /// Get an asset by its name
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storage in which to look for</param>
        /// <returns>Return an asset if the storage contains one withe the same name, otherwise null</returns>
        private T GetByName(string name, string storage)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Asset name is null or empty");
            }
            Dictionary<string, T> storageMap;
            this.assetStorageMap.TryGetValue(storage, out storageMap);
            if (storageMap == null)
            {
                return null;
            }

            T res = null;
            storageMap.TryGetValue(name, out res);

            return res;
        }

        private void OnStorageCreated(object sender, AssetStorageEventArgs e)
        {
            AssetStorage storage = e.Storage;
            if (this.assetStorageMap.ContainsKey(storage.Name))
            {
                return;
            }

            Dictionary<string, T> storageMap = new Dictionary<string, T>();
            this.assetStorageMap.Add(storage.Name, storageMap);
        }

        private void OnStorageDestroyed(object sender, AssetStorageEventArgs e)
        {
            AssetStorage storage = e.Storage;
            Dictionary<string, T> assetMap;
            this.assetStorageMap.TryGetValue(storage.Name, out assetMap);
            if (assetMap == null)
            {
                return;
            }

            assetMap.Clear();
            this.assetStorageMap.Remove(storage.Name);
        }

        #endregion
    }
}
