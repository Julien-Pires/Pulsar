using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Indicates that a class is an asset loader
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AssetLoaderAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets a category name for lazy initialization
        /// </summary>
        public string LazyInitCategory { get; set; }

        /// <summary>
        /// Gets or sets types of asset managed by the asset loader
        /// </summary>
        public Type[] AssetTypes { get; set; }

        #endregion
    }
}
