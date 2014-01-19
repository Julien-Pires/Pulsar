using System;
using System.Diagnostics;
using System.Collections.Generic;

using Pulsar.Core;

namespace Pulsar.Assets
{
    public sealed class AssetFolder : IDisposable
    {
        #region Fields

        private bool _disposed;
        private readonly string _directoryPath;
        private readonly AssetEngine _engine;
        private FolderManager _folderManager;
        private Dictionary<string, object> _assetsMap = new Dictionary<string, object>();
        private Dictionary<string, List<IDisposable>> _disposablesMap = new Dictionary<string, List<IDisposable>>();

        #endregion

        #region Constructors

        internal AssetFolder(string path, AssetEngine engine)
        {
            Debug.Assert(engine != null);

            _engine = engine;
            _directoryPath = PathHelpers.GetDirectoryPath(path);
            _folderManager = new FolderManager(path, this, _engine.ServiceProvider);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                UnloadAll();
                _folderManager.Dispose();
            }
            finally
            {
                _disposablesMap = null;
                _assetsMap = null;
                _folderManager = null;
            }

            _disposed = true;
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
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("assetName");

            assetName = PathHelpers.CleanPath(assetName);
            if (!_assetsMap.ContainsKey(assetName))
                return false;

            List<IDisposable> disposables;
            if (_disposablesMap.TryGetValue(assetName, out disposables))
            {
                for (int i = 0; i < disposables.Count; i++)
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

        public string GetNameFromFullPath(string path)
        {
            return PathHelpers.RemoveRoot(path, _directoryPath);
        }

        public T LoadWithFullPath<T>(string assetName, object parameters = null)
        {
            return LoadWithFullPath<T>(assetName, assetName, parameters);
        }

        public T LoadWithFullPath<T>(string assetName, string path, object parameters = null)
        {
            path = PathHelpers.RemoveRoot(path, _directoryPath);

            return InternalLoad<T>(assetName, path, parameters);
        }

        public T Load<T>(string assetName, object parameters = null)
        {
            return Load<T>(assetName, assetName, parameters);
        }

        public T Load<T>(string assetName, string path, object parameters = null)
        {
            if(path != null)
                path = PathHelpers.CleanPath(path);

            return InternalLoad<T>(assetName, path, parameters);
        }

        private T InternalLoad<T>(string assetName, string path, object parameters = null)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("assetName");

            object obj;
            if (!_assetsMap.TryGetValue(assetName, out obj))
            {
                IAssetLoader loader = _engine.GetLoader<T>() ?? _engine.DefaultLoader;
                LoadedAsset result = loader.Load<T>(assetName, path, parameters, this);
                if (result == null)
                    throw new NullReferenceException("");

                _assetsMap.Add(assetName, result.Asset);

                List<IDisposable> loadedDisposables = result.Disposables;
                if (loadedDisposables.Count > 0)
                {
                    List<IDisposable> disposables = EnsureDisposableList(assetName);
                    for (int j = 0; j < loadedDisposables.Count; j++)
                        disposables.Add(loadedDisposables[j]);
                }

                obj = result.Asset;
            }

            ICastable<T> castObj = obj as ICastable<T>;
            if (castObj != null)
                return castObj.Cast();

            if (!(obj is T))
                throw new Exception(string.Format("Asset {0} is not of type {1}", assetName, typeof(T)));

            return (T)obj;
        }

        internal void LoadFromFile<T>(string assetName, LoadedAsset result)
        {
            SearchResult status = _folderManager.LoadFromFile<T>(assetName, result);
            switch (status.State)
            {
                case SearchState.NotFound:
                    throw new Exception(string.Format("Asset {0} not found", assetName));

                case SearchState.ErrorLoading:
                    throw status.Error;
            }
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
    }
}
