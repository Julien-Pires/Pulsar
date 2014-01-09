using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    public sealed class LoadedAsset
    {
        #region Fields

        private const int DefaultCapacity = 4;

        #endregion

        #region Constructor

        internal LoadedAsset()
        {
            Disposables = new List<IDisposable>(DefaultCapacity);
        }

        #endregion

        #region Methods

        internal void Reset()
        {
            Name = string.Empty;
            Asset = null;
            Disposables.Clear();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public object Asset { get; set; }

        public List<IDisposable> Disposables { get; private set; }

        #endregion
    }
}
