using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    /// <summary>
    /// Manages a folder that contains assets
    /// </summary>
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

        /// <summary>
        /// Constructors of AssetFolder class
        /// </summary>
        /// <param name="path">Path of the folder</param>
        /// <param name="engine">AssetEngine instance</param>
        internal AssetFolder(string path, AssetEngine engine)
        {
            Debug.Assert(engine != null);

            _engine = engine;
            _directoryPath = PathHelpers.RemovesFirstFolder(path, 0);
            _folderManager = new FolderManager(path, this, _engine.ServiceProvider);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
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

        /// <summary>
        /// Unloads all asset in the folder
        /// </summary>
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

        /// <summary>
        /// Unloads an asset with a specified name
        /// </summary>
        /// <param name="assetName">Name of the asset</param>
        /// <returns>Returns true if the asset is unloaded successfully otherwise false</returns>
        public bool Unload(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("assetName");

            assetName = PathHelpers.CleanPath(assetName);
            if (!_assetsMap.ContainsKey(assetName))
                return false;

            List<IDisposable> disposables = null;
            try
            {
                if (_disposablesMap.TryGetValue(assetName, out disposables))
                {
                    for (int i = 0; i < disposables.Count; i++)
                        disposables[i].Dispose();
                }
            }
            finally
            {
                if(disposables != null)
                    disposables.Clear();

                _disposablesMap.Remove(assetName);
                _assetsMap.Remove(assetName);
            }

            return true;
        }

        /// <summary>
        /// Checks if an asset with a specified name is loaded
        /// </summary>
        /// <param name="assetName">Name of the asset</param>
        /// <returns>Returns true if the asset is loaded otherwise false</returns>
        public bool IsLoaded(string assetName)
        {
            return _assetsMap.ContainsKey(assetName);
        }

        /// <summary>
        /// Gets the name of an asset from a full path
        /// </summary>
        /// <example>
        /// Folder path : Content\Mesh
        /// Full path : Content\Mesh\Textures\MyTexture
        /// Result : Textures\MyTexture
        /// </example>
        /// <param name="path">Path from wich to extract asset name</param>
        /// <returns>Returns the asset name for this folder</returns>
        public string GetNameFromFullPath(string path)
        {
            return PathHelpers.RemoveRoot(path, _directoryPath);
        }

        /// <summary>
        /// Loads an asset from a full path
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>Returns the loaded asset</returns>
        public T LoadWithFullPath<T>(string assetName, object parameters = null)
        {
            return LoadWithFullPath<T>(assetName, assetName, parameters);
        }

        /// <summary>
        /// Loads an asset from a full path
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>Returns the loaded asset</returns>
        public T LoadWithFullPath<T>(string assetName, string path, object parameters = null)
        {
            if(path == null)
                throw new ArgumentNullException("path");

            path = PathHelpers.RemoveRoot(path, _directoryPath);

            return InternalLoad<T>(assetName, path, parameters);
        }

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>Returns the loaded asset</returns>
        public T Load<T>(string assetName, object parameters = null)
        {
            return Load<T>(assetName, assetName, parameters);
        }

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>Returns the loaded asset</returns>
        public T Load<T>(string assetName, string path, object parameters = null)
        {
            if(path != null)
                path = PathHelpers.CleanPath(path);

            return InternalLoad<T>(assetName, path, parameters);
        }

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>Returns the loaded asset</returns>
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

        /// <summary>
        /// Loads an asset from a file in this folder
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="result">Result that will contain everything about the loaded asset</param>
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

        /// <summary>
        /// Gets the list of disposable resources for an asset if it exists otherwise the list is created first
        /// </summary>
        /// <param name="assetName">Name of the asset</param>
        /// <returns>Returns the list of disposable resources</returns>
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

        /// <summary>
        /// Gets the number of loaded asset
        /// </summary>
        public int Count
        {
            get { return _assetsMap.Count; }
        }

        #endregion
    }
}
