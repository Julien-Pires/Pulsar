using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using Pulsar.Pipeline.Graphics;
using Pulsar.Pipeline.Serialization;
using Texture = Pulsar.Graphics.Texture;
using MaterialContent = Pulsar.Pipeline.Graphics.MaterialContent;

namespace Pulsar.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Material - Pulsar")]
    public sealed class MaterialDefinitionProcessor : ContentProcessor<RawMaterialContent, MaterialContent>
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

        private static readonly DelegateMapper<string> FindRealTypeDelegateMap = new DelegateMapper<string>
        {
            { "vector", (Func<string[], string>)GetVectorType }
        };

        private readonly List<MaterialDataContent> _datas = new List<MaterialDataContent>();
        private readonly ReaderManager _readerManager = new ReaderManager();
        private string _shader;
        private string _technique;

        #endregion

        #region Static methods

        private static Tuple<Type, Type> GetType(string type, string[] value)
        {
            if (!FindRealTypeDelegateMap.ContainsKey(type))
                return TypeMap[type];

            Func<string[], string> findType = FindRealTypeDelegateMap.GetTypedDelegate<Func<string[], string>>(type);
            type = findType(value);

            return TypeMap[type];
        }

        private static string GetVectorType(string[] value)
        {
            if ((value == null) || (value.Length == 0))
                return "vector4";

            int length = 2;
            for (int i = 0; i < value.Length; i++)
                length = Math.Max(MathSerializerHelper.GetValueCount(value[i]), length);

            if ((length < 2) || (length > 4))
                throw new Exception("Invalid vector format, a vector must have 2 to 4 number");

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
            string[] splitValues = rawShader.Split('\\');
            if(splitValues.Length <= 1)
                throw new Exception("");

            _technique = splitValues[splitValues.Length - 1];
            _shader = string.Join(@"\", splitValues.Take(splitValues.Length - 1));
        }

        private void GenerateData(List<RawMaterialDataContent> rawCollection, ContentProcessorContext context)
        {
            for (int i = 0; i < rawCollection.Count; i++)
            {
                RawMaterialDataContent rawData = rawCollection[i];
                if (rawData.Value.Length == 0)
                    continue;

                Tuple<Type, Type> type = GetType(rawData.Type.ToLower(), rawData.Value);
                SerializerContext serializerCtx = new SerializerContext(context);
                MaterialDataContent data = new MaterialDataContent(rawData.Name);
                List<Dictionary<string, object>> parameters = rawData.Parameters;
                if (rawData.IsNativeArray)
                {
                    object[] values = new object[rawData.Value.Length];
                    for (int j = 0; j < rawData.Value.Length; j++)
                    {
                        int parametersIdx = Math.Min(j, parameters.Count - 1);
                        serializerCtx.Parameters = (parameters.Count > 0) ? parameters[parametersIdx] : null;
                        values[j] = _readerManager.Read(type.Item1, rawData.Value[j], serializerCtx);
                    }
                    data.Value = values;
                    data.BuildType = type.Item1.MakeArrayType();
                    data.RuntimeType = type.Item2.MakeArrayType();
                }
                else
                {
                    serializerCtx.Parameters = (parameters.Count > 0) ? parameters[0] : null;
                    data.Value = _readerManager.Read(type.Item1, rawData.Value[0], serializerCtx);
                    data.BuildType = type.Item1;
                    data.RuntimeType = type.Item2;
                }

                _datas.Add(data);
            }
        }

        #endregion
    }
}
