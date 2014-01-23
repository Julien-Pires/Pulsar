using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents a loader for Texture asset
    /// </summary>
    public sealed class TextureLoader : AssetLoader
    {
        #region Fields

        internal const string LoaderName = "TextureLoader";

        private const string MissingTexture2DName = "Missing_Texture_2D";
        private const int MissingTexture2DSize = 256;

        private readonly Type[] _supportedTypes = { typeof(Texture), typeof(XnaTexture), typeof(Texture2D) };
        private readonly TextureParameters _defaultParameters;
        private readonly LoadedAsset _result = new LoadedAsset();
        private readonly LoadedAsset _fromFileResult = new LoadedAsset();
        private readonly GraphicsDeviceManager _deviceManager;
        private AssetFolder _textureFolder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TextureLoader class
        /// </summary>
        /// <param name="deviceManager">GraphicsDeviceManager</param>
        internal TextureLoader(GraphicsDeviceManager deviceManager)
        {
            Debug.Assert(deviceManager != null);

            _deviceManager = deviceManager;
            _defaultParameters = new TextureParameters { Level = TextureLevel.Texture2D };
            UseMissingTexture = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the loader
        /// </summary>
        /// <param name="engine">Asset engine that use this loader</param>
        public override void Initialize(AssetEngine engine)
        {
            base.Initialize(engine);

            _textureFolder = engine[GraphicsConstant.Storage][GraphicsConstant.TextureFolderName];
            if (!_textureFolder.IsLoaded(MissingTexture2DName))
            {
                TextureParameters texParams = new TextureParameters
                {
                    Height = MissingTexture2DSize,
                    Width = MissingTexture2DSize
                };
                Texture missingTexture2D = _textureFolder.Load<Texture>(MissingTexture2DName, texParams);
                Texture.CreateCheckerTexture(missingTexture2D, 4, Color.White, Color.Blue);
            }
        }

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
            TextureParameters textureParams;
            if (parameters != null)
            {
                textureParams = parameters as TextureParameters;
                if (textureParams == null)
                    throw new ArgumentException("Invalid parameters for this loader");
            }
            else
            {
                textureParams = _defaultParameters;
                textureParams.Level = TextureLevel.Texture2D;
                textureParams.Filename = path;

                Type textureType = typeof(T);
                if (textureType == typeof (Texture2D))
                    textureParams.Level = TextureLevel.Texture2D;
                else if (textureType == typeof (XnaTexture))
                    textureParams.Level = TextureLevel.Texture;
                else if (textureType == typeof (Texture3D))
                    textureParams.Level = TextureLevel.Texture3D;
            }

            Texture texture;
            switch (textureParams.Source)
            {
                case AssetSource.FromFile:
                    try
                    {
                        switch (textureParams.Level)
                        {
                            case TextureLevel.Texture2D:
                                LoadFromFile<Texture2D>(textureParams.Filename, assetFolder, _fromFileResult);
                                break;
                            case TextureLevel.Texture3D:
                                LoadFromFile<Texture3D>(textureParams.Filename, assetFolder, _fromFileResult);
                                break;
                            case TextureLevel.Texture:
                                LoadFromFile<XnaTexture>(textureParams.Filename, assetFolder, _fromFileResult);
                                break;
                        }

                        texture = new Texture(_deviceManager, assetName, _fromFileResult.Asset as XnaTexture);
                    }
                    catch (Exception)
                    {
                        if(!UseMissingTexture)
                            throw;

                        texture = _textureFolder.Load<Texture>(MissingTexture2DName);
                    }
                    break;
                
                case AssetSource.NewInstance:
                    texture = new Texture(_deviceManager, assetName, textureParams.Level, textureParams.Width, 
                        textureParams.Height, textureParams.Depth, textureParams.MipMap, textureParams.Format);
                    break;

                default:
                    throw new Exception("Invalid asset source provided");
            }
            _fromFileResult.Reset();
            
            _result.Reset();
            _result.Asset = texture;
            _result.Disposables.Add(texture);

            return _result;
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

        /// <summary>
        /// Gets or sets a value that indicate if a missing texture is used when a load failed
        /// </summary>
        public bool UseMissingTexture { get; set; }

        #endregion
    }
}