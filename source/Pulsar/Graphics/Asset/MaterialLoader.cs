using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    public sealed class MaterialLoader : AssetLoader
    {
        #region Fields

        private readonly Type[] _supportedTypes = { typeof(Material) };
        private readonly LoadResult _result = new LoadResult();
        private readonly MaterialParameters _defaultParameter = new MaterialParameters();

        #endregion

        #region Constructors

        internal MaterialLoader()
        {
        }

        #endregion

        #region Methods

        public override LoadResult Load<T>(string assetName, object parameters, Storage storage)
        {
            _result.Reset();

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

            LoadedAsset loadedMaterial = _result.AddAsset(assetName);
            loadedMaterial.Asset = material;

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
