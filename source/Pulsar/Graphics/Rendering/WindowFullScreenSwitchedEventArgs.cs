using System;

namespace Pulsar.Graphics.Rendering
{
    public sealed class WindowFullScreenSwitchedEventArgs : EventArgs
    {
        #region Fields

        private readonly bool _isFullScreen;
        private readonly Window _window;

        #endregion

        #region Constructor

        public WindowFullScreenSwitchedEventArgs(bool fullScreen, Window win)
        {
            _isFullScreen = fullScreen;
            _window = win;
        }

        #endregion

        #region Properties

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
        }

        public Window Window
        {
            get { return _window; }
        }

        #endregion
    }
}
