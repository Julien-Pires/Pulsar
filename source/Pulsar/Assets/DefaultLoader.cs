using System;

namespace Pulsar.Assets
{
    public sealed class DefaultLoader : AssetLoader
    {
        #region Fields

        private const string LoaderName = "DefaultLoader";

        private readonly Type[] _supportedTypes = {};
        private readonly LoadedAsset _result = new LoadedAsset();

        #endregion

        #region Methods

        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();
            _result.Name = assetName;
            LoadFromFile<T>(path, assetFolder, _result);

            return _result;
        }

        #endregion

        #region Properties

        public override string Name
        {
            get { return LoaderName; }
        }

        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}
