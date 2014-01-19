using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    internal sealed class FolderManager : ContentManager
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private int _assetOpened;
        private readonly AssetFolder _parent;
        private readonly Stack<LoadedAsset> _loadedStack = new Stack<LoadedAsset>(DefaultCapacity);

        #endregion

        #region Constructor

        internal FolderManager(string path, AssetFolder parent, IServiceProvider serviceProvider)
            : base(serviceProvider, path)
        {
            _parent = parent;
        }

        #endregion

        #region Methods

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

        public override T Load<T>(string assetName)
        {
            Debug.Assert(_loadedStack.Count > 0);

            return _parent.Load<T>(assetName);
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
