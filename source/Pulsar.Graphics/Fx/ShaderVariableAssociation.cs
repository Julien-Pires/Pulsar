using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed partial class Shader
    {
        private class ShaderVariableAssociation
        {
            #region Constructors

            internal ShaderVariableAssociation(EffectParameter parameter, ShaderVariableDefinition definition)
            {
                Parameter = parameter;
                Definition = definition;
            }

            #endregion

            #region Properties

            public EffectParameter Parameter { get; private set; }

            public ShaderVariableDefinition Definition { get; private set; }

            #endregion
        }
    }
}
