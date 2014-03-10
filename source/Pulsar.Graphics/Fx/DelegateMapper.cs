using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    internal class DelegateMapper<TKey> : Dictionary<TKey, Delegate>
    {
        #region Methods

        public T GetTypedDelegate<T>(TKey key) where T : class
        {
            Type type = typeof(T);
            if(!type.IsSubclassOf(typeof(Delegate)))
                throw new Exception(string.Format("{0} is not a delegate", type));

            Delegate method;
            if (!TryGetValue(key, out method))
                throw new Exception(string.Format("Failed to find delegate {0}", key));

            T result = method as T;
            if(result == null)
                throw new Exception(string.Format("Failed to cast {0} to {1}", method.GetType(), type));

            return result;
        }

        #endregion
    }
}
