using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    internal static class EffectParameterWriter
    {
        #region Fields

        private static readonly DelegateMapper<Type> DelegatesMap = new DelegateMapper<Type>
        {
            {typeof(Vector2), (Action<ShaderVariableBinding<Vector2>>)Write},
            {typeof(Vector2[]), (Action<ShaderVariableBinding<Vector2[]>>)Write},
            {typeof(Vector3), (Action<ShaderVariableBinding<Vector3>>)Write},
            {typeof(Vector3[]), (Action<ShaderVariableBinding<Vector3[]>>)Write},
            {typeof(Vector4), (Action<ShaderVariableBinding<Vector4>>)Write},
            {typeof(Vector4[]), (Action<ShaderVariableBinding<Vector4[]>>)Write},
            {typeof(Quaternion), (Action<ShaderVariableBinding<Quaternion>>)Write},
            {typeof(Quaternion[]), (Action<ShaderVariableBinding<Quaternion[]>>)Write},
            {typeof(Matrix), (Action<ShaderVariableBinding<Matrix>>)Write},
            {typeof(Matrix[]), (Action<ShaderVariableBinding<Matrix[]>>)Write},
            {typeof(int), (Action<ShaderVariableBinding<int>>)Write},
            {typeof(int[]), (Action<ShaderVariableBinding<int[]>>)Write},
            {typeof(float), (Action<ShaderVariableBinding<float>>)Write},
            {typeof(float[]), (Action<ShaderVariableBinding<float[]>>)Write},
            {typeof(bool), (Action<ShaderVariableBinding<bool>>)Write},
            {typeof(bool[]), (Action<ShaderVariableBinding<bool[]>>)Write},
            {typeof(XnaTexture), (Action<ShaderVariableBinding<XnaTexture>>)Write},
            {typeof(string), (Action<ShaderVariableBinding<string>>)Write}
        };

        #endregion

        #region Static methods

        public static Action<ShaderVariableBinding<T>> GetWriteMethod<T>()
        {
            return DelegatesMap.GetTypedDelegate<Action<ShaderVariableBinding<T>>>(typeof(T));
        }

        private static void Write(ShaderVariableBinding<Vector2> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Vector2[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Vector3> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Vector3[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Vector4> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Vector4[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Quaternion> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Quaternion[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Matrix> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<Matrix[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<int> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<int[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<float> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<float[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<bool> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<bool[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<XnaTexture> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        private static void Write(ShaderVariableBinding<string> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        #endregion
    }
}