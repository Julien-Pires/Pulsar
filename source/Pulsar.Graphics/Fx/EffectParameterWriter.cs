using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    /// <summary>
    /// Manages a collection of delegate used to write data to an effect parameter
    /// </summary>
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

        /// <summary>
        /// Get a strongly typed method to write a specific value
        /// </summary>
        /// <typeparam name="T">Type of the value to write</typeparam>
        /// <returns>Returns a delegate</returns>
        public static Action<ShaderVariableBinding<T>> GetWriteMethod<T>()
        {
            return DelegatesMap.GetTypedDelegate<Action<ShaderVariableBinding<T>>>(typeof(T));
        }

        /// <summary>
        /// Writes a vector2 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector2> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of vector2 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector2[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a vector3 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector3> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of vector3 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector3[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a vector4 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector4> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of vector4 to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Vector4[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a quaternion to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Quaternion> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of quaternion to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Quaternion[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a matrix to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Matrix> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of matrix to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<Matrix[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an int to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<int> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of int to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<int[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a float to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<float> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of float to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<float[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a bool to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<bool> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes an array of bool to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<bool[]> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a texture to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<XnaTexture> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        /// <summary>
        /// Writes a string to the effect parameter
        /// </summary>
        /// <param name="binding">Binding that contains the data to write</param>
        private static void Write(ShaderVariableBinding<string> binding)
        {
            binding.FxParameter.SetValue(binding.InternalValue);
        }

        #endregion
    }
}