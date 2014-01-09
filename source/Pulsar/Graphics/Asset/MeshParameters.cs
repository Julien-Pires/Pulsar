using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    public sealed class MeshParameters : Freezable
    {
        #region Fields

        public static readonly MeshParameters NewInstance;

        private string _filename;

        #endregion

        #region Static constructor

        static MeshParameters()
        {
            NewInstance = new MeshParameters
            {
                Source = AssetSource.NewInstance
            };
            NewInstance.Freeze();
        }

        #endregion

        #region Properties

        public AssetSource Source { get; private set; }

        public string Filename
        {
            get { return _filename; }
            set
            {
                if(IsFrozen)
                    throw new Exception("");

                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        #endregion
    }
}
