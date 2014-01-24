using System;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Defines parameters to load a Shader instance
    /// </summary>
    public sealed class ShaderParameters
    {
        #region Fields

        private string _filename;

        #endregion

        #region Parameters

        /// <summary>
        /// Gets the source of the Shader
        /// </summary>
        public AssetSource Source { get; private set; }

        /// <summary>
        /// Gets or sets the type of shader
        /// </summary>
        public Type ShaderType { get; set; }

        /// <summary>
        /// Gets or sets the name of the file
        /// </summary>
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
