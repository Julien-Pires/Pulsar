using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Manages all built-in shaders of the engine
    /// </summary>
    internal class BuiltInShaderManager
    {
        #region Static

        private const string BuiltInFile = "ShadersList";

        private readonly GraphicsStorage _storage;
        private readonly Dictionary<string, BuiltInShader> _shaderMap =
            new Dictionary<string, BuiltInShader>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of BuiltInShaderManager class
        /// </summary>
        /// <param name="storage">Storage used by the graphics engine</param>
        internal BuiltInShaderManager(GraphicsStorage storage)
        {
            Debug.Assert(storage != null);
            
            _storage = storage;
            BuiltInShaderInfo shadersInfo = _storage.ShaderFolder.Load<BuiltInShaderInfo>(BuiltInFile);
            List<BuiltInShader> shadersData = shadersInfo.Shaders;
            for (int i = 0; i < shadersData.Count; i++)
            {
                BuiltInShader shader = shadersData[i];
                _shaderMap.Add(shader.Name, shader);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a shader for a specified name
        /// </summary>
        /// <param name="name">Name of the shader</param>
        /// <returns>Returns a Shader instance if found otherwise null</returns>
        public Shader GetShader(string name)
        {
            BuiltInShader shaderInfo;
            if (!_shaderMap.TryGetValue(name, out shaderInfo))
                return null;

            Shader shader = _storage.ShaderFolder.Load<Shader>(shaderInfo.Path);

            return shader;
        }

        #endregion
    }
}
