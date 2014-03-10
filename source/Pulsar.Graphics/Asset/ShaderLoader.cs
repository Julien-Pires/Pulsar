using System;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Shader asset
    /// </summary>
    public sealed class ShaderLoader : AssetLoader
    {
        #region Fields

        internal const string LoaderName = "ShaderLoader";

        private readonly Type[] _supportedTypes = { typeof(ShaderOld) };
        private readonly ShaderParameters _defaultParameters = new ShaderParameters();
        private readonly LoadedAsset _result = new LoadedAsset();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderLoader class
        /// </summary>
        internal ShaderLoader()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads an asset
        /// </summary>
        /// <typeparam name="T">Type of asset</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Optional parameters</param>
        /// <param name="assetFolder">Folder where the asset will be stored</param>
        /// <returns>Returns a loaded asset</returns>
        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();

            ShaderParameters shaderParameters;
            if (parameters != null)
            {
                shaderParameters = parameters as ShaderParameters;
                if(shaderParameters == null)
                    throw new ArgumentException("Invalid parameters for this loader");
            }
            else
            {
                shaderParameters = _defaultParameters;
                shaderParameters.Filename = path;
            }

            ShaderOld shader;
            switch (shaderParameters.Source)
            {
                case AssetSource.FromFile:
                    shader = CreateInstance(assetName, shaderParameters.ShaderType);
                    LoadFromFile<Effect>(shaderParameters.Filename, assetFolder, _result);

                    shader.SetEffect(_result.Asset as Effect);
                    break;

                case AssetSource.NewInstance:
                    throw new NotSupportedException("Cannot create new instance of Shader");

                default:
                    throw new Exception("Invalid asset source provided");
            }
            _result.Reset();

            _result.Asset = shader;
            _result.Disposables.Add(shader);

            return _result;
        }

        /// <summary>
        /// Creates a new instance of derived Shader class
        /// </summary>
        /// <param name="name">Name of the shader</param>
        /// <param name="shaderType">Type of shader</param>
        /// <returns>Returns a shader instance</returns>
        public ShaderOld CreateInstance(string name, Type shaderType)
        {
            if (shaderType == null) 
                return new ShaderOld(name);

            if (!shaderType.IsSubclassOf(typeof(ShaderOld)))
                throw new Exception(string.Format("Failed to create instance of {0}, this class doesn't inherit from Shader class", shaderType));

            ConstructorInfo[] constructInfo = shaderType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (constructInfo.Length == 0)
                throw new Exception(string.Format("Failed to create instance of {0}, no constructor available", shaderType));

            ShaderOld fx = (ShaderOld)constructInfo[0].Invoke(new object[] { name });

            return fx;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the loader
        /// </summary>
        public override string Name
        {
            get { return LoaderName; }
        }

        /// <summary>
        /// Gets the list of assets supported by this loader
        /// </summary>
        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}
