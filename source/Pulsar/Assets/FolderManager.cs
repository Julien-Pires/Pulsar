using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    /// <summary>
    /// Manages a physical folder that contains asset
    /// </summary>
    internal sealed class FolderManager : ContentManager
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private int _assetOpened;
        private readonly AssetFolder _parent;
        private readonly Stack<LoadedAsset> _loadedStack = new Stack<LoadedAsset>(DefaultCapacity);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of FolderManager class
        /// </summary>
        /// <param name="path">Directory</param>
        /// <param name="parent">Parent asset folder</param>
        /// <param name="serviceProvider">Services provider</param>
        internal FolderManager(string path, AssetFolder parent, IServiceProvider serviceProvider)
            : base(serviceProvider, path)
        {
            _parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads an asset from a file
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="result">Result containing everything about the loaded asset</param>
        /// <returns>Returns a value indicating if the asset is loaded successfully</returns>
        internal SearchResult LoadFromFile<T>(string assetName, LoadedAsset result)
        {
            SearchResult status = new SearchResult { State = SearchState.Found };
            try
            {
                _loadedStack.Push(result);
                T asset = ReadAsset<T>(assetName, AddDisposable);
                result.Asset = asset;
            }
            catch (Exception ex)
            {
                status.State = (_assetOpened == 0) ? SearchState.NotFound : SearchState.ErrorLoading;
                status.Error = ex;

                result.Reset();
            }
            finally
            {
                _loadedStack.Pop();
                _assetOpened--;
            }

            return status;
        }

        /// <summary>
        /// Loads an external asset (recursive calls from ReadAsset)
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <returns>Returns the loaded asset</returns>
        public override T Load<T>(string assetName)
        {
            Debug.Assert(_loadedStack.Count > 0);

            return _parent.Load<T>(assetName);
        }

        /// <summary>
        /// Opens an asset file
        /// </summary>
        /// <param name="assetName">Name of the asset</param>
        /// <returns>Returns a stream containing asset data</returns>
        protected override Stream OpenStream(string assetName)
        {
            Stream result = base.OpenStream(assetName);
            _assetOpened++;

            return result;
        }

        /// <summary>
        /// Adds disposable resources for the current loaded asset
        /// </summary>
        /// <param name="disposable">Disposable resource</param>
        private void AddDisposable(IDisposable disposable)
        {
            Debug.Assert(_loadedStack.Count > 0);

            LoadedAsset loadedAsset = _loadedStack.Peek();
            loadedAsset.Disposables.Add(disposable);
        }

        #endregion
    }
}
