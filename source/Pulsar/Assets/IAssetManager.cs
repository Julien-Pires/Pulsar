using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    /// <summary>
    /// Interface defining a manager of asset
    /// </summary>
    /// <typeparam name="T">Type of managed asset</typeparam>
    public interface IAssetManager
    {
        #region Methods

        /// <summary>
        /// Create a new instance of an asset
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="parameter">Additional parameter</param>
        /// <returns>Return a new instance of an asset</returns>
        Asset CreateInstance(string name, object parameter = null);

        #endregion
    }
}