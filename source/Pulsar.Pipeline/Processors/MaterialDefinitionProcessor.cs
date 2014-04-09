using System;
using System.Linq;
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

        private static readonly DelegateMapper<string> FindRealTypeDelegateMap = new DelegateMapper<string>
        {
            { "vector", (Func<string, string>)GetVectorType }
        };

        private readonly List<MaterialDataContent> _datas = new List<MaterialDataContent>();
        private string _shader;
        private string _technique;

        #endregion

        #region Static methods

        private static bool IsArrayValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return (value[0] == FirstArrayDelimiter) && (value[value.Length - 1] == LastArrayDelimiter);
        }

        private static Tuple<Type, Type> GetType(string type, string value)
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

            ExtractTechnique(input.Shader);
            GenerateData(input.Data, context);

            return new MaterialContent(input.Name, _datas)
            {
                Shader = _shader,
                Technique = _technique
            };
        }

        private void ExtractTechnique(string rawShader)
        {
            if(string.IsNullOrWhiteSpace(rawShader))
                throw new ArgumentNullException("rawShader");

            rawShader = rawShader.Replace("/", @"\");
            string[] splitValues = rawShader.Split('/');
            if(splitValues.Length <= 1)
                throw new Exception("");

            _technique = splitValues[splitValues.Length - 1];
            _shader = string.Join(@"\", splitValues.Take(splitValues.Length - 1));
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
                    Tuple<Type, Type> type = GetType(rawData.Type.ToLower(), rawData.Value[0]);
                    data.Type = type.Item2;
                    data.Value = readerMngr.ReadMultiples(type.Item1, rawData.Value, contextList);
                }
                else
                {
                    string value = rawData.Value[0];
                    Tuple<Type, Type> typeInfo = GetType(rawData.Type.ToLower(), value);
                    data.Type = typeInfo.Item2;

                    Type readerType = typeInfo.Item1;
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
