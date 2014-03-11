using Pulsar.Graphics.Fx;

namespace Pulsar.Pipeline.Graphics
{
    public class ShaderVariableContent
    {
        #region Constructors

        public ShaderVariableContent(string name)
        {
            Name = name;
            Semantic = string.Empty;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public ShaderVariableSource Source { get; set; }

        public ShaderVariableUsage Usage { get; set; }
        
        public string Semantic { get; set; }

        #endregion
    }
}
