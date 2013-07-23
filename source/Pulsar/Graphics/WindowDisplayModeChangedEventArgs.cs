using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class WindowDisplayModeChangedEventArgs : EventArgs
    {
        #region Fields

        private readonly DisplayMode _mode;
        private readonly Window _window;

        #endregion

        #region Constructor

        public WindowDisplayModeChangedEventArgs(DisplayMode mode, Window win)
        {
            _mode = mode;
            _window = win;
        }

        #endregion

        #region Properties

        public DisplayMode Mode
        {
            get { return _mode; }
        }

        public Window Window
        {
            get { return _window; }
        }

        #endregion
    }
}
