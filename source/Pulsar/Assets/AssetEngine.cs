using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    public sealed class AssetEngine : IDisposable
    {
        #region Fields

        internal readonly DefaultLoader DefaultLoader = new DefaultLoader();
        internal readonly IServiceProvider ServiceProvider;

        private bool _isDisposed;
        private readonly Storage _systemStorage;
        private readonly Storage _globalStorage;
        private Dictionary<string, Storage> _storages = new Dictionary<string, Storage>();
        private readonly Dictionary<Type, IAssetLoader> _loaders = new Dictionary<Type, IAssetLoader>();
        private readonly Dictionary<string, IAssetLoader> _loadersByName = new Dictionary<string, IAssetLoader>();

        #endregion

        #region Constructors

        public AssetEngine(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _systemStorage = new Storage("SystemStorage", this);
            _globalStorage = new Storage("GlobalStorage", this);
        }

        #endregion

        #region Operators

        public Storage this[string name]
        {
            get { return GetStorage(name); }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                foreach (Storage storage in _storages.Values)
                    storage.Dispose();
            }
            finally
            {
                _storages.Clear();
                _storages = null;
            }

            _isDisposed = true;
        }

        public IAssetLoader GetLoader<T>() 
        {
            return GetLoader(typeof (T));
        }

        public IAssetLoader GetLoader(string name)
        {
            return _loadersByName[name];
        }

        public IAssetLoader GetLoader(Type assetType)
        {
            if(assetType == null)
                throw new ArgumentNullException("assetType");

            IAssetLoader loader;
            _loaders.TryGetValue(assetType, out loader);

            return loader;
        }

        public void AddLoader(IAssetLoader loader)
        {
            if(loader == null)
                throw new ArgumentNullException("loader");

            Type[] supportedTypes = loader.SupportedTypes;
            if(supportedTypes.Length == 0) return;

            for (int i = 0; i < supportedTypes.Length; i++)
            {
                if(_loaders.ContainsKey(supportedTypes[i]))
                    throw new Exception(string.Format("{0} already managed by another loader", supportedTypes[i]));

                _loaders.Add(supportedTypes[i], loader);
            }
        }

        public void RemoveLoader<T>()
        {
            RemoveLoader(typeof(T));
        }

        public void RemoveLoader(Type assetType)
        {
            if(assetType == null)
                throw new ArgumentNullException("assetType");

            _loaders[assetType] = null;
        }

        public bool RemoveLoader(IAssetLoader loader)
        {
            if(loader == null)
                throw new ArgumentNullException("loader");

            Type[] assetType = loader.SupportedTypes;
            for (int i = 0; i < assetType.Length; i++)
            {
                IAssetLoader currentLoader;
                if (!_loaders.TryGetValue(assetType[i], out currentLoader))
                    continue;

                if (currentLoader == loader)
                    _loaders.Remove(assetType[i]);
            }

            _loadersByName.Remove(loader.Name);

            return true;
        }

        public bool RemoveLoader(string name)
        {
            IAssetLoader loader;
            if (!_loadersByName.TryGetValue(name, out loader))
                return false;

            return RemoveLoader(loader);
        }

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

        public Storage GetStorage(string name)
        {
            Storage storage;
            _storages.TryGetValue(name, out storage);

            return storage;
        }

        #endregion

        #region Properties

        internal Storage SystemStorage
        {
            get { return _systemStorage; }
        }

        public Storage GlobalStorage
        {
            get { return _globalStorage; }
        }

        #endregion
    }
}
