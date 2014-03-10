using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    internal static class ShaderVariableBindingFactory
    {
        #region Fields

        private static readonly DelegateMapper<Type> DelegateMapper = new DelegateMapper<Type>
        {
            {typeof(Vector2), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector2Binding},
            {typeof(Vector2[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector2ArrayBinding},
            {typeof(Vector3), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector3Binding},
            {typeof(Vector3[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector3ArrayBinding},
            {typeof(Vector4), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector4Binding},
            {typeof(Vector4[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateVector4ArrayBinding},
            {typeof(Quaternion), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateQuaternionBinding},
            {typeof(Quaternion[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateQuaternionArrayBinding},
            {typeof(Matrix), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateMatrixBinding},
            {typeof(Matrix[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateMatrixArrayBinding},
            {typeof(int), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateIntBinding},
            {typeof(int[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateIntArrayBinding},
            {typeof(float), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateFloatBinding},
            {typeof(float[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateFloatArrayBinding},
            {typeof(bool), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateBoolBinding},
            {typeof(bool[]), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateBoolArrayBinding},
            {typeof(XnaTexture), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateTextureBinding},
            {typeof(string), (Func<ShaderVariableDefinition, ShaderVariableBinding>)CreateStringBinding}
        };

        #endregion

        #region Static methods

        internal static ShaderVariableBinding CreateBinding(ShaderVariableDefinition definition)
        {
            Func<ShaderVariableDefinition, ShaderVariableBinding> create = GetDelegate(definition.Type);

            return create(definition);
        }

        private static Func<ShaderVariableDefinition, ShaderVariableBinding> GetDelegate(Type type)
        {
            return DelegateMapper.GetTypedDelegate<Func<ShaderVariableDefinition, ShaderVariableBinding>>(
                type);
        }

        private static ShaderVariableBinding CreateTypedBinding<T>(ShaderVariableDefinition definition)
        {
            ShaderVariableBinding binding;
            switch (definition.Source)
            {
                case ShaderVariableSource.Auto:
                    binding = new AutomaticBinding<T>(definition);
                    break;

                case ShaderVariableSource.Constant:
                    binding = new ConstantBinding<T>(definition, default(T));
                    break;

                case ShaderVariableSource.Delegate:
                    binding = new DelegateBinding<T>(definition);
                    break;

                case ShaderVariableSource.Keyed:
                    binding = new KeyedBinding<T>(definition);
                    break;

                default:
                    throw new Exception(string.Format("Invalid source provided for shader variable {0}", definition.Name));
            }

            return binding;
        }

        private static ShaderVariableBinding CreateVector2Binding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector2>(definition);
        }

        private static ShaderVariableBinding CreateVector3Binding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector3>(definition);
        }

        private static ShaderVariableBinding CreateVector4Binding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector4>(definition);
        }

        private static ShaderVariableBinding CreateVector2ArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector2[]>(definition);
        }

        private static ShaderVariableBinding CreateVector3ArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector3[]>(definition);
        }

        private static ShaderVariableBinding CreateVector4ArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Vector4[]>(definition);
        }

        private static ShaderVariableBinding CreateQuaternionBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Quaternion>(definition);
        }

        private static ShaderVariableBinding CreateQuaternionArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Quaternion[]>(definition);
        }

        private static ShaderVariableBinding CreateMatrixBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Matrix>(definition);
        }

        private static ShaderVariableBinding CreateMatrixArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<Matrix[]>(definition);
        }

        private static ShaderVariableBinding CreateIntBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<int>(definition);
        }

        private static ShaderVariableBinding CreateIntArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<int[]>(definition);
        }

        private static ShaderVariableBinding CreateFloatBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<float>(definition);
        }

        private static ShaderVariableBinding CreateFloatArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<float[]>(definition);
        }

        private static ShaderVariableBinding CreateBoolBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<bool>(definition);
        }

        private static ShaderVariableBinding CreateBoolArrayBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<bool[]>(definition);
        }

        private static ShaderVariableBinding CreateTextureBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<XnaTexture>(definition);
        }

        private static ShaderVariableBinding CreateStringBinding(ShaderVariableDefinition definition)
        {
            return CreateTypedBinding<string>(definition);
        }

        #endregion
    }
}
