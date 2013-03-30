using System;

using System.Collections.Generic;

namespace Pulsar.Assets.Graphics.Materials
{
    /// <summary>
    /// Class used to load material
    /// </summary>
    public sealed class MaterialManager : Singleton<MaterialManager>, IAssetManager
    {
        #region Fields

        private const string defaultMaterialName = "PulsarSystem_DefaultMaterial";
        private readonly AssetGroup<Material> assetGroup = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MaterialManager class
        /// </summary>
        private MaterialManager()
        {
            this.assetGroup = new AssetGroup<Material>("Material", this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load an empty material
        /// </summary>
        /// <param name="name">Material name</param>
        /// <param name="storage">Storage to stock the material instance</param>
        /// <returns>Return an existing instance of Material if the storage has one, otherwise a new instance</returns>
        public Material LoadEmptyMaterial(string name, string storage)
        {
            AssetSearchResult<Material> result = this.assetGroup.Load(name, storage);
            Material mat = result.Resource;

            return mat;
        }

        /// <summary>
        /// Load the default material containing default texture
        /// </summary>
        /// <returns>Return an existing instance of the default material</returns>
        public Material LoadDefault()
        {
            AssetSearchResult<Material> result = this.assetGroup.Load(MaterialManager.defaultMaterialName, "Default");
            Material mat = result.Resource;

            if (result.Created)
            {
                mat.DiffuseMap = TextureManager.Instance.LoadDefaultTexture();
            }

            return mat;
        }

        /// <summary>
        /// Load a material with different textures
        /// </summary>
        /// <param name="name">Material name</param>
        /// <param name="storage">Storage to stock the material instance</param>
        /// <param name="diffuse">Diffuse texture used by the material</param>
        /// <param name="normalMap">Normal map used by the material</param>
        /// <param name="specular">Specular map used by the material</param>
        /// <returns>Return an existing instance of the material if the storage has one, otherwise a new instance</returns>
        public Material LoadWithTexture(string name, string storage, Texture diffuse, Texture normalMap, Texture specular)
        {
            AssetSearchResult<Material> result = this.assetGroup.Load(name, storage);
            Material mat = result.Resource;

            if (result.Created)
            {
                mat.DiffuseMap = diffuse;
                mat.NormalMap = normalMap;
                mat.SpecularMap = specular;
            }

            return mat;
        }

        /// <summary>
        /// Load a material and fill it with textures loaded by theirs filename
        /// </summary>
        /// <param name="name">Material name</param>
        /// <param name="storage">Storage to stock the material instance</param>
        /// <param name="diffuse">Diffuse texture used by the material</param>
        /// <param name="normalMap">Normal map used by the material</param>
        /// <param name="specular">Specular map used by the material</param>
        /// <returns>Return an existing instance of the material if the storage has one, otherwise a new instance</returns>
        public Material LoadFromFile(string name, string storage, string diffuseMap, string normalMap, string specularMap)
        {
            AssetSearchResult<Material> result = this.assetGroup.Load(name, storage);
            Material mat = result.Resource;

            if (result.Created)
            {
                this.FillMaterial(mat, storage, diffuseMap, normalMap, specularMap);
            }

            return mat;
        }

        /// <summary>
        /// Load textures and fill a material object
        /// </summary>
        /// <param name="mat">Material instance to fill with textures</param>
        /// <param name="storage">Storage to stock the material instance</param>
        /// <param name="diffuse">Diffuse texture used by the material</param>
        /// <param name="normalMap">Normal map used by the material</param>
        /// <param name="specular">Specular map used by the material</param>
        private void FillMaterial(Material mat, string storage, string diffuse, string normal, string specular)
        {
            TextureManager texMngr = TextureManager.Instance;
            Texture diffTex = texMngr.Load(diffuse, storage, diffuse);

            mat.DiffuseMap = diffTex;
        }

        public bool Unload(string name, string storage)
        {
            return this.assetGroup.Unload(name, storage);
        }

        /// <summary>
        /// Create a new empty material instance
        /// </summary>
        /// <param name="name">Material name</param>
        /// <param name="parameter">Additional parameter for loading the instance</param>
        /// <returns>Return a new instance of Material class</returns>
        public Asset CreateInstance(string name, params object[] parameter)
        {
            return new Material(this, name);
        }

        #endregion
    }
}
