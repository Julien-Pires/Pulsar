using System;
using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    public sealed class ShaderParameters
    {
        #region Fields

        private string _filename;

        #endregion

        #region Parameters

        public AssetSource Source { get; private set; }

        public Type ShaderType { get; set; }

        public string Filename
        {
            get { return _filename; }
            set
            {
                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        #endregion
    }
}
