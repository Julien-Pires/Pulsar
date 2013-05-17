using System;
using System.Reflection;

using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Core;

namespace Pulsar.Assets.Graphics.Shaders
{
    /// <summary>
    /// Class used to load Shader
    /// </summary>
    public sealed class ShaderManager : Singleton<ShaderManager>, IAssetManager
    {
        #region Fields

        private readonly AssetGroup<Shader> assetGroup;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the ShaderManager class
        /// </summary>
        private ShaderManager()
        {
            this.assetGroup = new AssetGroup<Shader>("Shader", this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load a Shader
        /// </summary>
        /// <param name="name">Name of the shader</param>
        /// <param name="storage">Storage in which the shader will be stored</param>
        /// <param name="effect">Name of the effect associated to the shader</param>
        /// <param name="shaderType">Concrete shader type to instantiate</param>
        /// <returns>Return an existing shader instance if the storage has one, otherwise a new instance</returns>
        public Shader LoadShader(string name, string storage, string effect, Type shaderType)
        {
            AssetSearchResult<Shader> result = this.assetGroup.Load(name, storage, shaderType);
            Shader shader = result.Resource;
            if (result.Created)
            {
                AssetStorage usedStorage = AssetStorageManager.Instance.GetStorage(storage);
                Effect fx = usedStorage.ResourceManager.Load<Effect>(effect);
                shader.SetEffect(fx);
            }

            return shader;
        }

        /// <summary>
        /// Unload a shader
        /// </summary>
        /// <param name="name">Name of the shader</param>
        /// <param name="storage">Storage in which the shader is stored</param>
        /// <returns>Returns true if the shader is unloaded successfully otherwise false</returns>
        public bool Unload(string name, string storage)
        {
            return this.assetGroup.Unload(name, storage);
        }

        /// <summary>
        /// Create a new Shader instance
        /// </summary>
        /// <param name="name">Name of the shader</param>
        /// <param name="parameter">Additional parameter to create the shader, in this case 
        /// the methods wait a Type instance describing the concrete type to instantiate</param>
        /// <returns>Return a new shader</returns>
        public Asset CreateInstance(string name, params object[] parameter)
        {
            if ((parameter != null) && (parameter.Length > 0))
            {
                Type type = parameter[0] as Type;
                if (type == null)
                {
                    throw new ArgumentException("Failed to create instance, parameter is not of Type instance");
                }
                if (type.BaseType != typeof(Shader))
                {
                    throw new Exception(string.Format("Failed to create instance of {0}, this class doesn't inherit from Shader class", type));
                }

                ConstructorInfo[] constructInfo = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (constructInfo.Length == 0)
                {
                    throw new Exception(string.Format("Failed to create instance of {0}, no constructor available", type));
                }

                Shader fx = (Shader)constructInfo[0].Invoke(new object[] { this, name });

                return fx;
            }
            else
            {
                return new Shader(this, name);
            }
        }

        #endregion
    }
}
