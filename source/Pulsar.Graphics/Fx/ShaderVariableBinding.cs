using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public abstract class ShaderVariableBinding
    {
        #region Constructors

        protected ShaderVariableBinding(EffectParameter fxParameter)
        {
            Debug.Assert(fxParameter != null, "Effect parameter cannot be null");

            FxParameter = fxParameter;
        }

        #endregion

        #region Methods

        public abstract void Update(FrameContext context);

        public abstract void Write();

        #endregion

        #region Properties
        
        internal EffectParameter FxParameter { get; private set; }

        public string Name
        {
            get { return FxParameter.Name; }
        }

        #endregion
    }
}
