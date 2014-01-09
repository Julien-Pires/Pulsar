using System;

namespace Pulsar.Graphics
{
    public static class GraphicsConstant
    {
        #region Fields

        private static string _storage = "Graphics_Engine_Storage";

        #endregion

        #region Properties

        public static string Storage
        {
            get { return _storage; }
            set
            {
                if(string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                _storage = value;
            }
        }

        #endregion
    }
}
