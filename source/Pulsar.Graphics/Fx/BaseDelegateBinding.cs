using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public abstract class BaseDelegateBinding<T> : ShaderVariableBinding<T>
    {
        #region Fields

        protected Func<FrameContext, T> InternalUpdateFunction;

        #endregion

        #region Constructors

        internal BaseDelegateBinding(EffectParameter fxParameter) 
            : base(fxParameter)
        {
        }

        #endregion

        #region Methods

        public override void Update(FrameContext renderable)
        {
            InternalValue = InternalUpdateFunction(renderable);
        }

        #endregion

        #region Properties

        public Func<FrameContext, T> UpdateFunction
        {
            get { return InternalUpdateFunction; }
        }

        #endregion
    }
}
