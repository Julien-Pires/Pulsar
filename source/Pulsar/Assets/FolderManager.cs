using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    internal sealed class FolderManager
    {
        #region Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, AssetFolder> _folders = new Dictionary<string, AssetFolder>();

        #endregion

        #region Constructor

        internal FolderManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Methods

        public void AddFolder(string folderName)
        {
            if(string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");

            if(_folders.ContainsKey(folderName))
                throw new Exception(string.Format("Folder {0} already added", folderName));

            AssetFolder folder = new AssetFolder(folderName, _serviceProvider);
            _folders.Add(folderName, folder);
        }

        public bool RemoveFolder(string folderName)
        {
            return _folders.Remove(folderName);
        }

        public bool IsUsingFolder(string folderName)
        {
            return _folders.ContainsKey(folderName);
        }

        public void SearchAsset<T>(string assetName, LoadResult result)
        {
            result.Reset();

            foreach (AssetFolder folder in _folders.Values)
            {
                SearchResult status = folder.Search<T>(assetName, result);
                switch (status.State)
                {
                    case SearchState.Found: return;

                    case SearchState.ErrorLoading:
                        throw status.Error;
                }
            }

            throw new Exception(string.Format("Asset {0} not found", assetName));
        }

        #endregion
    }
}
