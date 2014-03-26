﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Pipeline.Graphics;
using Pulsar.Pipeline.Serialization;
using MaterialContent = Pulsar.Pipeline.Graphics.MaterialContent;

namespace Pulsar.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Material - Pulsar")]
    public sealed partial class MaterialDefinitionProcessor : ContentProcessor<RawMaterialContent, MaterialContent>
    {
        #region Fields

        private const char FirstArrayDelimiter = '[';
        private const char LastArrayDelimiter = ']';

        private static readonly Dictionary<string, MaterialDataType> TypeMap = new Dictionary<string, MaterialDataType>
        {
            { "integer", new MaterialDataType(typeof(int), typeof(int)) },
            { "floating" , new MaterialDataType(typeof(float), typeof(float)) },
            { "vector2" , new MaterialDataType(typeof(Vector2), typeof(Vector2)) },
            { "vector3" , new MaterialDataType(typeof(Vector3), typeof(Vector3)) },
            { "vector4" , new MaterialDataType(typeof(Vector4), typeof(Vector4)) },
            { "matrix" , new MaterialDataType(typeof(Matrix), typeof(Matrix)) },
            { "texture" , new MaterialDataType(typeof(ExternalReference<TextureContent>), typeof(Texture)) },
            { "string", new MaterialDataType(typeof(string), typeof(string)) }
        };

        private static readonly DelegateMapper<string> FindRealTypeDelegateMap = new DelegateMapper<string>
        {
            { "vector", (Func<string, string>)GetVectorType }
        };

        private readonly List<MaterialDataContent> _datas = new List<MaterialDataContent>();

        #endregion

        #region Static methods

        private static bool IsArrayValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return (value[0] == FirstArrayDelimiter) && (value[value.Length - 1] == LastArrayDelimiter);
        }

        private static MaterialDataType GetType(string type, string value)
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

            GenerateData(input.Data, context);

            return new MaterialContent(input.Name, input.Shader, _datas);
        }

        private void GenerateData(List<RawMaterialDataContent> rawCollection, ContentProcessorContext context)
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
                    SerializerContext[] contextList = new SerializerContext[rawData.Value.Length];
                    for (int j = 0; j < rawData.Value.Length; j++)
                    {
                        SerializerContext serializerCtx = new SerializerContext(rawData.Parameters[j], context);
                        contextList[j] = serializerCtx;
                    }
                    MaterialDataType type = GetType(rawData.Type.ToLower(), rawData.Value[0]);
                    data.Type = type.RuntimeType;
                    data.Value = readerMngr.ReadMultiples(type.ReaderType, rawData.Value, contextList);
                }
                else
                {
                    string value = rawData.Value[0];
                    MaterialDataType typeInfo = GetType(rawData.Type.ToLower(), value);
                    data.Type = typeInfo.RuntimeType;

                    Type readerType = typeInfo.ReaderType;
                    if (IsArrayValue(value))
                        readerType = readerType.MakeArrayType();

                    SerializerContext serializerCtx = new SerializerContext(rawData.Parameters[0], context);
                    data.Value = readerMngr.Read(readerType, value, serializerCtx);
                }

                _datas.Add(data);
            }
        }

        #endregion
    }
}