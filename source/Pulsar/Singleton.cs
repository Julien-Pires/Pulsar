using System;
using System.Reflection;

namespace Pulsar
{
    /// <summary>
    /// Base class for singleton class
    /// </summary>
    /// <typeparam name="T">Type of the singleton</typeparam>
    public abstract class Singleton<T> where T : class
    {
        #region Fields

        private static T _instance;
        private static object locker = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;

                lock (locker)
                {
                    if (_instance != null) 
                        return _instance;

                    Type type = typeof(T);
                    ConstructorInfo[] constructInfo = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                    if (constructInfo.Length == 0)
                        throw new Exception(
                            string.Format(
                                "Failed to instantiate instance of type {0}, no private constructor declared", type));

                    _instance = (T)constructInfo[0].Invoke(null);
                }

                return _instance;
            }
        }

        #endregion
    }
}
