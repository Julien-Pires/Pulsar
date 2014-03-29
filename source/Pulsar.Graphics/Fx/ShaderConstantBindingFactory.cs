using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    /// <summary>
    /// Provides methods to create strongly-typed shader constant binding
    /// </summary>
    internal static class ShaderConstantBindingFactory
    {
        #region Fields

        private static readonly DelegateMapper<Type> DelegateMapper = new DelegateMapper<Type>
        {
            {typeof(Vector2), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector2Binding},
            {typeof(Vector2[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector2ArrayBinding},
            {typeof(Vector3), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector3Binding},
            {typeof(Vector3[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector3ArrayBinding},
            {typeof(Vector4), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector4Binding},
            {typeof(Vector4[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateVector4ArrayBinding},
            {typeof(Quaternion), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateQuaternionBinding},
            {typeof(Quaternion[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateQuaternionArrayBinding},
            {typeof(Matrix), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateMatrixBinding},
            {typeof(Matrix[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateMatrixArrayBinding},
            {typeof(int), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateIntBinding},
            {typeof(int[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateIntArrayBinding},
            {typeof(float), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateFloatBinding},
            {typeof(float[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateFloatArrayBinding},
            {typeof(bool), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateBoolBinding},
            {typeof(bool[]), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateBoolArrayBinding},
            {typeof(XnaTexture), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateTextureBinding},
            {typeof(string), (Func<ShaderConstantDefinition, ShaderConstantBinding>)CreateStringBinding}
        };

        #endregion

        #region Static methods

        /// <summary>
        /// Creates a strongly-typed shader constant binding
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        internal static ShaderConstantBinding CreateBinding(ShaderConstantDefinition definition)
        {
            Func<ShaderConstantDefinition, ShaderConstantBinding> create = GetDelegate(definition.Type);

            return create(definition);
        }

        /// <summary>
        /// Gets a delegate used to create a strongly-typed binding
        /// </summary>
        /// <param name="type">Type of the constant</param>
        /// <returns>Returns a delegate</returns>
        private static Func<ShaderConstantDefinition, ShaderConstantBinding> GetDelegate(Type type)
        {
            return DelegateMapper.GetTypedDelegate<Func<ShaderConstantDefinition, ShaderConstantBinding>>(
                type);
        }

        /// <summary>
        /// Creates a specific binding
        /// </summary>
        /// <typeparam name="T">Type of the constant</typeparam>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateTypedBinding<T>(ShaderConstantDefinition definition)
        {
            ShaderConstantBinding binding;
            switch (definition.Source)
            {
                case ShaderConstantSource.Auto:
                    binding = new AutomaticBinding<T>(definition);
                    break;

                case ShaderConstantSource.Constant:
                    binding = new ConstantBinding<T>(definition);
                    break;

                case ShaderConstantSource.Delegate:
                    binding = new DelegateBinding<T>(definition);
                    break;

                case ShaderConstantSource.Keyed:
                    binding = new KeyedBinding<T>(definition);
                    break;

                default:
                    throw new Exception(string.Format("Invalid source provided for shader variable {0}", definition.Name));
            }

            return binding;
        }

        /// <summary>
        /// Creates a binding for a vector2
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector2Binding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector2>(definition);
        }

        /// <summary>
        /// Creates a binding for a vector3
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector3Binding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector3>(definition);
        }

        /// <summary>
        /// Creates a binding for a vector4
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector4Binding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector4>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of vector2
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector2ArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector2[]>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of vector3
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector3ArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector3[]>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of vector4
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateVector4ArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Vector4[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a quaternion
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateQuaternionBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Quaternion>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of quaternion
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateQuaternionArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Quaternion[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a matrix
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateMatrixBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Matrix>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of matrix
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateMatrixArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<Matrix[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a int
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateIntBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<int>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of int
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateIntArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<int[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a float
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateFloatBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<float>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of float
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateFloatArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<float[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a bool
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateBoolBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<bool>(definition);
        }

        /// <summary>
        /// Creates a binding for an array of bool
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateBoolArrayBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<bool[]>(definition);
        }

        /// <summary>
        /// Creates a binding for a texture
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateTextureBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<XnaTexture>(definition);
        }

        /// <summary>
        /// Creates a binding for a string
        /// </summary>
        /// <param name="definition">Constant definition</param>
        /// <returns>Returns an object that represents a binding to a shader constant</returns>
        private static ShaderConstantBinding CreateStringBinding(ShaderConstantDefinition definition)
        {
            return CreateTypedBinding<string>(definition);
        }

        #endregion
    }
}
