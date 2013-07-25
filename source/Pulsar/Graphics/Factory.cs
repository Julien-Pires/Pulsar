using System;
using System.Text;

using System.Collections.Generic;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Factory base class
    /// </summary>
    /// <typeparam name="T">Type of instance create by this factory</typeparam>
    internal abstract class Factory<T>
    {
        #region Methods

        /// <summary>
        /// Method used to create an instance of a specific type
        /// </summary>
        /// <param name="name">Parameter used by the method</param>
        /// <returns>Returns a new instance</returns>
        public abstract T Create(params string[] data);

        #endregion
    }
}
