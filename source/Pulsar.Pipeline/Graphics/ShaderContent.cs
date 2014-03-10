using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Pulsar.Pipeline.Graphics
{
    public class ShaderContent
    {
        #region Fields

        private readonly ShaderVariableCollection _variables = new ShaderVariableCollection();
        private readonly ShaderTechniqueCollection _techniques = new ShaderTechniqueCollection();

        #endregion

        #region Properties

        public ExternalReference<EffectContent> Effect { get; set; }

        public ShaderVariableCollection Variables
        {
            get { return _variables; }
        }

        public ShaderTechniqueCollection Techniques
        {
            get { return _techniques; }
        }

        public string InstancingTechnique { get; set; }

        public string Fallback { get; set; }

        #endregion
    }
}
