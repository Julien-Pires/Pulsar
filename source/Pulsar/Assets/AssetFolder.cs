using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    internal sealed class AssetFolder : ContentManager
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private LoadResult _currentResult;
        private int _assetOpened;
        private readonly Stack<LoadedAsset> _loadedStack = new Stack<LoadedAsset>(DefaultCapacity);

        #endregion

        #region Constructor

        public AssetFolder(string folder, IServiceProvider serviceProvider)
            : base(serviceProvider, folder)
        {
        }

        #endregion

        #region Methods

        public SearchResult Search<T>(string assetName, LoadResult result)
        {
            Debug.Assert(_loadedStack.Count == 0);

            if(result == null)
                throw new ArgumentNullException("result");

            SearchResult status = new SearchResult { State = SearchState.Found };
            _currentResult = result;
            try
            {
                Load<T>(assetName);
            }
            catch (Exception ex)
            {
                status.State = (_assetOpened == 0) ? SearchState.NotFound : SearchState.ErrorLoading;
                status.Error = ex;
                _currentResult.Reset();
            }
            finally
            {
                _assetOpened = 0;
                _loadedStack.Clear();
                _currentResult = null;
            }

            return status;
        }

        public override T Load<T>(string assetName)
        {
            LoadedAsset loadedAsset = _currentResult.AddAsset(assetName);

            _loadedStack.Push(loadedAsset);
            T asset = ReadAsset<T>(assetName, AddDisposable);
            loadedAsset.Asset = asset;
            _loadedStack.Pop();

            return asset;
        }

        protected override Stream OpenStream(string assetName)
        {
            Stream result = base.OpenStream(assetName);
            _assetOpened++;

            return result;
        }

        private void AddDisposable(IDisposable disposable)
        {
            Debug.Assert(_loadedStack.Count > 0);

            LoadedAsset loadedAsset = _loadedStack.Peek();
            loadedAsset.Disposables.Add(disposable);
        }

        #endregion
    }
}
