using System;
using System.Reflection;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Class used to load Shader
    /// </summary>
    public sealed class ShaderLoader : AssetLoader
    {
        #region Fields

        internal const string LoaderName = "ShaderLoader";

        private readonly Type[] _supportedTypes = { typeof(Shader) };
        private readonly ShaderParameters _defaultParameters = new ShaderParameters();
        private readonly LoadedAsset _result = new LoadedAsset();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the ShaderManager class
        /// </summary>
        internal ShaderLoader()
        {
        }

        #endregion

        #region Methods

        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            _result.Reset();
            _result.Name = assetName;

            ShaderParameters shaderParameters;
            if (parameters != null)
            {
                shaderParameters = parameters as ShaderParameters;
                if(shaderParameters == null)
                    throw new ArgumentException("");
            }
            else
            {
                shaderParameters = _defaultParameters;
                shaderParameters.Filename = path;
            }

            Shader shader;
            switch (shaderParameters.Source)
            {
                case AssetSource.FromFile:
                    shader = CreateInstance(assetName, shaderParameters.ShaderType);
                    LoadFromFile<Effect>(shaderParameters.Filename, assetFolder, _result);

                    shader.SetEffect(_result.Asset as Effect);
                    break;

                case AssetSource.NewInstance:
                    throw new NotSupportedException();
                default:
                    throw new Exception("");
            }
            _result.Reset();

            _result.Asset = shader;
            _result.Disposables.Add(shader);

            return _result;
        }

        public Shader CreateInstance(string name, Type shaderType)
        {
            if (shaderType == null) return new Shader(name);

            if (!shaderType.IsSubclassOf(typeof(Shader)))
                throw new Exception(string.Format("Failed to create instance of {0}, this class doesn't inherit from Shader class", shaderType));

            ConstructorInfo[] constructInfo = shaderType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (constructInfo.Length == 0)
                throw new Exception(string.Format("Failed to create instance of {0}, no constructor available", shaderType));

            Shader fx = (Shader)constructInfo[0].Invoke(new object[] { name });

            return fx;
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
