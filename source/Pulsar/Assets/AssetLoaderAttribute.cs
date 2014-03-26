using System;

namespace Pulsar.Assets
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AssetLoaderAttribute : Attribute
    {
        #region Properties

        public string LazyInitCategory { get; set; }

        public Type[] AssetTypes { get; set; }

        #endregion
    }
}
