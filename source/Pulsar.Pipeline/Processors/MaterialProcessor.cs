using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Fx;
using Pulsar.Pipeline.Graphics;
using Pulsar.Pipeline.Serialization;

namespace Pulsar.Pipeline.Processors
{
    public sealed class MaterialProcessor : ContentProcessor<RawMaterialContent, MaterialContent>
    {
        #region Fields

        private const char FirstArrayDelimiter = '[';
        private const char LastArrayDelimiter = ']';

        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>
        {
            { "integer", typeof(int) },
            { "floating" , typeof(float) },
            { "vector2" , typeof(Vector2) },
            { "vector3" , typeof(Vector3) },
            { "vector4" , typeof(Vector4) },
            { "matrix" , typeof(Matrix) },
            { "texture" , typeof(Texture) },
            { "string", typeof(string) }
        };

        private static readonly DelegateMapper<string> FindRealTypeDelegateMap = new DelegateMapper<string>
        {
            { "vector", (Func<string, string>)GetVectorType }
        };

        #endregion

        #region Static methods

        private static void GenerateData(List<RawMaterialDataContent> rawCollection, MaterialContent content,
            ContentProcessorContext context)
        {
            ReaderManager readerMngr = new ReaderManager();
            for (int i = 0; i < rawCollection.Count; i++)
            {
                RawMaterialDataContent rawData = rawCollection[i];
                if (rawData.Value.Length == 0)
                    continue;

                MaterialDataContent data = new MaterialDataContent(rawData.Name);
                if (rawData.IsNativeArray)
                {
                    ReaderContext[] contextList = new ReaderContext[rawData.Value.Length];
                    for (int j = 0; j < rawData.Value.Length; j++)
                    {
                        ReaderContext readerCtx = new ReaderContext(rawData.Parameters[j], context);
                        contextList[j] = readerCtx;
                    }
                    Type type = GetType(rawData.Type, rawData.Value[0]);
                    data.Value = readerMngr.ReadMultiples(type, rawData.Value, contextList);
                }
                else
                {
                    string value = rawData.Value[0];
                    Type type = GetType(rawData.Type, value);
                    if (IsArrayValue(value))
                        type = type.MakeArrayType();

                    ReaderContext readerCtx = new ReaderContext(rawData.Parameters[0], context);
                    data.Value = readerMngr.Read(type, value, readerCtx);
                }

                content.Data.Add(data);
            }
        }

        private static bool IsArrayValue(string value)
        {
            return (value[0] == FirstArrayDelimiter) && (value[value.Length - 1] == LastArrayDelimiter);
        }

        private static Type GetType(string type, string value)
        {
            if (!FindRealTypeDelegateMap.ContainsKey(type))
                return TypeMap[type];

            Func<string, string> findType = FindRealTypeDelegateMap.GetTypedDelegate<Func<string, string>>(type);
            type = findType(value);

            return TypeMap[type];
        }

        private static string GetVectorType(string value)
        {
            int length = MathSerializerHelper.GetValueCount(value);
            if((length < 2) || (length > 4))
                throw new Exception("");

            return "vector" + length;
        }

        #endregion

        #region Methods

        public override MaterialContent Process(RawMaterialContent input, ContentProcessorContext context)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (context == null)
                throw new ArgumentNullException("context");

            MaterialContent material = new MaterialContent(input.Name);
            GenerateData(input.Data, material, context);

            return material;
        }

        #endregion
    }
}
