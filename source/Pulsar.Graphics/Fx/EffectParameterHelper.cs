using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Contains methods to provides additional information about effect parameter
    /// </summary>
    internal static class EffectParameterHelper
    {
        #region Fields

        private static readonly Dictionary<Type, Dictionary<string, Type>> EquivalentTypeMap =
            new Dictionary<Type, Dictionary<string, Type>>
        {
            {typeof(Vector4), new Dictionary<string, Type>
                {
                    {"quaternion", typeof(Quaternion)}
                }
            }
        };

        #endregion

        #region Static methods

        /// <summary>
        /// Gets the managed type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <param name="equivalentType">Name of the managed type to use instead of the native type</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        public static Type GetManagedType(EffectParameter parameter, string equivalentType)
        {
            Type result = null;
            switch (parameter.ParameterClass)
            {
                case EffectParameterClass.Matrix:
                    result = typeof(Matrix);
                    break;

                case EffectParameterClass.Object:
                    result = GetObjectType(parameter);
                    break;

                case EffectParameterClass.Scalar:
                    result = GetScalarType(parameter);
                    break;

                case EffectParameterClass.Vector:
                    result = GetVectorType(parameter);
                    break;
            }

            if (result == null)
                throw new Exception("Failed to load shader, unsupported parameter type detected");

            if (string.IsNullOrEmpty(equivalentType)) 
                return result;

            result = GetEquivalentType(result, equivalentType);
            if (result == null)
                throw new Exception(string.Format("Cannot use managed type {0} for shader constant {1}",
                    equivalentType, parameter.Name));

            return result;
        }

        /// <summary>
        /// Gets an managed type associated to a native type
        /// </summary>
        /// <param name="native">Native type</param>
        /// <param name="equivalentType">Name of the managed type</param>
        /// <returns>Returns a type instance</returns>
        private static Type GetEquivalentType(Type native, string equivalentType)
        {
            Type result = null;
            Dictionary<string, Type> map;
            if (EquivalentTypeMap.TryGetValue(native, out map))
                map.TryGetValue(equivalentType, out result);

            return result;
        }

        /// <summary>
        /// Gets the vector type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetVectorType(EffectParameter parameter)
        {
            Type result = null;
            switch (parameter.ColumnCount)
            {
                case 2:
                    result = typeof(Vector2);
                    break;

                case 3:
                    result = typeof(Vector3);
                    break;

                case 4:
                    result = typeof(Vector4);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the object type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetObjectType(EffectParameter parameter)
        {
            Type result = null;
            if (parameter.Elements.Count == 0)
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(XnaTexture);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string);
                        break;
                }
            }
            else
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(XnaTexture[]);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D[]);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D[]);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string[]);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the scalar type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetScalarType(EffectParameter parameter)
        {
            Type result = null;
            if (parameter.Elements.Count == 0)
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof(bool);
                        break;

                    case EffectParameterType.Int32:
                        result = typeof(int);
                        break;

                    case EffectParameterType.Single:
                        result = typeof(float);
                        break;
                }
            }
            else
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof(bool[]);
                        break;

                    case EffectParameterType.Int32:
                        result = typeof(int[]);
                        break;

                    case EffectParameterType.Single:
                        result = typeof(float[]);
                        break;
                }
            }

            return result;
        }

        #endregion
    }
}
