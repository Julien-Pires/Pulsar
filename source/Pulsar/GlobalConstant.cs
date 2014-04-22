using System;

namespace Pulsar
{
    /// <summary>
    /// Contains constant for the project
    /// </summary>
    internal static class GlobalConstant
    {
        #region Fields

        private static string _contentFolder = "Content";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the content folder
        /// </summary>
        public static string ContentFolder
        {
            get { return _contentFolder; }
            internal set
            {
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                _contentFolder = value;
            }
        }

        #endregion
    }
}
