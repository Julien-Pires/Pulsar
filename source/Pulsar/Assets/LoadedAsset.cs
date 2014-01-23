using System;
using System.Collections.Generic;

namespace Pulsar.Assets
{
    /// <summary>
    /// Contains datas about a loaded asset
    /// </summary>
    public sealed class LoadedAsset
    {
        #region Fields

        private const int DefaultCapacity = 4;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of LoadedAsset class
        /// </summary>
        internal LoadedAsset()
        {
            Disposables = new List<IDisposable>(DefaultCapacity);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the instance
        /// </summary>
        internal void Reset()
        {
            Asset = null;
            Disposables.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the asset
        /// </summary>
        public object Asset { get; set; }

        /// <summary>
        /// Gets a list of disposable resources used by the asset
        /// </summary>
        public List<IDisposable> Disposables { get; private set; }

        #endregion
    }
}
