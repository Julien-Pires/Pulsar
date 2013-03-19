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
    /// Interface defining a group of asset
    /// </summary>
    /// <typeparam name="T">Type of asset managed by the AssetGroup</typeparam>
    public interface IAssetGroup<out T> where T : Asset
    {
    }

    /// <summary>
    /// Represents a group of asset (eg: Texture, Material, Mesh, ...)
    /// An asset group keep track of all the asset for a specific type of asset
    /// and across all storage
    /// </summary>
    /// <typeparam name="T">Type of asset managed by the AssetGroup</typeparam>
    public sealed class AssetGroup<T> : IAssetGroup<T> where T : Asset
    {
        #region Fields

        public const string ContentDirectory = "content\\";

        protected string group;
        protected IAssetManager<T> concreteManager = null;
        protected ContentManager content = null;
        protected Dictionary<string, AssetPtr<T>> assetMap = new Dictionary<string, AssetPtr<T>>();
        protected Dictionary<string, Dictionary<string, AssetPtr<T>>> assetStorageMap =
            new Dictionary<string, Dictionary<string, AssetPtr<T>>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetGroup class
        /// </summary>
        /// <param name="name">Name of the group</param>
        /// <param name="manager">Asset manager for a specific type of asset</param>
        public AssetGroup(string name, IAssetManager<T> manager)
        {
            this.group = name;
            this.concreteManager = manager;
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

            AssetPtr<T> resPtr = this.GetByName(name, storage);
            if (resPtr == null)
            {
                resPtr = this.Create(name, storage, parameter);
                create = true;
            }

            result = new AssetSearchResult<T>(resPtr.Resource, create);

            return result;
        }

        /// <summary>
        /// Create a new asset
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storage in which the asset will be stored</param>
        /// <param name="parameter">Addition parameter for the asset creation</param>
        /// <returns>Return a new asset</returns>
        private AssetPtr<T> Create(string name, string storage, object parameter = null)
        {
            AssetPtr<T> resPtr = new AssetPtr<T>(
                this.concreteManager.CreateInstance(name, parameter)
            );

            this.AddIntern(resPtr, storage);
            AssetStorageManager.Instance.AddResourceInStorage(resPtr, storage);
            resPtr.AddRef();

            return resPtr;
        }

        /// <summary>
        /// Add an asset
        /// </summary>
        /// <param name="res">Asset to add</param>
        /// <param name="storage">Storage in which the asset will be stored</param>
        private void AddIntern(AssetPtr<T> res, string storage)
        {
            Dictionary<string, AssetPtr<T>> map = null;
            this.assetStorageMap.TryGetValue(storage, out map);
            if (map == null)
            {
                map = new Dictionary<string, AssetPtr<T>>();
                this.assetStorageMap.Add(storage, map);
            }

            map.Add(res.Name, res);
            this.assetMap.Add(res.Name, res);
        }

        /// <summary>
        /// Get an asset by its name
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="storage">Storage in which to look for</param>
        /// <returns>Return an asset if the storage contains one withe the same name, otherwise null</returns>
        private AssetPtr<T> GetByName(string name, string storage)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Asset name is null or empty");
            }

            AssetPtr<T> res = null;
            this.assetMap.TryGetValue(name, out res);

            return res;
        }

        #endregion
    }
}
