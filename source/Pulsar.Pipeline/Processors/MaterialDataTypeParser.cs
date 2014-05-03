using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Pulsar.Graphics;
using Pulsar.Pipeline.Serialization;

namespace Pulsar.Pipeline.Processors
{
    /// <summary>
    /// Provides mechanism to converts string type for material file to strong type
    /// </summary>
    internal sealed class MaterialDataTypeParser
    {
        #region Fields

        private static readonly Dictionary<string, Tuple<Type, Type>> TypeMap = new Dictionary<string, Tuple<Type, Type>>
        {
            { "integer", new Tuple<Type, Type>(typeof(int), typeof(int)) },
            { "floating" , new Tuple<Type, Type>(typeof(float), typeof(float)) },
            { "vector2" , new Tuple<Type, Type>(typeof(Vector2), typeof(Vector2)) },
            { "vector3" , new Tuple<Type, Type>(typeof(Vector3), typeof(Vector3)) },
            { "vector4" , new Tuple<Type, Type>(typeof(Vector4), typeof(Vector4)) },
            { "matrix" , new Tuple<Type, Type>(typeof(Matrix), typeof(Matrix)) },
            { "texture" , new Tuple<Type, Type>(typeof(ExternalReference<TextureContent>), typeof(Texture)) },
            { "string", new Tuple<Type, Type>(typeof(string), typeof(string)) }
        };

        private static readonly DelegateMapper<string> ConcreteTypeDelegateMap = new DelegateMapper<string>
        {
            { "vector", (Func<string[], string>)GetVectorType }
        };

        #endregion

        #region Static methods

        /// <summary>
        /// Gets strong types for the specified string type
        /// </summary>
        /// <param name="type">String type</param>
        /// <param name="value">Value used to get additional informations about the type</param>
        /// <returns>Returns a tuple of type that represents a build type and a runtime type</returns>
        public static Tuple<Type, Type> GetType(string type, string[] value)
        {
            string realType = type.ToLower();
            Delegate findConcreteType;
            if (ConcreteTypeDelegateMap.TryGetValue(realType, out findConcreteType))
                realType = ((Func<string[], string>)findConcreteType)(value);

            Tuple<Type, Type> result;
            if (!TypeMap.TryGetValue(realType, out result))
                throw new Exception(string.Format("{0} is not a valid material data type", type));

            return result;
        }

        /// <summary>
        /// Parses the specified value to detect the vector type
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns a vector type</returns>
        private static string GetVectorType(string[] value)
        {
            if ((value == null) || (value.Length == 0))
                return "vector4";

            int length = 0;
            for (int i = 0; i < value.Length; i++)
                length = Math.Max(MathSerializerHelper.GetValueCount(value[i]), length);

            if ((length < 2) || (length > 4))
                throw new Exception("Invalid vector format, a vector must have 2 to 4 elements");

            return "vector" + length;
        }

        #endregion
    }
}
