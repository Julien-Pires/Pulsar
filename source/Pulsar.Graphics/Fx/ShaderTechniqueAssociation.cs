using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed partial class Shader
    {
        private class ShaderTechniqueAssociation
        {
            #region Constructors

            internal ShaderTechniqueAssociation(EffectTechnique technique, ShaderTechniqueDefinition definition)
            {
                Technique = technique;
                Definition = definition;
            }

            #endregion

            #region Properties

            public EffectTechnique Technique { get; private set; }

            public ShaderTechniqueDefinition Definition { get; private set; }

            #endregion
        }
    }
}
