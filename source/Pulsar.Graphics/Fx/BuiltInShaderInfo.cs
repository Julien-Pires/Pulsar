using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

namespace Pulsar.Graphics.Fx
{
    internal sealed class BuiltInShaderInfo
    {
        #region Properties

        [ContentSerializer]
        public List<BuiltInShader> Shaders { get; internal set; }

        #endregion
    }
}
