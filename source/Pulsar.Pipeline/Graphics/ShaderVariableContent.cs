using Pulsar.Graphics.Fx;

namespace Pulsar.Pipeline.Graphics
{
    public class ShaderVariableContent
    {
        #region Properties

        public string Name { get; set; }

        public ShaderVariableSource Source { get; set; }

        public ShaderVariableUsage Usage { get; set; }
        
        public string Semantic { get; set; }

        public string Type { get; set; }

        #endregion
    }
}
