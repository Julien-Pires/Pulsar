using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public abstract class ShaderVariableBinding<T> : ShaderVariableBinding
    {
        #region Fields

        private static readonly Action<ShaderVariableBinding<T>> WriteToParameter;

        internal T InternalValue;

        #endregion

        #region Constructors

        static ShaderVariableBinding()
        {
            WriteToParameter = EffectParameterWriter.GetWriteMethod<T>();
        }

        protected ShaderVariableBinding(EffectParameter fxParameter) 
            : base(fxParameter)
        {
        }

        #endregion

        #region Methods

        public override void Write()
        {
            WriteToParameter(this);
        }

        public override void Update(FrameContext context)
        {
        }

        #endregion

        #region Properties

        public T Value
        {
            get { return InternalValue; }
        }

        #endregion
    }
}
