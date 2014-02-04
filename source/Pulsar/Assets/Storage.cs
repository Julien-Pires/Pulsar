using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    /// <summary>
    /// Manages multiple folders of asset
    /// </summary>
    public sealed class Storage : IDisposable
    {
        #region Fields

        private bool _disposed;
        private readonly AssetEngine _engine;
        private Dictionary<string, AssetFolder> _foldersMap = new Dictionary<string, AssetFolder>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Storage class
        /// </summary>
        /// <param name="name">Name of the storage</param>
        /// <param name="engine">Asset engine that create this storage</param>
        internal Storage(string name, AssetEngine engine)
        {
            Name = name;
            _engine = engine;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets a folder for a specified name
        /// </summary>
        /// <param name="name">Name of the folder</param>
        /// <returns>Returns a folder</returns>
        public AssetFolder this[string name]
        {
            get { return _foldersMap[name]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes all resources
        /// </summary>
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

        /// <summary>
        /// Adds a folder
        /// </summary>
        /// <param name="path">Path of the folder</param>
        public void AddFolder(string path)
        {
            AddFolder(path, path);
        }

        /// <summary>
        /// Adds a folder
        /// </summary>
        /// <param name="path">Path of the folder</param>
        /// <param name="name">Name of the folder</param>
        public void AddFolder(string path, string name)
        {
            if(_foldersMap.ContainsKey(name))
                throw new ArgumentException(string.Format("Failed to add, a folder named {0} already presents", name));

            AssetFolder folder = new AssetFolder(path, _engine);
            _foldersMap.Add(name, folder);
        }

        /// <summary>
        /// Removes a folder
        /// </summary>
        /// <param name="name">Name of the folder</param>
        /// <returns>Returns true if the folder is removed successfully otherwise false</returns>
        public bool RemoveFolder(string name)
        {
            AssetFolder folder;
            if (!_foldersMap.TryGetValue(name, out folder))
                return false;

            _foldersMap.Remove(name);
            folder.Dispose();

            return true;
        }

        /// <summary>
        /// Gets a folder with a specified name
        /// </summary>
        /// <param name="name">Name of the folder</param>
        /// <returns>Returns an AssetFolder instance</returns>
        public AssetFolder GetFolder(string name)
        {
            return _foldersMap[name];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the storage
        /// </summary>
        public string Name { get; private set; }

        #endregion
    }
}
