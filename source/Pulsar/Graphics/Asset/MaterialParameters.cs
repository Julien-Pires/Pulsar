using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Defines parameters to load a Material instance
    /// </summary>
    public sealed class MaterialParameters : Freezable
    {
        #region Fields

        /// <summary>
        /// Default parameters to create a new instance
        /// </summary>
        public static readonly MaterialParameters NewInstance;

        private string _filename;

        #endregion

        #region Static constructor

        /// <summary>
        /// Static constructor of MaterialParameters class
        /// </summary>
        static MaterialParameters()
        {
            NewInstance = new MaterialParameters
            {
                Source = AssetSource.NewInstance
            };
            NewInstance.Freeze();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the source of the material
        /// </summary>
        public AssetSource Source { get; private set; }

        /// <summary>
        /// Gets or sets the name of the file
        /// </summary>
        public string Filename
        {
            get { return _filename; }
            set
            {
                if (IsFrozen)
                    throw new Exception("");

                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        #endregion
    }
}
