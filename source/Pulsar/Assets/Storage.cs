using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    public sealed class Storage : IDisposable
    {
        #region Fields

        private bool _disposed;
        private readonly AssetEngine _engine;
        private Dictionary<string, AssetFolder> _foldersMap = new Dictionary<string, AssetFolder>();

        #endregion

        #region Constructors

        internal Storage(string name, AssetEngine engine)
        {
            Name = name;
            _engine = engine;
        }

        #endregion

        #region Operators

        public AssetFolder this[string name]
        {
            get { return _foldersMap[name]; }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_disposed) return;

            try
            {
                foreach (AssetFolder folder in _foldersMap.Values)
                    folder.Dispose();
            }
            finally
            {
                _foldersMap.Clear();
                _foldersMap = null;
            }

            _disposed = true;
        }

        public void AddFolder(string path)
        {
            AddFolder(path, path);
        }

        public void AddFolder(string path, string name)
        {
            if(_foldersMap.ContainsKey(name))
                throw new ArgumentException("");

            AssetFolder folder = new AssetFolder(path, _engine);
            _foldersMap.Add(name, folder);
        }

        public bool RemoveFolder(string name)
        {
            AssetFolder folder;
            if (!_foldersMap.TryGetValue(name, out folder))
                return false;

            _foldersMap.Remove(name);
            folder.Dispose();

            return true;
        }

        public AssetFolder GetFolder(string name)
        {
            return _foldersMap[name];
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        #endregion
    }
}
