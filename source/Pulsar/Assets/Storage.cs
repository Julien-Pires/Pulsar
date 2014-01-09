using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    public sealed class Storage : IDisposable
    {
        #region Fields

        private bool _disposed;
        private readonly AssetEngine _engine;
        private readonly FolderManager _folderManager;
        private Dictionary<string, object> _assetsMap = new Dictionary<string, object>();
        private Dictionary<string, List<IDisposable>> _disposablesMap = new Dictionary<string, List<IDisposable>>();

        #endregion

        #region Constructors

        internal Storage(string name, AssetEngine engine)
        {
            Name = name;
            _engine = engine;
            _folderManager = new FolderManager(_engine.ServiceProvider);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_disposed) return;

            try
            {
                UnloadAll();
            }
            finally
            {
                _disposablesMap = null;
                _assetsMap = null;
            }

            _disposed = true;
        }

        public void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
        }

        public bool RemoveFolder(string folderName)
        {
            return _folderManager.RemoveFolder(folderName);
        }

        public void UnloadAll()
        {
            try
            {
                foreach (List<IDisposable> disposableList in _disposablesMap.Values)
                {
                    for (int i = 0; i < disposableList.Count; i++)
                        disposableList[i].Dispose();

                    disposableList.Clear();
                }
            }
            finally
            {
                _disposablesMap.Clear();
                _assetsMap.Clear();
            }
        }

        public bool Unload(string assetName)
        {
            if(string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("assetName");

            assetName = PathHelpers.GetPath(assetName);
            if (!_assetsMap.ContainsKey(assetName))
                return false;

            List<IDisposable> disposables;
            if (_disposablesMap.TryGetValue(assetName, out disposables))
            {
                for(int i = 0; i < disposables.Count; i++)
                    disposables[i].Dispose();

                disposables.Clear();
            }
            _disposablesMap.Remove(assetName);
            _assetsMap.Remove(assetName);

            return true;
        }

        public bool IsLoaded(string name)
        {
            return _assetsMap.ContainsKey(name);
        }

        public T Load<T>(string assetName, object parameters = null)
        {
            if(string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("assetName");

            assetName = PathHelpers.GetPath(assetName);
            object obj;
            if (!_assetsMap.TryGetValue(assetName, out obj))
            {
                AssetLoader loader = _engine.GetLoader<T>() ?? _engine.DefaultLoader;
                LoadResult result = loader.Load<T>(assetName, parameters, this);
                LoadedAsset asset = result[assetName];
                if (asset == null)
                    throw new Exception(string.Format("Failed to find {0}", assetName));

                for (int i = 0; i < result.Count; i++)
                {
                    LoadedAsset loadedAsset = result[i];
                    _assetsMap.Add(loadedAsset.Name, loadedAsset.Asset);

                    List<IDisposable> loadedDisposables = loadedAsset.Disposables;
                    if (loadedDisposables.Count > 0)
                    {
                        List<IDisposable> disposables = EnsureDisposableList(assetName);
                        for (int j = 0; j < loadedDisposables.Count; j++)
                            disposables.Add(loadedDisposables[j]);
                    }
                }

                return (T) asset.Asset;
            }

            if(!(obj is T))
                throw new Exception(string.Format("Asset {0} is not of type {1}", assetName, typeof(T)));

            return (T)obj;
        }

        internal void LoadFromFile<T>(string assetName, LoadResult result)
        {
            _folderManager.SearchAsset<T>(assetName, result);
        }

        private List<IDisposable> EnsureDisposableList(string assetName)
        {
            List<IDisposable> disposables;
            if (_disposablesMap.TryGetValue(assetName, out disposables)) 
                return disposables;

            disposables = new List<IDisposable>();
            _disposablesMap.Add(assetName, disposables);

            return disposables;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        #endregion
    }
}
