using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Defines parameters to load a Mesh instance
    /// </summary>
    public sealed class MeshParameters : Freezable
    {
        #region Fields

        /// <summary>
        /// Default parameters to create a new instance
        /// </summary>
        public static readonly MeshParameters NewInstance;

        private string _filename;

        #endregion

        #region Static constructor

        /// <summary>
        /// Static constructor of MeshParameters class
        /// </summary>
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

        /// <summary>
        /// Gets the source of the mesh
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
                if(IsFrozen)
                    throw new Exception("Failed to change value, object is frozen");

                Source = string.IsNullOrEmpty(value) ? AssetSource.NewInstance : AssetSource.FromFile;
                _filename = value;
            }
        }

        #endregion
    }
}
