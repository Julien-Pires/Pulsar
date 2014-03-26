using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Assets
{
    public static class ContentReader
    {
        #region Fields

        private static readonly DelegateMapper<Type> ReadMap = new DelegateMapper<Type>
        {
            {typeof(int), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadInt32())},
            {typeof(float), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadSingle())},
            {typeof(Vector2), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadVector2())},
            {typeof(Vector3), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadVector3())},
            {typeof(Vector4), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadVector4())},
            {typeof(Matrix), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadMatrix())},
            {typeof(Quaternion), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadQuaternion())},
            {typeof(string), (Func<Microsoft.Xna.Framework.Content.ContentReader, object>)(c => c.ReadString())}
        };

        #endregion

        #region Static methods

        public static void AddReadObject(Func<Microsoft.Xna.Framework.Content.ContentReader, object> func, Type type)
        {
            ReadMap.Add(type, func);
        }

        public static object ReadObject(this Microsoft.Xna.Framework.Content.ContentReader input, Type type)
        {
            Func<Microsoft.Xna.Framework.Content.ContentReader, object> read = 
                ReadMap.GetTypedDelegate<Func<Microsoft.Xna.Framework.Content.ContentReader, object>>(type);

            return read(input);
        }

        #endregion
    }
}
