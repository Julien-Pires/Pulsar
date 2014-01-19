using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    public sealed class MaterialLoader : AssetLoader
    {
        #region Fields

        internal const string LoaderName = "MaterialLoader";

        private readonly Type[] _supportedTypes = { typeof(Material) };
        private readonly LoadedAsset _result = new LoadedAsset();
        private readonly MaterialParameters _defaultParameter = new MaterialParameters();

        #endregion

        #region Constructors

        internal MaterialLoader()
        {
        }

        #endregion

        #region Methods

        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            MaterialParameters matParameters;
            if (parameters != null)
            {
                matParameters = parameters as MaterialParameters;
                if(matParameters == null)
                    throw new Exception("");
            }
            else
                matParameters = _defaultParameter;

            Material material;
            switch (matParameters.Source)
            {
                case AssetSource.FromFile:
                    throw new NotSupportedException("");

                case AssetSource.NewInstance:
                    material = new Material(assetName);
                    break;

                default:
                    throw new Exception("");
            }
            _result.Reset();
            _result.Name = assetName;
            _result.Asset = material;

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
