using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    internal class BuiltInShaderManager
    {
        #region Static

        private const string BuiltInFile = "";

        private readonly GraphicsStorage _storage;
        private readonly Dictionary<string, BuiltInShaderInfo> _shaderMap =
            new Dictionary<string, BuiltInShaderInfo>();

        #endregion

        #region Constructors

        internal BuiltInShaderManager(GraphicsStorage storage)
        {
            Debug.Assert(storage != null);

            _storage = storage;
            /*List<BuiltInShaderInfo> shadersList = _storage.ShaderFolder.Load<List<BuiltInShaderInfo>>(BuiltInFile);
            for (int i = 0; i < shadersList.Count; i++)
            {
                BuiltInShaderInfo shaderInfo = shadersList[i];
                _shaderMap.Add(shaderInfo.Name, shaderInfo);
            }*/
        }

        #endregion

        #region Methods

        public Shader GetShader(string name)
        {
            BuiltInShaderInfo shaderInfo;
            if (!_shaderMap.TryGetValue(name, out shaderInfo))
                return null;

            Shader shader = _storage.ShaderFolder.Load<Shader>(shaderInfo.Path);

            return shader;
        }

        #endregion
    }
}
