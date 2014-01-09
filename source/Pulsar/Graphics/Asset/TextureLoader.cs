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

        private const string MissingTexture2DName = "Missing_Texture_2D";
        private const int MissingTexture2DSize = 256;

        private readonly Type[] _supportedTypes = { typeof(Texture) };
        private readonly TextureParameters _defaultParameters;
        private readonly LoadResult _result = new LoadResult();
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly Storage _system;

        #endregion

        #region Constructor

        internal TextureLoader(GraphicsDeviceManager deviceManager, Storage system)
        {
            Debug.Assert(deviceManager != null);
            Debug.Assert(system != null);

            _deviceManager = deviceManager;
            _system = system;
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
            Texture texture = _system.Load<Texture>(MissingTexture2DName, texParams);

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

        public override LoadResult Load<T>(string assetName, object parameters, Storage storage)
        {
            _result.Reset();

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
                textureParams.Filename = assetName;
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
                                LoadFromFile<Texture2D>(textureParams.Filename, storage, _result);
                                break;
                            case TextureLevel.Texture3D:
                                LoadFromFile<Texture3D>(textureParams.Filename, storage, _result);
                                break;
                        }

                        LoadedAsset textureResult = _result[assetName];
                        if (textureResult == null)
                            throw new Exception("");

                        texture = new Texture(_deviceManager, assetName, textureResult.Asset as XnaTexture);
                    }
                    catch (Exception)
                    {
                        if(!UseMissingTexture)
                            throw;

                        if(!_system.IsLoaded(MissingTexture2DName))
                            CreateMissingTexture2D(MissingTexture2DSize, 2, Color.White, Color.Blue);

                        texture = _system.Load<Texture>(MissingTexture2DName);
                    }
                    break;
                
                case AssetSource.NewInstance:
                    texture = new Texture(_deviceManager, assetName, textureParams.Level, textureParams.Width, 
                        textureParams.Height, textureParams.Depth, textureParams.MipMap, textureParams.Format);
                    break;

                default:
                    throw new Exception("");
            }
            _result.Reset();

            LoadedAsset finalTexture = _result.AddAsset(assetName);
            finalTexture.Asset = texture;
            finalTexture.Disposables.Add(texture);

            return _result;
        }

        #endregion

        #region Properties

        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        public bool UseMissingTexture { get; set; }

        #endregion
    }
}