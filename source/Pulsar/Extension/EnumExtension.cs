using System;
using System.Reflection;

namespace Pulsar.Extension
{
    public static class EnumExtension
    {
        #region Methods

        public static T[] GetValues<T>()
        {
            Type t = typeof(T);
            if (t.IsEnum)
            {
                FieldInfo[] infos = t.GetFields(BindingFlags.Static | BindingFlags.Public);
                T[] result = new T[infos.Length];
                for (int i = 0; i < infos.Length; i++)
                {
                    result[i] = (T)infos[i].GetValue(null);
                }

                return result;
            }

            return new T[0];
        }

        #endregion
    }
}
