using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class WindowDisplayModeChangedEventArgs : EventArgs
    {
        #region Fields

        private DisplayMode mode;
        private Window window;

        #endregion

        #region Constructor

        public WindowDisplayModeChangedEventArgs(DisplayMode mode, Window win)
        {
            this.mode = mode;
            this.window = win;
        }

        #endregion

        #region Properties

        public DisplayMode Mode
        {
            get { return this.mode; }
        }

        public Window Window
        {
            get { return this.window; }
        }

        #endregion
    }
}
