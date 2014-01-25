using System;

namespace Pulsar
{
    internal static class GlobalConstant
    {
        #region Fields

        private static string _contentFolder = "Content";

        #endregion

        #region Properties

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
