using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Pulsar.Pipeline.Serialization.Compiler
{
    public static class ContentWriterHelper
    {
        #region Fields

        private static readonly DelegateMapper<Type> WriteMap = new DelegateMapper<Type>
        {
            {typeof(int), (Action<ContentWriter, object>)Write<int>},
            {typeof(float), (Action<ContentWriter, object>)Write<float>},
            {typeof(Vector2), (Action<ContentWriter, object>)Write<Vector2>},
            {typeof(Vector3), (Action<ContentWriter, object>)Write<Vector3>},
            {typeof(Vector4), (Action<ContentWriter, object>)Write<Vector4>},
            {typeof(Matrix), (Action<ContentWriter, object>)Write<Matrix>},
            {typeof(Quaternion), (Action<ContentWriter, object>)Write<Quaternion>},
            {typeof(string), (Action<ContentWriter, object>)Write<string>},
            {typeof(ExternalReference<TextureContent>), (Action<ContentWriter, object>)Write<ExternalReference<TextureContent>>}
        };

        #endregion

        #region Static methods

        public static void Write(ContentWriter output, object value, Type type)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                Action<ContentWriter, object> write =
                    WriteMap.GetTypedDelegate<Action<ContentWriter, object>>(elementType);

                object[] array = (object[])value;
                output.Write(array.Length);
                for (int i = 0; i < array.Length; i++)
                    write(output, array[i]);
            }
            else
            {
                Action<ContentWriter, object> write =
                    WriteMap.GetTypedDelegate<Action<ContentWriter, object>>(type);
                write(output, value);
            }
        }

        private static void Write<T>(ContentWriter output, object data)
        {
            T typedData = (T) data;
            output.WriteRawObject(typedData);
        }

        #endregion
    }
}
