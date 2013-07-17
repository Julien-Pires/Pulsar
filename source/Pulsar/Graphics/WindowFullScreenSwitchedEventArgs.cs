using System;

namespace Pulsar.Graphics
{
    public sealed class WindowFullScreenSwitchedEventArgs : EventArgs
    {
        #region Fields

        private bool isFullScreen;
        private Window window;

        #endregion

        #region Constructor

        public WindowFullScreenSwitchedEventArgs(bool fullScreen, Window win)
        {
            this.isFullScreen = fullScreen;
            this.window = win;
        }

        #endregion

        #region Properties

        public bool IsFullScreen
        {
            get { return this.isFullScreen; }
        }

        public Window Window
        {
            get { return this.window; }
        }

        #endregion
    }
}
