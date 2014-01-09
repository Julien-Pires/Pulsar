using System;

namespace Pulsar.Assets
{
    public sealed class DefaultLoader : AssetLoader
    {
        #region Fields

        private readonly Type[] _supportedTypes = {};
        private readonly LoadResult _result = new LoadResult();

        #endregion

        #region Methods

        public override LoadResult Load<T>(string assetName, object parameters, Storage storage)
        {
            _result.Reset();
            LoadFromFile<T>(assetName, storage, _result);

            return _result;
        }

        #endregion

        #region Properties

        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}
