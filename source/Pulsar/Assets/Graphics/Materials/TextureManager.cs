using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Core;

namespace Pulsar.Assets.Graphics.Materials
{
    /// <summary>
    /// Class used to load Texture
    /// </summary>
    public sealed class TextureManager : Singleton<TextureManager>, IAssetManager<Texture>
    {
        #region Fields

        private const string defaultTextureName = "DefaultTexture_PangolinSystem";
        private readonly AssetGroup<Texture> assetGroup;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TextureManager class
        /// </summary>
        private TextureManager()
        {
            this.assetGroup = new AssetGroup<Texture>("Texture", this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load a Texture instance with a Texture2D
        /// </summary>
        /// <param name="name">Texture name</param>
        /// <param name="storage">Storage to stock the Texture instance</param>
        /// <param name="tex2D">Texture2D associated to Texture instance</param>
        /// <returns>Return an existing instance of Texture class if the storage has one, otherwise a new instance</returns>
        public Texture Load(string name, string storage, Texture2D tex2D)
        {
            AssetSearchResult<Texture> result = this.assetGroup.Load(name, storage);
            Texture tex = result.Resource;

            if (result.Created)
            {
                tex.Image = tex2D;
            }

            return tex;
        }

        /// <summary>
        /// Load a Texture instance with a Texture2D by its filename
        /// </summary>
        /// <param name="name">Texture name</param>
        /// <param name="storage">Storage to stock the Texture instance</param>
        /// <param name="file">Filename of the Texture2D</param>
        /// <returns>Return an existing instance of Texture class if the storage has one, otherwise a new instance</returns>
        public Texture Load(string name, string storage, string file)
        {
            AssetSearchResult<Texture> result = this.assetGroup.Load(name, storage);
            Texture tex = result.Resource;

            if (result.Created)
            {
                Texture2D image = AssetStorageManager.Instance.Content.Load<Texture2D>(file);
                tex.Image = image;
                tex.File = file;
            }

            return tex;
        }

        /// <summary>
        /// Load the default texture
        /// </summary>
        /// <returns>Return an instance of the default texture</returns>
        public Texture LoadDefaultTexture()
        {
            AssetSearchResult<Texture> result = this.assetGroup.Load(TextureManager.defaultTextureName, "Default");
            Texture tex = result.Resource;

            if (result.Created)
            {
                Texture2D image = Texture.CreateMissingTexture();
                tex.Image = image;
            }

            return tex;
        }

        /// <summary>
        /// Create a new Texture class instance
        /// </summary>
        /// <param name="name">Texture name</param>
        /// <param name="parameter">Addtional parameter for loading the instance</param>
        /// <returns>Return a new instance</returns>
        public Texture CreateInstance(string name, object parameter = null)
        {
            return new Texture(this, name);
        }

        #endregion
    }
}