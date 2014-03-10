using System;

#if XBOX || XBOX360
using System.Reflection;
#endif

namespace Pulsar.Extension
{
    /// <summary>
    /// Class containing extension method for Enum class
    /// </summary>
    public static class EnumExtension
    {
        #region Static methods

        public static bool TryParse<T>(string value, bool ignoreCase, out T result) where T : struct
        {
#if WINDOWS
            return Enum.TryParse(value, ignoreCase, out result);
#elif XBOX || XBOX360
            result = default(T);
            bool success;
            try
            {
                result = (T)Enum.Parse(typeof (T), value, ignoreCase);

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
#endif
        }

        /// <summary>
        /// Get an array containing all values for a specific enum
        /// </summary>
        /// <typeparam name="T">Type of enum on which to extract values</typeparam>
        /// <returns>Return an array with enum values</returns>
        public static T[] GetValues<T>()
        {
#if WINDOWS
            return Enum.GetValues(typeof (T)) as T[];
#elif XBOX || XBOX360
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
#endif
        }

        #endregion
    }
}
