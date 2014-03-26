using System;
using System.Reflection;
using System.Collections.Generic;

using Pulsar.System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Represents the entry point of the asset engine
    /// </summary>
    public sealed class AssetEngine : IDisposable
    {
        #region Fields

        private const BindingFlags CtorFlag = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        internal readonly DefaultLoader DefaultLoader = new DefaultLoader();
        internal readonly IServiceProvider ServiceProvider;

        private bool _isDisposed;
        private Storage _globalStorage;
        private Dictionary<string, Storage> _storages = new Dictionary<string, Storage>();
        private readonly Dictionary<Type, IAssetLoader> _loadersMap = new Dictionary<Type, IAssetLoader>();
        private readonly Dictionary<string, List<IAssetLoader>> _pendingInit =
            new Dictionary<string, List<IAssetLoader>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructors of AssetEngine class
        /// </summary>
        /// <param name="serviceProvider">Services provider</param>
        public AssetEngine(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _globalStorage = new Storage("GlobalStorage", this);

            InternalInitializeLoaders();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets the storage with a specified name
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Returns the storage with the specified name</returns>
        public Storage this[string name]
        {
            get { return GetStorage(name); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                foreach (Storage storage in _storages.Values)
                    storage.Dispose();

                _globalStorage.Dispose();
            }
            finally
            {
                _storages.Clear();
                _storages = null;
                _globalStorage = null;
            }

            _isDisposed = true;
        }

        public void InitializeLoaders(string category)
        {
            List<IAssetLoader> list;
            if(!_pendingInit.TryGetValue(category, out list))
                return;

            for (int i = 0; i < list.Count; i++)
                list[i].Initialize(this, ServiceProvider);
            
            list.Clear();
        }

        private void InternalInitializeLoaders()
        {
            TypeDetector detector = new TypeDetector
            {
                BaseType = typeof(IAssetLoader),
                Exclude = (TypeDetectorRule.Abstract | TypeDetectorRule.Interface | TypeDetectorRule.Private |
                           TypeDetectorRule.ValueType | TypeDetectorRule.NoParameterLessCtor | TypeDetectorRule.Nested)
            };
            detector.Attributes.Add(typeof(AssetLoaderAttribute));
            foreach (Type type in detector.GetTypes())
            {
                ConstructorInfo ctorInfo = type.GetConstructor(CtorFlag, null, Type.EmptyTypes, null);
                if (ctorInfo == null)
                    throw new Exception("");

                IAssetLoader loader = (IAssetLoader)ctorInfo.Invoke(null);
                AddLoader(loader);
            }
        }

        /// <summary>
        /// Gets the loader for a specified type of asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <returns>Returns a loader</returns>
        public IAssetLoader GetLoader<T>() 
        {
            return GetLoader(typeof (T));
        }

        /// <summary>
        /// Gets the loader for a specified type of asset
        /// </summary>
        /// <param name="assetType">Type of asset</param>
        /// <returns>Returns a loader</returns>
        public IAssetLoader GetLoader(Type assetType)
        {
            if(assetType == null)
                throw new ArgumentNullException("assetType");

            IAssetLoader loader;
            _loadersMap.TryGetValue(assetType, out loader);

            return loader;
        }

        /// <summary>
        /// Adds an asset loader
        /// </summary>
        /// <param name="loader">Loader to add</param>
        private void AddLoader(IAssetLoader loader)
        {
            if(loader == null)
                throw new ArgumentNullException("loader");

            object[] attributes = loader.GetType().GetCustomAttributes(typeof (AssetLoaderAttribute), false);
            if(attributes.Length == 0)
                throw new Exception("");

            AssetLoaderAttribute loaderAttr = (AssetLoaderAttribute)attributes[0];
            Type[] supportedTypes = loaderAttr.AssetTypes;
            if(supportedTypes.Length == 0) return;

            for (int i = 0; i < supportedTypes.Length; i++)
            {
                if(_loadersMap.ContainsKey(supportedTypes[i]))
                    throw new Exception(string.Format("{0} already managed by another loader", supportedTypes[i]));

                _loadersMap.Add(supportedTypes[i], loader);
            }

            if(!string.IsNullOrWhiteSpace(loaderAttr.LazyInitCategory))
                AddLazyInitLoader(loader, loaderAttr.LazyInitCategory);
            else
                loader.Initialize(this, ServiceProvider);
        }

        private void AddLazyInitLoader(IAssetLoader loader, string category)
        {
            List<IAssetLoader> list;
            if (!_pendingInit.TryGetValue(category, out list))
            {
                list = new List<IAssetLoader>();
                _pendingInit.Add(category, list);
            }

            if (list.Contains(loader)) return;

            list.Add(loader);
        }

        /// <summary>
        /// Removes a loader for a specified type of asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        public void RemoveLoader<T>()
        {
            RemoveLoader(typeof(T));
        }

        /// <summary>
        /// Removes a loader for a specified type of asset
        /// </summary>
        /// <param name="assetType">Type of asset</param>
        public void RemoveLoader(Type assetType)
        {
            if(assetType == null)
                throw new ArgumentNullException("assetType");

            _loadersMap[assetType] = null;
        }

        /// <summary>
        /// Removes a loader
        /// </summary>
        /// <param name="loader">Loader to remove</param>
        /// <returns>Returns true if the loader is removed successfully otherwise false</returns>
        public bool RemoveLoader(IAssetLoader loader)
        {
            if(loader == null)
                throw new ArgumentNullException("loader");

            object[] attributes = loader.GetType().GetCustomAttributes(typeof(AssetLoaderAttribute), false);
            if (attributes.Length == 0)
                throw new Exception("");

            AssetLoaderAttribute loaderAttr = (AssetLoaderAttribute)attributes[0];
            Type[] assetType = loaderAttr.AssetTypes;
            for (int i = 0; i < assetType.Length; i++)
            {
                IAssetLoader currentLoader;
                if (!_loadersMap.TryGetValue(assetType[i], out currentLoader))
                    continue;

                if (currentLoader == loader)
                    _loadersMap.Remove(assetType[i]);
            }

            return true;
        }

        /// <summary>
        /// Creates a storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Returns a new storage</returns>
        public Storage CreateStorage(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if(_storages.ContainsKey(name))
                throw new ArgumentException(string.Format("Failed to create, a storage named {0} already exist", name));

            Storage storage = new Storage(name, this);
            _storages.Add(name, storage);

            return storage;
        }

        /// <summary>
        /// Destroys a storage
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Returns true if the storage is destroyed otherwise false</returns>
        public bool DestroyStorage(string name)
        {
            Storage storage;
            _storages.TryGetValue(name, out storage);

            if (storage == null)
                return false;

            _storages.Remove(name);
            storage.Dispose();

            return true;
        }

        /// <summary>
        /// Gets a storage with a specified name
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <returns>Returns a storage instance if found otherwise null</returns>
        public Storage GetStorage(string name)
        {
            Storage storage;
            _storages.TryGetValue(name, out storage);

            return storage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the global storage
        /// </summary>
        public Storage GlobalStorage
        {
            get { return _globalStorage; }
        }

        #endregion
    }
}
