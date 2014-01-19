using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets;

using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Asset
{
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
        private readonly AssetFolder _textureFolder;

        #endregion

        #region Constructor

        internal TextureLoader(GraphicsDeviceManager deviceManager, Storage graphicsStorage)
        {
            Debug.Assert(deviceManager != null);
            Debug.Assert(graphicsStorage != null);

            _deviceManager = deviceManager;
            _textureFolder = graphicsStorage[GraphicsConstant.TextureFolderName];
            _defaultParameters = new TextureParameters { Level = TextureLevel.Texture2D };
            UseMissingTexture = true;
        }

        #endregion

        #region Methods

        private void CreateMissingTexture2D(int size, int stripSize, Color odd, Color even)
        {
            TextureParameters texParams = new TextureParameters
            {
                Height = size,
                Width = size
            };
            Texture texture = _textureFolder.Load<Texture>(MissingTexture2DName, texParams);

            Color[] textureData = new Color[size * size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    bool hasColor = (((x & stripSize) == 0) ^ ((y & stripSize) == 0));
                    int idx = (x * size) + y;

                    if (hasColor)
                        textureData[idx] = odd;
                    else
                        textureData[idx] = even;
                }
            }
            texture.SetData(textureData);
        }

        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            TextureParameters textureParams;
            if (parameters != null)
            {
                textureParams = parameters as TextureParameters;
                if (textureParams == null)
                    throw new ArgumentException("");
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

                        if(!_textureFolder.IsLoaded(MissingTexture2DName))
                            CreateMissingTexture2D(MissingTexture2DSize, 2, Color.White, Color.Blue);

                        texture = _textureFolder.Load<Texture>(MissingTexture2DName);
                    }
                    break;
                
                case AssetSource.NewInstance:
                    texture = new Texture(_deviceManager, assetName, textureParams.Level, textureParams.Width, 
                        textureParams.Height, textureParams.Depth, textureParams.MipMap, textureParams.Format);
                    break;

                default:
                    throw new Exception("");
            }
            _fromFileResult.Reset();
            
            _result.Reset();
            _result.Name = assetName;
            _result.Asset = texture;
            _result.Disposables.Add(texture);

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

        public bool UseMissingTexture { get; set; }

        #endregion
    }
}