using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    public sealed class LoadResult
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private readonly List<LoadedAsset> _loadedAssets = new List<LoadedAsset>(DefaultCapacity);
        private int _usedCount;

        #endregion

        #region Constructor

        internal LoadResult()
        {
            for(int i = 0; i < DefaultCapacity; i++)
                _loadedAssets.Add(new LoadedAsset());
        }

        #endregion

        #region Operators

        public LoadedAsset this[int index]
        {
            get { return _loadedAssets[index]; }
        }

        public LoadedAsset this[string name]
        {
            get { return Get(name); }
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _usedCount = 0;
            for(int i = 0; i < _loadedAssets.Count; i++)
                _loadedAssets[i].Reset();
        }

        public LoadedAsset AddAsset(string name)
        {
            Debug.Assert(_usedCount > -1);

            LoadedAsset asset;
            if (_usedCount >= _loadedAssets.Count)
            {
                asset = new LoadedAsset();
                _loadedAssets.Add(asset);
            }
            else
                asset = _loadedAssets[_usedCount];

            asset.Reset();
            asset.Name = name;
            _usedCount++;

            return asset;
        }

        public bool RemoveAsset(string assetName)
        {
            int index = IndexOf(assetName);
            if(index == -1)
                return false;

            RemoveAsset(index);

            return true;
        }

        public void RemoveAsset(int index)
        {
            if((index < 0) || (index >= _usedCount))
                throw new ArgumentOutOfRangeException("index");

            LoadedAsset temp = _loadedAssets[_usedCount - 1];
            _loadedAssets[_usedCount - 1] = _loadedAssets[index];
            _loadedAssets[index] = temp;

            _usedCount--;
            _loadedAssets[_usedCount].Reset();
        }

        public int IndexOf(string assetName)
        {
            for (int i = 0; i < _loadedAssets.Count; i++)
            {
                if (!string.Equals(assetName, _loadedAssets[i].Name)) continue;

                return i;
            }

            return -1;
        }

        public int IndexOf(Type assetType)
        {
            for (int i = 0; i < _loadedAssets.Count; i++)
            {
                LoadedAsset asset = _loadedAssets[i];
                if (asset.Asset.GetType().IsSubclassOf(assetType))
                    return i;
            }

            return -1;
        }

        public LoadedAsset Get(Type assetType)
        {
            int index = IndexOf(assetType);
            if (index == -1)
                return null;

            return _loadedAssets[index];
        }

        public LoadedAsset Get<T>()
        {
            return Get(typeof (T));
        }

        public LoadedAsset Get(string assetName)
        {
            int index = IndexOf(assetName);
            if (index == -1)
                return null;

            return _loadedAssets[index];
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _usedCount; }
        }

        #endregion
    }
}
