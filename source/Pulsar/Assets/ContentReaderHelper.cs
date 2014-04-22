using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pulsar.Assets
{
    public static class ContentReaderHelper
    {
        #region Fields

        private static readonly DelegateMapper<Type> ReadMap = new DelegateMapper<Type>
        {
            {typeof(int), (Func<ContentReader, object>)(c => c.ReadInt32())},
            {typeof(float), (Func<ContentReader, object>)(c => c.ReadSingle())},
            {typeof(Vector2), (Func<ContentReader, object>)(c => c.ReadVector2())},
            {typeof(Vector3), (Func<ContentReader, object>)(c => c.ReadVector3())},
            {typeof(Vector4), (Func<ContentReader, object>)(c => c.ReadVector4())},
            {typeof(Matrix), (Func<ContentReader, object>)(c => c.ReadMatrix())},
            {typeof(Quaternion), (Func<ContentReader, object>)(c => c.ReadQuaternion())},
            {typeof(string), (Func<ContentReader, object>)(c => c.ReadString())}
        };

        #endregion

        #region Static methods

        public static void AddReadMethod(Func<ContentReader, object> func, Type type)
        {
            ReadMap.Add(type, func);
        }

        public static object Read(ContentReader input, Type type)
        {
            Debugger.Break();

            object result;
            if (type.IsArray)
            {
                Func<ContentReader, object> read =
                    ReadMap.GetTypedDelegate<Func<ContentReader, object>>(type.GetElementType());
                int length = input.ReadInt32();
                object[] values = new object[length];
                for (int i = 0; i < length; i++)
                    values[i] = read(input);

                result = values;
            }
            else
            {
                Func<ContentReader, object> read =
                    ReadMap.GetTypedDelegate<Func<ContentReader, object>>(type);
                result = read(input);
            }

            return result;
        }

        #endregion
    }
}
