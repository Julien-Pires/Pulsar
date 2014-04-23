namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Contains informations about a built-in shader
    /// </summary>
    internal sealed class BuiltInShaderInfo
    {
        #region Properties

        /// <summary>
        /// Gets the name of the shader
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the path to the shader definition file
        /// </summary>
        public string Path { get; internal set; }

        #endregion
    }
}
